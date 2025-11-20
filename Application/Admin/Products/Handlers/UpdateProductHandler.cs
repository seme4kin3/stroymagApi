using Application.Abstractions.Admin;
using Application.Admin.Products.Commands;
using MediatR;

namespace Application.Admin.Products.Handlers
{
    public sealed class UpdateProductHandler(
        IProductAdminRepository productRepo,
        ICategoryAdminRepository categoryRepo,
        IAttributeAdminRepository attributeRepo
    ) : IRequestHandler<UpdateProductCommand>
    {
        public async Task Handle(UpdateProductCommand request, CancellationToken ct)
        {
            // 1. Подтягиваем продукт с его значениями атрибутов
            var product = await productRepo.GetWithAttributesAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Product not found");

            // 2. Обновляем basic-поля
            product.UpdateBasic(
                name: request.Name,
                description: request.Description,
                price: request.Price,
                rrp: request.RecommendedRetailPrice,
                hasStock: request.HasStock
            );

            if (!string.IsNullOrWhiteSpace(request.Article) &&
                request.Article!.Trim() != product.Article)
            {
                product.SetArticle(request.Article);
            }

            // 3. Подтягиваем категорию с привязками атрибутов (по текущему CategoryId продукта)
            var category = await categoryRepo.GetWithAttributesAsync(product.CategoryId, ct)
                ?? throw new InvalidOperationException("Product category not found");

            // 4. Загружаем определения атрибутов категории
            var attachedAttrIds = category.Attributes
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var attrDefs = await attributeRepo.GetByIdsAsync(attachedAttrIds, ct);

            if (attrDefs.Count != attachedAttrIds.Length)
            {
                var missing = attachedAttrIds.Where(id => !attrDefs.ContainsKey(id));
                throw new InvalidOperationException(
                    $"Не найдены определения атрибутов: {string.Join(", ", missing)}");
            }

            // 5. Применяем значения атрибутов
            var values = request.AttributeValues ?? new Dictionary<Guid, string?>();

            product.ApplyAttributeValues(
                category: category,
                values: values,
                attributeDefinitions: attrDefs
            );

            await productRepo.SaveChangesAsync(ct);
        }
    }
}
