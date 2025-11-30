
namespace Application.Admin.Products
{
    public sealed record ProductAdminListItemDto(
        Guid Id,
        string Sku,
        string Article,
        string Name,
        Guid BrandId,
        string BrandName,
        Guid CategoryId,
        string CategoryName,
        string? CategorySlug,
        decimal Price,
        decimal? RecommendedRetailPrice,
        bool HasStock,
        IReadOnlyList<ProductAttributeValueDto> Attributes,
        IReadOnlyList<string> Advantages,
        IReadOnlyList<string> Complectation
    );
}
