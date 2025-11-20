using Domain.Catalog;

namespace Application.Abstractions.Admin
{
    public interface IProductAdminRepository
    {
        Task AddAsync(Product product, CancellationToken ct);
        Task<Product?> GetWithAttributesAsync(Guid id, CancellationToken ct); // Product + Attributes
        void Remove(Product product);
        Task<int> SaveChangesAsync(CancellationToken ct);
        Task<(IReadOnlyList<Product> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    }
}
