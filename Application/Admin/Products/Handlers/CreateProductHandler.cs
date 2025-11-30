using Application.Abstractions.Admin;
using Application.Admin.Products.Commands;
using Domain.Catalog;
using MediatR;

namespace Application.Admin.Products.Handlers
{
    public sealed class CreateProductHandler(
       IProductAdminRepository productRepo,
       ICategoryAdminRepository categoryRepo,
       IAttributeAdminRepository attributeRepo
   ) : IRequestHandler<CreateProductCommand, Guid>
    {
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
        {
            // 1. Категория с привязанными атрибутами
            var category = await categoryRepo.GetWithAttributesAsync(request.CategoryId, ct)
                ?? throw new KeyNotFoundException("Category not found");

            // 2. Определения атрибутов
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

            // 3. Создаём доменный Product
            var product = new Product(
                sku: request.Sku,
                name: request.Name,
                brandId: request.BrandId,
                categoryId: request.CategoryId,
                price: request.Price,
                description: request.Description,
                article: request.Article,
                recommendedRetailPrice: request.RecommendedRetailPrice,
                hasStock: request.HasStock
            );

            // ➕ новые поля в домене
            product.SetAdvantages(request.Advantages ?? Array.Empty<string>());
            product.SetComplectation(request.Complectation ?? Array.Empty<string>());

            // 4. Атрибуты товара
            var values = request.AttributeValues ?? new Dictionary<Guid, string?>();

            product.ApplyAttributeValues(
                category: category,
                values: values,
                attributeDefinitions: attrDefs
            );

            // 5. Сохранение
            await productRepo.AddAsync(product, ct);
            await productRepo.SaveChangesAsync(ct);

            return product.Id;
        }
    }
}
