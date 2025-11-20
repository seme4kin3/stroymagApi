using MediatR;

namespace Application.Admin.Products.Commands
{
    public sealed record DeleteProductCommand(Guid Id) : IRequest;
}
