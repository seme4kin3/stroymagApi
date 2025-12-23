using Domain.Catalog;

namespace Application.Admin.Categories.DTOs
{
    public sealed record CategoryAttributeViewDto(
        Guid AttributeDefinitionId,
        string AttributeName,
        string AttributeKey,
        AttributeDataType DataType,
        Guid? UnitId,
        string? UnitName,
        string? UnitSymbol,
        bool IsRequired,
        int SortOrder
    );
}
