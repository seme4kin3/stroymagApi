using Application.Common;
using MediatR;


namespace Application.Admin.Brands.Queries
{
    public sealed record GetBrandsPagedQuery(
        int Page = 1,
        int PageSize = 50,
        string? Name = null)
        : IRequest<PagedResult<BrandListItemDto>>;
}
