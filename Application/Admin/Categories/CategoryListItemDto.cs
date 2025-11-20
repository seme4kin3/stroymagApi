
namespace Application.Admin.Categories
{
    public sealed record CategoryListItemDto(
        Guid Id,
        string Name,
        string? Slug,
        Guid? ParentId,
        string? ImageUrl,
        IReadOnlyList<CategoryAttributeItemDto> Attributes
    );
}
