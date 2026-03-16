using Application.Abstractions.Admin;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Admin
{
    internal sealed class BrandAdminRepository : IBrandAdminRepository
    {
        private readonly StroymagDbContext _db;
        public BrandAdminRepository(StroymagDbContext db)
        {
            _db = db;
        }
        public Task AddAsync(Brand brand, CancellationToken ct) =>
            _db.Set<Brand>().AddAsync(brand, ct).AsTask();

        public Task<Brand?> GetAsync(Guid id, CancellationToken ct) =>
            _db.Set<Brand>()
              .FirstOrDefaultAsync(b => b.Id == id, ct);

        public async Task<(IReadOnlyList<Brand> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            string? name,
            CancellationToken ct)
        {
            var query = _db.Set<Brand>().AsNoTracking();
            var nameLike = name?.Trim();

            if (!string.IsNullOrWhiteSpace(nameLike))
            {
                query = query.Where(b => EF.Functions.ILike(b.Name, $"%{nameLike}%"));
            }

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public void Remove(Brand brand) => _db.Set<Brand>().Remove(brand);

        public Task<int> SaveChangesAsync(CancellationToken ct) =>
            _db.SaveChangesAsync(ct);
    }
}
