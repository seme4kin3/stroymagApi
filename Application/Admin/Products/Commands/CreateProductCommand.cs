using MediatR;


namespace Application.Admin.Products.Commands
{
    public sealed record CreateProductCommand(
        string Sku,
        string Name,
        Guid BrandId,
        Guid CategoryId,
        Guid UnitId,
        decimal Price,
        string? Description,
        string? Article,
        decimal? RecommendedRetailPrice,
        bool HasStock,
        IReadOnlyDictionary<Guid, string?>? AttributeValues,
        IReadOnlyList<string>? Advantages,
        IReadOnlyList<string>? Complectation
    ) : IRequest<Guid>;
}
