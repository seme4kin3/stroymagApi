using Domain.Catalog;

namespace Application.Abstractions.Admin
{

    public interface IAttributeAdminRepository
    {
        Task<(IReadOnlyList<AttributeDefinition> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            string? name,
            CancellationToken ct);

        Task<AttributeDefinition?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<Dictionary<Guid, AttributeDefinition>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken ct);

        Task AddAsync(AttributeDefinition attribute, CancellationToken ct);

        Task SaveChangesAsync(CancellationToken ct);
    }
}
