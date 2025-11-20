using Application.Abstractions.Admin;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Admin
{
    internal sealed class AttributeAdminRepository(StroymagDbContext db) : IAttributeAdminRepository
    {
        public Task AddAsync(AttributeDefinition def, CancellationToken ct) =>
            db.Set<AttributeDefinition>().AddAsync(def, ct).AsTask();

        public Task<AttributeDefinition?> GetAsync(Guid id, CancellationToken ct) =>
            db.Set<AttributeDefinition>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<Dictionary<Guid, AttributeDefinition>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
        {
            var list = await db.Set<AttributeDefinition>()
                .Where(x => ids.Contains(x.Id) && x.IsActive)
                .ToListAsync(ct);
            return list.ToDictionary(x => x.Id, x => x);
        }

        public async Task<(IReadOnlyList<AttributeDefinition> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
        {
            var query = db.Set<AttributeDefinition>().AsNoTracking();

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(a => a.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    }
}
