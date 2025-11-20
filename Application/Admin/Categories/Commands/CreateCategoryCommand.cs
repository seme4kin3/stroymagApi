using MediatR;

namespace Application.Admin.Categories.Commands
{
    public sealed record CreateCategoryCommand(
        string Name,
        Guid? ParentId = null,
        string? Slug = null,
        string? ImageUrl = null
    ) : IRequest<Guid>;

}
