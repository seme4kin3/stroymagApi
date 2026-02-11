using Application.Abstractions.Admin;
using Application.Admin.Products.Commands;
using MediatR;

namespace Application.Admin.Products.Handlers
{
    public sealed class UploadProductImagesHandler(
         IProductAdminRepository productRepo,
         IStorageUploader storageUploader
     ) : IRequestHandler<UploadProductImagesCommand>
    {
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;
        private static readonly IReadOnlyDictionary<string, string> ContentTypeToExtension =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["image/jpeg"] = "jpg",
                ["image/png"] = "png",
                ["image/webp"] = "webp"
            };

        public async Task Handle(UploadProductImagesCommand request, CancellationToken ct)
        {
            if (request.Files is null || request.Files.Count == 0)
                throw new InvalidOperationException("Не переданы файлы изображений");

            var product = await productRepo.GetWithAttributesAsync(request.ProductId, ct)
                ?? throw new KeyNotFoundException("Товар не найден");

            var uploadedItems = new List<(string Bucket, string Key)>();
            var oldItemsToDelete = new List<(string Bucket, string Key)>();

            try
            {
                if (request.ReplaceExisting)
                {
                    foreach (var image in product.Images)
                    {
                        if (!TryParseStoragePath(image.StoragePath, out var bucket, out var objectKey))
                            continue;

                        oldItemsToDelete.Add((bucket, objectKey));
                    }

                    product.Images.Clear();
                }

                var nextSortOrder = product.Images.Count == 0
                    ? 0
                    : product.Images.Max(i => i.SortOrder) + 1;

                foreach (var file in request.Files)
                {
                    var ext = ValidateAndResolveExtension(file.ContentType, file.ContentLength);
                    var bucket = "product";
                    var imageId = Guid.NewGuid();
                    var objectKey = $"products/{product.Id}/{imageId:N}.{ext}";

                    await storageUploader.UploadAsync(
                        bucket: bucket,
                        objectKey: objectKey,
                        content: file.Content,
                        contentType: file.ContentType,
                        ct: ct);

                    uploadedItems.Add((bucket, objectKey));

                    var shouldBePrimary = (file.Main ?? false) || !product.Images.Any(i => i.IsPrimary);

                    product.AddImage(
                        imageId: imageId,
                        url: storageUploader.GetPublicUrl(bucket, objectKey),
                        storagePath: $"{bucket}/{objectKey}",
                        alt: null,
                        isPrimary: shouldBePrimary,
                        sortOrder: nextSortOrder
                    );
                    nextSortOrder++;
                }

                await productRepo.SaveChangesAsync(ct);

                foreach (var oldItem in oldItemsToDelete)
                {
                    await storageUploader.DeleteIfExistsAsync(oldItem.Bucket, oldItem.Key, ct);
                }
            }
            catch
            {
                foreach (var uploadedItem in uploadedItems)
                {
                    await storageUploader.DeleteIfExistsAsync(uploadedItem.Bucket, uploadedItem.Key, ct);
                }

                throw;
            }
        }

        private static string ValidateAndResolveExtension(string contentType, long contentLength)
        {
            if (contentLength <= 0 || contentLength > MaxFileSizeBytes)
                throw new InvalidOperationException("Каждый файл должен быть до 5MB");

            if (!ContentTypeToExtension.TryGetValue(contentType, out var extension))
                throw new InvalidOperationException("Разрешены только jpg/png/webp");

            return extension;
        }

        private static bool TryParseStoragePath(string? storagePath, out string bucket, out string objectKey)
        {
            bucket = string.Empty;
            objectKey = string.Empty;

            if (string.IsNullOrWhiteSpace(storagePath))
                return false;

            var separatorIndex = storagePath.IndexOf('/');
            if (separatorIndex <= 0 || separatorIndex >= storagePath.Length - 1)
                return false;

            bucket = storagePath[..separatorIndex];
            objectKey = storagePath[(separatorIndex + 1)..];
            return true;
        }
    }
}
