
namespace Application.Products
{
    public sealed record ProductPriceDto(string Currency, decimal Amount, decimal? OldAmount);

    public sealed record ProductListItemDto(
        Guid Id,
        string Sku,
        string Name,
        string? Brand,
        ProductPriceDto Price,
        bool InStock
    );

    public sealed record ProductListResultDto(
        IReadOnlyList<ProductListItemDto> Items,
        int Total,
        int Page,
        int PageSize
    );
}
