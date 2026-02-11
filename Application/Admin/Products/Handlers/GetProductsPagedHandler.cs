using Application.Abstractions.Admin;
using Application.Admin.Products.Queries;
using Application.Common;
using MediatR;


namespace Application.Admin.Products.Handlers
{
    public sealed class GetProductsPagedHandler(
       IProductAdminRepository productRepo,
       IAttributeAdminRepository attributeRepo
   ) : IRequestHandler<GetProductsPagedQuery, PagedResult<ProductAdminListItemDto>>
    {
        public async Task<PagedResult<ProductAdminListItemDto>> Handle(
            GetProductsPagedQuery request,
            CancellationToken ct)
        {
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var (products, total) = await productRepo.GetPagedAsync(page, pageSize, ct);

            // все AttributeDefinitionId, используемые в продуктах
            var allAttrIds = products
                .SelectMany(p => p.Attributes)
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var defs = await attributeRepo.GetByIdsAsync(allAttrIds, ct);

            var dtoItems = products.Select(p =>
            {
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

                var imageDtos = p.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new ProductAdminImageDto(
                        Url: i.Url,
                        IsPrimary: i.IsPrimary
                    ))
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
                    Complectation: p.Complectation.ToList(),
                    Images: imageDtos
                );
            }).ToList();

            return new PagedResult<ProductAdminListItemDto>(dtoItems, total, page, pageSize);
        }
    }
}
