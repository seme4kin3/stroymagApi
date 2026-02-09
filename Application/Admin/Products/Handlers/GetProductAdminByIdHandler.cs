using Application.Abstractions.Admin;
using Application.Admin.Products.Queries;
using MediatR;


namespace Application.Admin.Products.Handlers
{
    public sealed class GetProductAdminByIdHandler(
        IProductAdminRepository productRepo,
        IAttributeAdminRepository attributeRepo
    ) : IRequestHandler<GetProductAdminByIdQuery, ProductAdminListItemDto?>
    {
        public async Task<ProductAdminListItemDto?> Handle(GetProductAdminByIdQuery request, CancellationToken ct)
        {

            var p = await productRepo.GetDetailsAsync(request.Id, ct);
            if (p is null)
                return null;

            // Подтягиваем определения атрибутов для отображения
            var attrIds = p.Attributes
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var defs = await attributeRepo.GetByIdsAsync(attrIds, ct);

            var attrDtos = p.Attributes
                .Select(a =>
                {
                    defs.TryGetValue(a.AttributeDefinitionId, out var def);

                    return new ProductAttributeValueDto(
                        AttributeDefinitionId: a.AttributeDefinitionId,
                        AttributeName: def?.Name ?? string.Empty,
                        AttributeKey: def?.Key ?? string.Empty,
                        DataType: def?.DataType ?? default,
                        StringValue: a.StringValue,
                        NumericValue: a.NumericValue,
                        BoolValue: a.BoolValue
                    );
                })
                .ToList();

            return new ProductAdminListItemDto(
                Id: p.Id,
                Sku: p.Sku,
                Article: p.Article,
                Name: p.Name,
                BrandId: p.BrandId,
                BrandName: p.Brand?.Name ?? string.Empty,
                CategoryId: p.CategoryId,
                CategoryName: p.Category?.Name ?? string.Empty,
                CategorySlug: p.Category?.Slug,
                UnitId: p.UnitId,
                UnitName: p.Unit?.Name ?? string.Empty,
                UnitSymbol: p.Unit?.Symbol ?? string.Empty,
                Price: p.Price,
                RecommendedRetailPrice: p.RecommendedRetailPrice,
                HasStock: p.HasStock,
                Attributes: attrDtos,
                Advantages: p.Advantages.ToList(),
                Complectation: p.Complectation.ToList()
            );
        }
    }
}
