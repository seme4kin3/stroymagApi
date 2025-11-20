using Domain.Catalog;


namespace Application.Abstractions.Admin
{
    public interface IAttributeAdminRepository
    {
        Task AddAsync(AttributeDefinition def, CancellationToken ct);
        Task<AttributeDefinition?> GetAsync(Guid id, CancellationToken ct);
        Task<Dictionary<Guid, AttributeDefinition>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
        Task<(IReadOnlyList<AttributeDefinition> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
