using Domain.Catalog;

namespace Application.Abstractions.Admin
{
    public interface ICategoryAdminRepository
    {
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(Category category, CancellationToken ct);

        Task<Category?> GetWithAttributesAsync(Guid id, CancellationToken ct);

        Task<(IReadOnlyList<Category> Items, int Total)> GetPagedAsync(
            int page,
            int pageSize,
            string? name,
            CancellationToken ct);

        void Remove(Category category);

        Task<int> SaveChangesAsync(CancellationToken ct);
        Task<IReadOnlyList<Category>> GetFlatWithAttributesAsync(CancellationToken ct);
    }
}
