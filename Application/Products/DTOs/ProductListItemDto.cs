
namespace Application.Products.DTOs
{
    public sealed record ProductListItemDto(
        Guid Id,
        string Sku,
        string Name,
        string? Brand,    
        PriceDto Price,
        bool InStock
    );
}
