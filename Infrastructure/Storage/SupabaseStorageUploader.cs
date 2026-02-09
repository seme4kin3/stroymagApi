using Application.Abstractions.Admin;
using Supabase;
using Supabase.Storage;
using FileOptions = Supabase.Storage.FileOptions;

namespace Infrastructure.Storage
{
    public sealed class SupabaseStorageUploader : IStorageUploader
    {
        private readonly Supabase.Client _client;
        public SupabaseStorageUploader(Supabase.Client client)
        {
            _client = client;
        }

        public async Task UploadAsync(
            string bucket,
            string objectKey,
            Stream content,
            string contentType,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(bucket))
                throw new ArgumentException("Bucket is required", nameof(bucket));
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("ObjectKey is required", nameof(objectKey));
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            // Supabase.Upload ожидает byte[]
            using var ms = new MemoryStream();
            await content.CopyToAsync(ms, ct);

            var bytes = ms.ToArray();

            // Вариант “как в примере”, но с опциями (контент-тайп полезно задать)
            await _client.Storage.From(bucket).Upload(
                bytes,
                objectKey,
                new FileOptions
                {
                    ContentType = contentType,
                    Upsert = true
                }
            );
        }

        public async Task DeleteIfExistsAsync(string bucket, string objectKey, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(bucket) || string.IsNullOrWhiteSpace(objectKey))
                return;

            try
            {
                // Remove принимает список путей
                await _client.Storage.From(bucket).Remove(new List<string> { objectKey });
            }
            catch
            {
                // intentionally ignore (можно логировать)
            }
        }

        public string GetPublicUrl(string bucket, string objectKey)
            => _client.Storage.From(bucket).GetPublicUrl(objectKey);
    }
}
