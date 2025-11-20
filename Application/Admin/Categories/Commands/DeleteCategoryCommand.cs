using MediatR;


namespace Application.Admin.Categories.Commands
{
    public sealed record DeleteCategoryCommand(Guid Id) : IRequest;
}
