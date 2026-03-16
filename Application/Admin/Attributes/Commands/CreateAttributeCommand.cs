using Domain.Catalog;
using MediatR;


namespace Application.Admin.Attributes.Commands
{
    public sealed record CreateAttributeCommand(
        string Name,
        //string Key,
        AttributeDataType DataType,
        string? Unit
    ) : IRequest<Guid>;
}
