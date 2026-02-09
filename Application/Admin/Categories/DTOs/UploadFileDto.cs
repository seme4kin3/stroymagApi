
namespace Application.Admin.Categories.DTOs
{
    public sealed record UploadFileDto(
        Stream Content,
        string ContentType,
        long ContentLength,
        string? FileName
    );
}
