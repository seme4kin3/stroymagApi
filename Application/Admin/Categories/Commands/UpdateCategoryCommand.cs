using Application.Admin.Categories.DTOs;
using MediatR;

namespace Application.Admin.Categories.Commands
{
    public sealed record UpdateCategoryCommand(
        Guid Id,
        string Name,
        Guid? ParentId,
        string? Slug,
        IReadOnlyList<CategoryAttributeAdminItemDto> Attributes,
        UploadFileDto? Image,      
        bool RemoveImage = false   
    ) : IRequest;
}
