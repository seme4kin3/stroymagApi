using Application.Abstractions.Admin;
using Application.Admin.Brands.Commands;
using MediatR;


namespace Application.Admin.Brands.Handlers
{
    public sealed class DeleteBrandHandler(IBrandAdminRepository repo)
        : IRequestHandler<DeleteBrandCommand>
    {
        public async Task Handle(DeleteBrandCommand request, CancellationToken ct)
        {
            var brand = await repo.GetAsync(request.Id, ct);
            if (brand is null)
                return; 

            repo.Remove(brand);
            await repo.SaveChangesAsync(ct);
        }
    }
}
