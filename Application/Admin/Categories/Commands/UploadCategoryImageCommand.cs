using MediatR;

namespace Application.Admin.Categories.Commands
{
    public sealed record UploadCategoryImageCommand(
        Guid CategoryId,
        Stream Content,
        string ContentType,
        long ContentLength,
        string? FileName
    ) : IRequest;
}
