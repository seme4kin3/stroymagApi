using Application.Abstractions.Admin;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Admin
{
    internal sealed class CategoryAdminRepository : ICategoryAdminRepository
    {
        private readonly StroymagDbContext _db;

        public CategoryAdminRepository(StroymagDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(Category category, CancellationToken ct) =>
            _db.Set<Category>().AddAsync(category, ct).AsTask();

        public Task<Category?> GetWithAttributesAsync(Guid id, CancellationToken ct) =>
            _db.Set<Category>()
              .Include(c => c.CategoryAttributes)
              .FirstOrDefaultAsync(c => c.Id == id, ct);

        public void Remove(Category category) => _db.Set<Category>().Remove(category);

        public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
        public async Task<(IReadOnlyList<Category> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
        {
            var query = _db.Set<Category>()
                .AsNoTracking()
                .Include(c => c.CategoryAttributes); // ⬅ атрибуты категории

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<IReadOnlyList<Category>> GetFlatWithAttributesAsync(CancellationToken ct)
        {
            return await _db.Set<Category>()
                .AsNoTracking()
                .Include(c => c.CategoryAttributes)
                .ToListAsync(ct);
        }

        public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct) =>
             _db.Set<Category>()
              .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}
