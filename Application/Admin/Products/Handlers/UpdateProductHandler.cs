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
            // 1. подтягиваем продукт с атрибутами
            var product = await productRepo.GetWithAttributesAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Product not found");

            // 2. обновляем basic + UnitId
            product.Update(
                sku: request.Sku,
                brandId: request.BrandId,
                categoryId: request.CategoryId,
                name: request.Name,
                description: request.Description,
                price: request.Price,
                recommendedRetailPrice: request.RecommendedRetailPrice,
                hasStock: request.HasStock,
                unitId: request.UnitId
            );

            if (!string.IsNullOrWhiteSpace(request.Article) &&
                request.Article!.Trim() != product.Article)
            {
                product.SetArticle(request.Article);
            }

            product.SetAdvantages(request.Advantages ?? Array.Empty<string>());
            product.SetComplectation(request.Complectation ?? Array.Empty<string>());

            // 3. категория и её атрибуты
            var category = await categoryRepo.GetWithAttributesAsync(product.CategoryId, ct)
                ?? throw new InvalidOperationException("Product category not found");

            var attachedAttrIds = category.CategoryAttributes
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

            // 4. применяем значения атрибутов
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
