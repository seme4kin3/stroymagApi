using Application.Common;
using MediatR;


namespace Application.Admin.Attributes.Queries
{
    public sealed record GetAttributesPagedQuery(
        int Page = 1,
        int PageSize = 50,
        string? Name = null
    ) : IRequest<PagedResult<AttributeAdminListItemDto>>;
}
