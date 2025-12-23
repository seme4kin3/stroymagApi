namespace Application.Admin.Categories.DTOs
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
