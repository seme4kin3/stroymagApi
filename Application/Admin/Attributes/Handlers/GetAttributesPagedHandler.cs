using Application.Abstractions.Admin;
using Application.Admin.Attributes.Queries;
using Application.Common;
using MediatR;

namespace Application.Admin.Attributes.Handlers
{
    public sealed class GetAttributesPagedHandler
        : IRequestHandler<GetAttributesPagedQuery, PagedResult<AttributeAdminListItemDto>>
    {
        private readonly IAttributeAdminRepository _repo;

        public GetAttributesPagedHandler(IAttributeAdminRepository repo)
            => _repo = repo;

        public async Task<PagedResult<AttributeAdminListItemDto>> Handle(
            GetAttributesPagedQuery request,
            CancellationToken ct)
        {
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;
            var name = request.Name;

            var (items, total) = await _repo.GetPagedAsync(page, pageSize, name, ct);

            var dtoItems = items
                .Select(a => new AttributeAdminListItemDto(
                    a.Id,
                    a.Name,
                    a.Key,
                    a.DataType,
                    a.IsActive))
                .ToList();

            return new PagedResult<AttributeAdminListItemDto>(dtoItems, total, page, pageSize);
        }
    }
}
