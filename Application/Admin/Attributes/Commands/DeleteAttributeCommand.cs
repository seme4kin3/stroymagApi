using MediatR;

namespace Application.Admin.Attributes.Commands
{
    public sealed record DeleteAttributeCommand(Guid Id) : IRequest<Unit>;
}
