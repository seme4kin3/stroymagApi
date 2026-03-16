using Application.Abstractions.Admin;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.Admin
{
    internal sealed class MeasurementUnitAdminRepository : IMeasurementUnitAdminRepository
    {
        private readonly StroymagDbContext _db;
        public MeasurementUnitAdminRepository(StroymagDbContext db)
        {
            _db = db;
        }
        public Task AddAsync(MeasurementUnit unit, CancellationToken ct) =>
            _db.Set<MeasurementUnit>().AddAsync(unit, ct).AsTask();

        public Task<MeasurementUnit?> GetAsync(Guid id, CancellationToken ct) =>
            _db.Set<MeasurementUnit>().FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<(IReadOnlyList<MeasurementUnit> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            string? name,
            string? symbol,
            CancellationToken ct)
        {
            var query = _db.Set<MeasurementUnit>().AsNoTracking();
            var nameLike = name?.Trim();
            var symbolLike = symbol?.Trim();

            if (!string.IsNullOrWhiteSpace(nameLike))
            {
                query = query.Where(u => EF.Functions.ILike(u.Name, $"%{nameLike}%"));
            }

            if (!string.IsNullOrWhiteSpace(symbolLike))
            {
                query = query.Where(u => EF.Functions.ILike(u.Symbol, $"%{symbolLike}%"));
            }

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(u => u.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public Task SaveChangesAsync(CancellationToken ct) =>
            _db.SaveChangesAsync(ct);

        public async Task<Dictionary<Guid, MeasurementUnit>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken ct)
        {
            if (ids.Count == 0)
                return new Dictionary<Guid, MeasurementUnit>();

            return await _db.Set<MeasurementUnit>()
                .Where(u => ids.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, ct);
        }
    }
}
