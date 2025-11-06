using Application.Common;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries
{
    public sealed record GetProductsByCategorySlugQuery(
        string? SlugPath,
        int Page = 1,
        int PageSize = 25
    ) : IRequest<PagedResult<ProductListItemDto>>;
}
