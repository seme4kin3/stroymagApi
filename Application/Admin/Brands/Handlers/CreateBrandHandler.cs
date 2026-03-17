using Application.Abstractions.Admin;
using Application.Admin.Brands.Commands;
using Domain.Catalog;
using MediatR;


namespace Application.Admin.Brands.Handlers
{
    public sealed class CreateBrandHandler(IBrandAdminRepository repo)
        : IRequestHandler<CreateBrandCommand, Guid>
    {
        public async Task<Guid> Handle(CreateBrandCommand request, CancellationToken ct)
        {
            var brand = new Brand(request.Name);
            await repo.AddAsync(brand, ct);
            await repo.SaveChangesAsync(ct);
            return brand.Id;
        }
    }
}
