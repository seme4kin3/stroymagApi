using Domain.Catalog;

namespace Application.Admin.Products
{
    public sealed record ProductAttributeValueDto(
        Guid AttributeDefinitionId,
        string Name,
        string Key,
        string? Unit,
        AttributeDataType DataType,
        string? StringValue,
        decimal? NumericValue,
        bool? BoolValue
    );
}
