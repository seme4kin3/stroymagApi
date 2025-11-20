using Domain.Catalog;
using MediatR;


namespace Application.Admin.Attributes.Commands
{
    public sealed record UpdateAttributeDefinitionCommand(
        Guid Id,
        string Name,
        string Key,
        AttributeDataType DataType,
        string? Unit,
        bool IsActive                // хотим ли держать атрибут активным
    ) : IRequest;
}
