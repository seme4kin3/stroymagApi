namespace Application.Products
{
    public sealed record ProductDto(
        Guid Id,
        string Sku,          // Штрихкод
        string Name,         // Наименование
        string? Article,     // Артикул
        decimal Price
    );
}
