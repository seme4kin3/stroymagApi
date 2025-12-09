using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using Application.Admin.Categories.Queries;
using Application.Common;
using Domain.Catalog;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class GetCategoriesPagedHandler(
       ICategoryAdminRepository categoryRepo,
       IAttributeAdminRepository attributeRepo,
       IMeasurementUnitAdminRepository unitRepo
   ) : IRequestHandler<GetCategoriesPagedQuery, PagedResult<CategoryAdminDto>>
    {
        public async Task<PagedResult<CategoryAdminDto>> Handle(
            GetCategoriesPagedQuery request,
            CancellationToken ct)
        {
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var (categories, total) = await categoryRepo.GetPagedAsync(page, pageSize, ct);

            // собираем id атрибутов и единиц измерения
            var allAttrIds = categories
                .SelectMany(c => c.CategoryAttributes)
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var allUnitIds = categories
                .SelectMany(c => c.CategoryAttributes)
                .Select(a => a.UnitId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToArray();

            var attrDefs = await attributeRepo.GetByIdsAsync(allAttrIds, ct);
            var units = allUnitIds.Length == 0
                ? new Dictionary<Guid, MeasurementUnit>()
                : await unitRepo.GetByIdsAsync(allUnitIds, ct);

            var dtoItems = categories
                .Select(c =>
                {
                    var attrs = c.CategoryAttributes
                        .OrderBy(a => a.SortOrder)
                        .Select(link =>
                        {
                            attrDefs.TryGetValue(link.AttributeDefinitionId, out var def);

                            MeasurementUnit? unit = null;
                            if (link.UnitId.HasValue)
                                units.TryGetValue(link.UnitId.Value, out unit);

                            return new CategoryAttributeViewDto(
                                AttributeDefinitionId: link.AttributeDefinitionId,
                                AttributeName: def?.Name ?? string.Empty,
                                AttributeKey: def?.Key ?? string.Empty,
                                DataType: def?.DataType ?? default,
                                UnitId: link.UnitId,
                                UnitName: unit?.Name,
                                UnitSymbol: unit?.Symbol,
                                IsRequired: link.IsRequired,
                                SortOrder: link.SortOrder
                            );
                        })
                        .ToList();

                    return new CategoryAdminDto(
                        Id: c.Id,
                        Name: c.Name,
                        Slug: c.Slug,
                        ParentId: c.ParentId,
                        ImageUrl: c.ImageUrl,
                        Attributes: attrs
                    );
                })
                .ToList();

            return new PagedResult<CategoryAdminDto>(dtoItems, total, page, pageSize);
        }
    }
}
