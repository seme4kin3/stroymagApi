using MediatR;

namespace Application.Admin.Categories.Commands
{
    public sealed record UpdateCategoryCommand(
        Guid Id,
        string Name,
        Guid? ParentId,
        string? Slug,
        string? ImageUrl,
        IReadOnlyList<CategoryAttributeItem> Attributes
    ) : IRequest;
}
