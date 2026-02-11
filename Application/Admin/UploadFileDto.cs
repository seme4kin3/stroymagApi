namespace Application.Admin
{
    public sealed record UploadFileDto(
        Stream Content,
        string ContentType,
        long ContentLength,
        string? FileName,
        bool? Main
    );
}
