using Domain.Catalog;

namespace Application.Abstractions.Admin
{
    public interface ICategoryAdminRepository
    {
        Task AddAsync(Category category, CancellationToken ct);

        Task<Category?> GetWithAttributesAsync(Guid id, CancellationToken ct);

        Task<(IReadOnlyList<Category> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken ct);

        void Remove(Category category);

        Task<int> SaveChangesAsync(CancellationToken ct);
        Task<IReadOnlyList<Category>> GetFlatWithAttributesAsync(CancellationToken ct);
    }
}
