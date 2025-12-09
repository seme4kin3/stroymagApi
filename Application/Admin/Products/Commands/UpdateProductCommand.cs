using MediatR;

namespace Application.Admin.Products.Commands
{
    public sealed record UpdateProductCommand(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        decimal? RecommendedRetailPrice,
        bool HasStock,
        string? Article,
        Guid UnitId,
        IReadOnlyDictionary<Guid, string?>? AttributeValues,
        IReadOnlyList<string>? Advantages,
        IReadOnlyList<string>? Complectation
    ) : IRequest;
}
