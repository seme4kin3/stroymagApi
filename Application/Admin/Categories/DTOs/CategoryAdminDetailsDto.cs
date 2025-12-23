
namespace Application.Admin.Categories.DTOs
{
    public sealed record CategoryAdminDetailsDto(
        Guid Id,
        string Name,
        string? Slug,
        Guid? ParentId,
        string? ImageUrl,
        CategoryParentDto? Parent,
        IReadOnlyList<CategoryAttributeResolvedDto> OwnAttributes,
        IReadOnlyList<CategoryAttributeResolvedDto> InheritedAttributes
    );
}
