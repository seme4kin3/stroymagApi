using Domain.Catalog;

namespace Application.Admin.Products
{
    public sealed record ProductAttributeValueDto(
        Guid AttributeDefinitionId,
        string AttributeName,
        string AttributeKey,
        AttributeDataType DataType,
        string? StringValue,
        decimal? NumericValue,
        bool? BoolValue
    );
}
