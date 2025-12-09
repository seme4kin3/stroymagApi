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
                return;

            // по желанию: проверки на наличие потомков / товаров

            categoryRepo.Remove(category);
            await categoryRepo.SaveChangesAsync(ct);
        }
    }
}
