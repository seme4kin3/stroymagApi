using Application.Common;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries
{
    /// <summary>
    /// Единая поисковая строка. Ищет по SKU, Barcode и Name с ранжированием.
    /// </summary>
    public sealed record SearchLineQuery(
        string Q,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<PagedResult<ProductListItemDto>>;
}
