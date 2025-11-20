
namespace Application.Admin.Categories
{
    public sealed record CategoryAttributeItemDto(
        Guid AttributeDefinitionId,
        string Name,
        string Key,
        string? Unit,
        bool IsActive,
        bool IsRequired,
        int SortOrder
    );
}
