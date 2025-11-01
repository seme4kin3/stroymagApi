using Application.Abstractions;
using Application.Common;
using Application.Products.Queries;
using MediatR;


namespace Application.Products.Handlers
{
    public sealed class SearchLineHandler(IProductReadRepository repo)
        : IRequestHandler<SearchLineQuery, PagedResult<ProductDto>>
    {
        public async Task<PagedResult<ProductDto>> Handle(SearchLineQuery request, CancellationToken ct)
        {
            var (items, total) = await repo.SearchSmartAsync(request.Q.Trim(), request.Page, request.PageSize, ct);
            return new(items.Select(p => p.ToDto()).ToList(), total, request.Page, request.PageSize);
        }
    }
}
