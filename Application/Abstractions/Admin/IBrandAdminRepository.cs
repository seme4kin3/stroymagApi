using Domain.Catalog;


namespace Application.Abstractions.Admin
{
    public interface IBrandAdminRepository
    {
        Task AddAsync(Brand brand, CancellationToken ct);
        Task<Brand?> GetAsync(Guid id, CancellationToken ct);

        Task<(IReadOnlyList<Brand> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken ct);

        void Remove(Brand brand);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
