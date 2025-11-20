using MediatR;

namespace Application.Admin.Categories.Commands
{
    public sealed record CreateCategoryWithAttributesCommand(
        string Name,
        Guid? ParentId,
        string? Slug,
        string? ImageUrl,
        IReadOnlyList<CategoryAttributeItem> Attributes
    ) : IRequest<Guid>;
}
