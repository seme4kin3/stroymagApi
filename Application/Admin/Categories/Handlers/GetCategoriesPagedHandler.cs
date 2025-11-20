using Application.Abstractions.Admin;
using Application.Admin.Categories.Queries;
using Application.Common;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class GetCategoriesPagedHandler(
        ICategoryAdminRepository categoryRepo,
        IAttributeAdminRepository attributeRepo
    ) : IRequestHandler<GetCategoriesPagedQuery, PagedResult<CategoryListItemDto>>
    {
        public async Task<PagedResult<CategoryListItemDto>> Handle(
            GetCategoriesPagedQuery request,
            CancellationToken ct)
        {
            var page = request.Page;
            var pageSize = request.PageSize;

            var (categories, total) = await categoryRepo.GetPagedAsync(page, pageSize, ct);

            // все AttributeDefinitionId, которые используются в вытащенных категориях
            var allAttrIds = categories
                .SelectMany(c => c.Attributes)
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var defs = await attributeRepo.GetByIdsAsync(allAttrIds, ct);

            var dtoItems = categories.Select(c =>
            {
                var attrDtos = c.Attributes
                    .OrderBy(a => a.SortOrder)
                    .Select(a =>
                    {
                        defs.TryGetValue(a.AttributeDefinitionId, out var def);

                        return new CategoryAttributeItemDto(
                            AttributeDefinitionId: a.AttributeDefinitionId,
                            Name: def?.Name ?? string.Empty,
                            Key: def?.Key ?? string.Empty,
                            Unit: def?.Unit,
                            IsActive: def?.IsActive ?? false,
                            IsRequired: a.IsRequired,
                            SortOrder: a.SortOrder
                        );
                    })
                    .ToList();

                return new CategoryListItemDto(
                    Id: c.Id,
                    Name: c.Name,
                    Slug: c.Slug,
                    ParentId: c.ParentId,
                    ImageUrl: c.ImageUrl,
                    Attributes: attrDtos
                );
            }).ToList();

            return new PagedResult<CategoryListItemDto>(dtoItems, total, page, pageSize);
        }
    }
}
