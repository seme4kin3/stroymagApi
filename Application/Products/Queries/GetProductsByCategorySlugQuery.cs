using MediatR;

namespace Application.Products.Queries
{
    public sealed record GetProductsByCategorySlugQuery(
        string? SlugPath,
        int Page = 1,
        int PageSize = 24
    ) : IRequest<ProductListResultDto>;
}
