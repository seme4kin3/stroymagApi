using Application.Abstractions.Admin;
using Application.Admin.Products.Commands;
using MediatR;

namespace Application.Admin.Products.Handlers
{
    public sealed class DeleteProductHandler(
       IProductAdminRepository productRepo,
       IStorageUploader storageUploader
   ) : IRequestHandler<DeleteProductCommand>
    {
        public async Task Handle(DeleteProductCommand request, CancellationToken ct)
        {
            var product = await productRepo.GetWithAttributesAsync(request.Id, ct);
            if (product is null)
                return;

            var imageStoragePaths = product.Images
                .Select(i => i.StoragePath)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            productRepo.Remove(product);
            await productRepo.SaveChangesAsync(ct);

            foreach (var storagePath in imageStoragePaths)
            {
                if (!TryParseStoragePath(storagePath!, out var bucket, out var objectKey))
                    continue;

                await storageUploader.DeleteIfExistsAsync(bucket, objectKey, ct);
            }
        }

        private static bool TryParseStoragePath(string storagePath, out string bucket, out string objectKey)
        {
            bucket = string.Empty;
            objectKey = string.Empty;

            var separatorIndex = storagePath.IndexOf('/');
            if (separatorIndex <= 0 || separatorIndex >= storagePath.Length - 1)
                return false;

            bucket = storagePath[..separatorIndex];
            objectKey = storagePath[(separatorIndex + 1)..];
            return true;
        }
    }
}
