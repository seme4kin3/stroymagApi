using Domain.Catalog;

namespace Application.Admin.Attributes
{
    public sealed record AttributeAdminListItemDto(
        Guid Id,
        string Name,
        string Key,
        AttributeDataType DataType,
        bool IsActive
    );
}
