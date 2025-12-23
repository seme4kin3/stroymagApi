namespace Application.Admin.Categories.DTOs
{
    public sealed record CategoryAttributeAdminItemDto(
        Guid AttributeDefinitionId,
        Guid? UnitId,
        bool IsRequired,
        int SortOrder
    );
}
