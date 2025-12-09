
namespace Application.Admin.Categories
{
    public sealed record CategoryAdminDto(
        Guid Id,
        string Name,
        string? Slug,
        Guid? ParentId,
        string? ImageUrl,
        IReadOnlyList<CategoryAttributeViewDto> Attributes
    );
}
