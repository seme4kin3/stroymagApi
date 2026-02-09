using Application.Admin.Categories.DTOs;
using MediatR;

namespace Application.Admin.Categories.Commands
{
    public sealed record CreateCategoryCommand(
        string Name,
        Guid? ParentId,
        string? Slug,
        IReadOnlyList<CategoryAttributeAdminItemDto> Attributes,
        UploadFileDto? Image
    ) : IRequest<Guid>;
}
