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
        IReadOnlyDictionary<Guid, string?>? AttributeValues
    ) : IRequest;
}
