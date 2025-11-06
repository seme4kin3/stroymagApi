
namespace Application.Products.DTOs
{
    public sealed record ProductDetailsDto(
        Guid Id,
        string Sku,
        string Article,
        string Name,
        string? Description,
        string? Brand,       
        Guid CategoryId,
        string CategoryName,
        string CategorySlug,
        PriceDto Price,
        bool InStock,
        IReadOnlyList<string> Images,                 
        IReadOnlyDictionary<string, string> Attributes
    );
}
