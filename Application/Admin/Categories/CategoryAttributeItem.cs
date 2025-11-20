
namespace Application.Admin.Categories
{
    public sealed record CategoryAttributeItem(
        Guid AttributeDefinitionId,
        bool IsRequired,
        int SortOrder
    );
}
