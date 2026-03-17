using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using Application.Common.Exceptions;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class UploadCategoryImageHandler(
        ICategoryAdminRepository categoryRepo,
        IStorageUploader storageUploader
    ) : IRequestHandler<UploadCategoryImageCommand>
    {
        public async Task Handle(UploadCategoryImageCommand request, CancellationToken ct)
        {
            var category = await categoryRepo.GetByIdAsync(request.CategoryId, ct)
                ?? throw new NotFoundException("Категория не найдена.");

            ValidateImage(request);

            // сохраняем старую картинку для последующего удаления
            var oldBucket = category.ImageBucket;
            var oldKey = category.ImageObjectKey;

            string? newBucket = null;
            string? newKey = null;

            try
            {
                var ext = request.ContentType switch
                {
                    "image/jpeg" => "jpg",
                    "image/png" => "png",
                    "image/webp" => "webp",
                    _ => throw new DomainException("Разрешены только jpg/png/webp.")
                };

                newBucket = "category";
                newKey = $"categories/{category.Id}.{ext}";

                // удаляем старую картинку ТОЛЬКО после успешного сохранения
                if (!string.IsNullOrWhiteSpace(oldBucket) && !string.IsNullOrWhiteSpace(oldKey))
                {
                    await storageUploader.DeleteIfExistsAsync(oldBucket!, oldKey!, ct);
                }

                await storageUploader.UploadAsync(
                    bucket: newBucket,
                    objectKey: newKey,
                    content: request.Content,
                    contentType: request.ContentType,
                    ct: ct
                );

                category.SetImage(newBucket, newKey);

                await categoryRepo.SaveChangesAsync(ct);
            }
            catch
            {
                // компенсация: если новую картинку успели загрузить, но дальше упали
                if (!string.IsNullOrWhiteSpace(newBucket) && !string.IsNullOrWhiteSpace(newKey))
                {
                    await storageUploader.DeleteIfExistsAsync(newBucket!, newKey!, ct);
                }

                throw;
            }
        }

        private static void ValidateImage(UploadCategoryImageCommand request)
        {
            if (request.ContentLength <= 0 || request.ContentLength > 5 * 1024 * 1024)
                throw new DomainException("Файл должен быть до 5MB.");

            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(request.ContentType))
                throw new DomainException("Разрешены только jpg/png/webp.");
        }
    }
}
