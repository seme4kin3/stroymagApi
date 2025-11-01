using Application.Products;
using Domain.Catalog;

namespace Application.Abstractions
{
    public interface IProductReadRepository
    {
        Task<Product?> GetBySkuAsync(string sku, CancellationToken ct);
        Task<Product?> GetByArticleAsync(string barcode, CancellationToken ct);

        Task<(IReadOnlyList<Product> Items, int Total)> SearchByNameAsync(
            string nameLike, int page, int pageSize, CancellationToken ct);

        Task<(IReadOnlyList<Product> Items, int Total)> SearchAsync(
            string? sku, string? nameLike, string? barcode, int page, int pageSize, CancellationToken ct);

        // ЕДИНЫЙ умный поиск
        Task<(IReadOnlyList<Product> Items, int Total)> SearchSmartAsync(
            string q, int page, int pageSize, CancellationToken ct);

        Task<(IReadOnlyList<ProductListItemDto> Items, int Total)> GetByCategoryIdsAsync(
            IReadOnlyList<Guid> categoryIds,
            int page,
            int pageSize,
            CancellationToken ct);

        // выборка всех (если slug не указан) — опционально
        Task<(IReadOnlyList<ProductListItemDto> Items, int Total)> GetAllAsync(
            int page,
            int pageSize,
            CancellationToken ct);
    }
}
