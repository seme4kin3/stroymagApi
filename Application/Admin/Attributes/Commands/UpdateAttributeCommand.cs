using Domain.Catalog;
using MediatR;


namespace Application.Admin.Attributes.Commands
{
    public sealed record UpdateAttributeCommand(
        Guid Id,
        string Name,
        string Key,
        AttributeDataType DataType,
        bool? IsActive = null
    ) : IRequest<Unit>;
}
