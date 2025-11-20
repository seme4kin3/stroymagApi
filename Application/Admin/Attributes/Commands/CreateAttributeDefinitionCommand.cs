using Domain.Catalog;
using MediatR;


namespace Application.Admin.Attributes.Commands
{
    public sealed record CreateAttributeDefinitionCommand(
        string Name,
        string Key,
        AttributeDataType DataType,
        string? Unit
    ) : IRequest<Guid>;
}
