using Application.Abstractions.Admin;
using Application.Admin.Brands.Commands;
using MediatR;


namespace Application.Admin.Brands.Handlers
{
    public sealed class UpdateBrandHandler(IBrandAdminRepository repo)
        : IRequestHandler<UpdateBrandCommand>
    {
        public async Task Handle(UpdateBrandCommand request, CancellationToken ct)
        {
            var brand = await repo.GetAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Brand not found");

            // доменный метод (см. пункт 1)
            brand.Rename(request.Name);

            await repo.SaveChangesAsync(ct);
        }
    }
}
