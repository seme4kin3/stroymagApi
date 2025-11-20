using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class DeleteCategoryHandler(ICategoryAdminRepository categoryRepo)
        : IRequestHandler<DeleteCategoryCommand>
    {
        public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var category = await categoryRepo.GetWithAttributesAsync(request.Id, ct);
            if (category is null)
                return; // идемпотентность

            // при желании здесь можно проверить:
            // - наличие дочерних категорий
            // - наличие товаров в категории
            // и кидать InvalidOperationException, если нельзя удалить.

            categoryRepo.Remove(category);
            await categoryRepo.SaveChangesAsync(ct);
        }
    }
}
