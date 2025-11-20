using MediatR;

namespace Application.Admin.Brands.Commands
{
    public sealed record CreateBrandCommand(string Name) : IRequest<Guid>;
}
