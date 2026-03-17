using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using Application.Common.Exceptions;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class DeleteCategoryHandler(
        ICategoryAdminRepository categoryRepo,
        IStorageUploader storageUploader
    ) : IRequestHandler<DeleteCategoryCommand>
    {
        public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var category = await categoryRepo.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException("Категория не найдена.");

            var bucket = category.ImageBucket;
            var key = category.ImageObjectKey;

            // 1) Удаляем из БД
            categoryRepo.Remove(category);
            await categoryRepo.SaveChangesAsync(ct);

            // 2) Удаляем файл (после БД — чтобы не упасть на сторедже и не оставить категорию)
            if (!string.IsNullOrWhiteSpace(bucket) && !string.IsNullOrWhiteSpace(key))
            {
                await storageUploader.DeleteIfExistsAsync(bucket!, key!, ct);
            }
        }
    }
}
