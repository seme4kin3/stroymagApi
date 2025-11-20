using MediatR;


namespace Application.Admin.Brands.Commands
{
    public sealed record DeleteBrandCommand(Guid Id) : IRequest;
}
