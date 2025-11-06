using Application.Abstractions;
using Application.Common;
using Application.Products.DTOs;
using Application.Products.Queries;
using MediatR;


namespace Application.Products.Handlers
{
    public sealed class GetProductsByCategorySlugHandler(
        ICategoryRepository categories,
        IProductReadRepository products
    ) : IRequestHandler<GetProductsByCategorySlugQuery, PagedResult<ProductListItemDto>>
    {
        public async Task<PagedResult<ProductListItemDto>> Handle(GetProductsByCategorySlugQuery request, CancellationToken ct)
        {
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 24;

            if (string.IsNullOrWhiteSpace(request.SlugPath))
            {
                var (itemsAll, totalAll) = await products.GetAllAsync(page, pageSize, ct);
                return new PagedResult<ProductListItemDto>(itemsAll, totalAll, page, pageSize);
            }

            // (Id, ParentId, Slug)
            var flat = await categories.GetFlatAsync(ct);

            // Ключ словаря — всегда GUID (null -> Guid.Empty)
            var byParent = flat
                .GroupBy(c => NormalizeParent(c.ParentId))
                .ToDictionary(g => g.Key, g => g.ToList());

            // Разбираем путь "a/b/c" (без учета регистра)
            var parts = request.SlugPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var currentParent = (Guid?)null; // стартуем с корня
            (Guid Id, Guid? ParentId, string Slug)? current = null;

            foreach (var slug in parts)
            {
                var key = NormalizeParent(currentParent);
                if (!byParent.TryGetValue(key, out var children))
                    throw new KeyNotFoundException($"Категория по пути '{request.SlugPath}' не найдена.");

                current = children.FirstOrDefault(c => string.Equals(c.Slug, slug, StringComparison.OrdinalIgnoreCase));
                if (current is null)
                    throw new KeyNotFoundException($"Категория по пути '{request.SlugPath}' не найдена.");

                currentParent = current.Value.Id; // переходим на следующий уровень
            }

            var targetId = current!.Value.Id;

            // Собираем все потомки (включая саму)
            var allIds = CollectDescendants(byParent, targetId);

            var (items, total) = await products.GetByCategoryIdsAsync(allIds, page, pageSize, ct);
            return new PagedResult<ProductListItemDto>(items, total, page, pageSize);
        }

        // null / Guid.Empty -> Guid.Empty; иначе возвращаем сам Guid
        private static Guid NormalizeParent(Guid? parentId) =>
            parentId.HasValue && parentId.Value != Guid.Empty ? parentId.Value : Guid.Empty;

        // byParent: Dictionary<Guid, List<(Id, ParentId, Slug)>>, где Guid.Empty — «корень»
        private static IReadOnlyList<Guid> CollectDescendants(
            Dictionary<Guid, List<(Guid Id, Guid? ParentId, string Slug)>> byParent,
            Guid rootId)
        {
            var result = new List<Guid> { rootId };
            var queue = new Queue<Guid>();
            queue.Enqueue(rootId);

            while (queue.Count > 0)
            {
                var pid = queue.Dequeue();
                if (!byParent.TryGetValue(pid, out var children)) continue;

                foreach (var ch in children)
                {
                    result.Add(ch.Id);
                    queue.Enqueue(ch.Id);
                }
            }

            return result;
        }
    }
}
