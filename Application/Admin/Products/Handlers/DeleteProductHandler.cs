using Application.Abstractions.Admin;
using Application.Admin.Products.Commands;
using MediatR;


namespace Application.Admin.Products.Handlers
{
    public sealed class DeleteProductHandler(
        IProductAdminRepository productRepo
    ) : IRequestHandler<DeleteProductCommand>
    {
        public async Task Handle(DeleteProductCommand request, CancellationToken ct)
        {
            var product = await productRepo.GetWithAttributesAsync(request.Id, ct);
            if (product is null)
                return; // идемпотентное удаление

            productRepo.Remove(product);
            await productRepo.SaveChangesAsync(ct);
        }
    }
}
