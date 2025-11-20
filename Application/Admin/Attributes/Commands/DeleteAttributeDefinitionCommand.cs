using MediatR;

namespace Application.Admin.Attributes.Commands
{
    public sealed record DeleteAttributeDefinitionCommand(Guid Id) : IRequest;
}
