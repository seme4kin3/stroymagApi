using Application.Abstractions.Admin;
using Application.Admin.Products.Commands;
using Domain.Catalog;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // 1. Подтягиваем категорию с привязанными атрибутами
            var category = await categoryRepo.GetWithAttributesAsync(request.CategoryId, ct)
                ?? throw new KeyNotFoundException("Category not found");

            // 2. Загружаем определения атрибутов, которые привязаны к категории
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

            // 3. Создаём доменную сущность Product
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

            // 4. Применяем значения атрибутов (если они переданы)
            var values = request.AttributeValues ?? new Dictionary<Guid, string?>();

            product.ApplyAttributeValues(
                category: category,
                values: values,
                attributeDefinitions: attrDefs
            );

            // 5. Сохраняем
            await productRepo.AddAsync(product, ct);
            await productRepo.SaveChangesAsync(ct);

            return product.Id;
        }
    }
}
