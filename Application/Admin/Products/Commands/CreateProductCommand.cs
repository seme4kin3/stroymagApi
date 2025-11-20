using MediatR;


namespace Application.Admin.Products.Commands
{
    public sealed record CreateProductCommand(
        string Sku,
        string Name,
        Guid BrandId,
        Guid CategoryId,
        decimal Price,
        string? Description = null,
        string? Article = null,
        decimal? RecommendedRetailPrice = null,
        bool HasStock = false,
        IReadOnlyDictionary<Guid, string?>? AttributeValues = null
    ) : IRequest<Guid>;
}
