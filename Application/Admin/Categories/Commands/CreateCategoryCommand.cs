using Application.Admin.Categories.DTOs;
using MediatR;

namespace Application.Admin.Categories.Commands
{
    public sealed record CreateCategoryCommand(
        string Name,
        Guid? ParentId,
        string? Slug,
        string? ImageUrl,
        IReadOnlyList<CategoryAttributeAdminItemDto> Attributes
    ) : IRequest<Guid>;
}
