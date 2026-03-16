using Domain.Catalog;

namespace Application.Abstractions.Admin
{
    public interface IMeasurementUnitAdminRepository
    {
        Task AddAsync(MeasurementUnit unit, CancellationToken ct);

        Task<MeasurementUnit?> GetAsync(Guid id, CancellationToken ct);

        Task<(IReadOnlyList<MeasurementUnit> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            string? name,
            string? symbol,
            CancellationToken ct);

        Task SaveChangesAsync(CancellationToken ct);
        Task<Dictionary<Guid, MeasurementUnit>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken ct);
    }
}
