using Application.Abstractions;
using Application.Products.Queries;
using MediatR;


namespace Application.Products.Handlers
{
    public sealed class GetProductsByCategorySlugHandler(
        ICategoryRepository categories,
        IProductReadRepository products
    ) : IRequestHandler<GetProductsByCategorySlugQuery, ProductListResultDto>
    {
        public async Task<ProductListResultDto> Handle(GetProductsByCategorySlugQuery request, CancellationToken ct)
        {
            // если slugPath не указан — вернуть просто список товаров
            if (string.IsNullOrWhiteSpace(request.SlugPath))
            {
                var (itemsAll, totalAll) = await products.GetAllAsync(request.Page, request.PageSize, ct);
                return new ProductListResultDto(itemsAll, totalAll, request.Page, request.PageSize);
            }

            // 1. вытаскиваем все категории плоско
            var flat = await categories.GetFlatAsync(ct);
            // flat: (Id, ParentId, Slug)

            // 2. разбиваем путь
            var parts = request.SlugPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
            {
                var (itemsAll, totalAll) = await products.GetAllAsync(request.Page, request.PageSize, ct);
                return new ProductListResultDto(itemsAll, totalAll, request.Page, request.PageSize);
            }

            // 3. находим категорию по пути slug'ов
            var currentParentId = (Guid?)null;
            (Guid Id, Guid? ParentId, string Slug)? current = null;
            foreach (var slug in parts)
            {
                current = flat.FirstOrDefault(c => c.Slug == slug && c.ParentId == currentParentId);
                if (current is null)
                    throw new KeyNotFoundException($"Категория по пути '{request.SlugPath}' не найдена.");
                currentParentId = current.Value.Id;
            }

            var targetCategoryId = current!.Value.Id;

            // 4. собираем все дочерние категории (в т.ч. саму)
            var allIds = CollectDescendants(flat, targetCategoryId);

            // 5. вытаскиваем товары по этим категориям
            var (items, total) = await products.GetByCategoryIdsAsync(allIds, request.Page, request.PageSize, ct);

            return new ProductListResultDto(items, total, request.Page, request.PageSize);
        }

        private static IReadOnlyList<Guid> CollectDescendants(
            IReadOnlyList<(Guid Id, Guid? ParentId, string Slug)> flat,
            Guid rootId)
        {
            var result = new List<Guid> { rootId };

            // очередь в ширину
            var queue = new Queue<Guid>();
            queue.Enqueue(rootId);

            while (queue.Count > 0)
            {
                var parentId = queue.Dequeue();
                var children = flat.Where(c => c.ParentId == parentId).Select(c => c.Id).ToList();
                foreach (var childId in children)
                {
                    result.Add(childId);
                    queue.Enqueue(childId);
                }
            }

            return result;
        }
    }
}
