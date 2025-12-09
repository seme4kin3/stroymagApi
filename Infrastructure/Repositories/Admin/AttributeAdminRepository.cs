using Application.Abstractions.Admin;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Admin
{
    internal sealed class AttributeAdminRepository : IAttributeAdminRepository
    {
        private readonly StroymagDbContext _db;

        public AttributeAdminRepository(StroymagDbContext db)
        {
            _db = db;
        }

        public async Task<(IReadOnlyList<AttributeDefinition> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = _db.Set<AttributeDefinition>().AsNoTracking();

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(a => a.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<AttributeDefinition?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _db.Set<AttributeDefinition>()
                .FirstOrDefaultAsync(a => a.Id == id, ct);

        public async Task<Dictionary<Guid, AttributeDefinition>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken ct)
        {
            if (ids.Count == 0)
                return new Dictionary<Guid, AttributeDefinition>();

            return await _db.Set<AttributeDefinition>()
                .Where(a => ids.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, ct);
        }

        public async Task AddAsync(AttributeDefinition attribute, CancellationToken ct)
        {
            await _db.Set<AttributeDefinition>().AddAsync(attribute, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
            => _db.SaveChangesAsync(ct);
    }
}
