
namespace Application.Abstractions.Admin
{
    public interface IStorageUploader
    {
        Task UploadAsync(string bucket, string objectKey, Stream content, string contentType, CancellationToken ct);
        Task DeleteIfExistsAsync(string bucket, string objectKey, CancellationToken ct);
        string GetPublicUrl(string bucket, string objectKey);
    }
}
