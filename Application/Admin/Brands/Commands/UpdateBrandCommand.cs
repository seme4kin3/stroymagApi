using MediatR;


namespace Application.Admin.Brands.Commands
{
    public sealed record UpdateBrandCommand(Guid Id, string Name) : IRequest;
}
