using Application.Abstractions.Admin;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Admin
{
    internal sealed class CategoryAdminRepository(StroymagDbContext db) : ICategoryAdminRepository
    {
        public Task AddAsync(Category category, CancellationToken ct) =>
            db.Set<Category>().AddAsync(category, ct).AsTask();

        public Task<Category?> GetWithAttributesAsync(Guid id, CancellationToken ct) =>
            db.Set<Category>()
              .Include(c => c.Attributes)
              .FirstOrDefaultAsync(c => c.Id == id, ct);

        public void Remove(Category category) => db.Set<Category>().Remove(category);

        public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
        public async Task<(IReadOnlyList<Category> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
        {
            var query = db.Set<Category>()
                .AsNoTracking()
                .Include(c => c.Attributes); // ⬅ атрибуты категории

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
