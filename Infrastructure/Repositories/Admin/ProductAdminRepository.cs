using Application.Abstractions.Admin;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Admin
{
    internal sealed class ProductAdminRepository : IProductAdminRepository
    {
        private readonly StroymagDbContext _db;
        public ProductAdminRepository(StroymagDbContext db)
        {
            _db = db;
        }
        public Task AddAsync(Product product, CancellationToken ct) =>
           _db.Set<Product>().AddAsync(product, ct).AsTask();

        public Task<Product?> GetWithAttributesAsync(Guid id, CancellationToken ct) =>
            _db.Set<Product>()
              .Include(p => p.Attributes)
              .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<(IReadOnlyList<Product> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = _db.Set<Product>()
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.Attributes);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public void Remove(Product product) =>
            _db.Set<Product>().Remove(product);

        public Task<int> SaveChangesAsync(CancellationToken ct) =>
            _db.SaveChangesAsync(ct);
    }
}
