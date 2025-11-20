using Application.Abstractions.Admin;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Admin
{
    internal sealed class BrandAdminRepository(StroymagDbContext db) : IBrandAdminRepository
    {
        public Task AddAsync(Brand brand, CancellationToken ct) =>
            db.Set<Brand>().AddAsync(brand, ct).AsTask();

        public Task<Brand?> GetAsync(Guid id, CancellationToken ct) =>
            db.Set<Brand>()
              .FirstOrDefaultAsync(b => b.Id == id, ct);

        public async Task<(IReadOnlyList<Brand> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = db.Set<Brand>().AsNoTracking();

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public void Remove(Brand brand) => db.Set<Brand>().Remove(brand);

        public Task<int> SaveChangesAsync(CancellationToken ct) =>
            db.SaveChangesAsync(ct);
    }
}
