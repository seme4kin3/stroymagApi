using Domain.Catalog;

namespace Application.Admin.Attributes
{
    public sealed record AttributeDefinitionListItemDto(
        Guid Id,
        string Name,
        string Key,
        AttributeDataType DataType,
        string? Unit,
        bool IsActive
    );
}
