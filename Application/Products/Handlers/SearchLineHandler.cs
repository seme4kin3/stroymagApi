using Application.Abstractions;
using Application.Common;
using Application.Products.DTOs;
using Application.Products.Queries;
using MediatR;


namespace Application.Products.Handlers
{
    public sealed class SearchLineHandler(IProductReadRepository repo)
        : IRequestHandler<SearchLineQuery, PagedResult<ProductListItemDto>>
    {
        public async Task<PagedResult<ProductListItemDto>> Handle(SearchLineQuery request, CancellationToken ct)
        {
            var (items, total) = await repo.SearchSmartAsync(
                request.Q.Trim(), request.Page, request.PageSize, ct);

            return new PagedResult<ProductListItemDto>(items, total, request.Page, request.PageSize);
        }
    }
}
