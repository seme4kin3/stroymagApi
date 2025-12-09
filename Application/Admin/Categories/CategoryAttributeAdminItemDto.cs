namespace Application.Admin.Categories
{
    public sealed record CategoryAttributeAdminItemDto(
        Guid AttributeDefinitionId,
        Guid? UnitId,
        bool IsRequired,
        int SortOrder
    );
}
