using Application.Abstractions.Admin;
using Application.Admin.Attributes.Queries;
using Application.Common;
using MediatR;

namespace Application.Admin.Attributes.Handlers
{
    public sealed class GetAttributeDefinitionsPagedHandler(
        IAttributeAdminRepository repo
    ) : IRequestHandler<GetAttributeDefinitionsPagedQuery, PagedResult<AttributeDefinitionListItemDto>>
    {
        public async Task<PagedResult<AttributeDefinitionListItemDto>> Handle(
            GetAttributeDefinitionsPagedQuery request,
            CancellationToken ct)
        {
            var page = request.Page;
            var pageSize = request.PageSize;

            var (items, total) = await repo.GetPagedAsync(page, pageSize, ct);

            var dtoItems = items
                .Select(a => new AttributeDefinitionListItemDto(
                    a.Id,
                    a.Name,
                    a.Key,
                    a.DataType,
                    a.Unit,
                    a.IsActive
                ))
                .ToList();

            return new PagedResult<AttributeDefinitionListItemDto>(dtoItems, total, page, pageSize);
        }
    }
}
