using Application.Common;
using MediatR;


namespace Application.Admin.Products.Queries
{
    public sealed record GetProductsPagedQuery(int Page = 1, int PageSize = 50)
        : IRequest<PagedResult<ProductAdminListItemDto>>;
}
