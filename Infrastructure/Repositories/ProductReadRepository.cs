using Application.Abstractions;
using Application.Products;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace Infrastructure.Repositories
{
    public partial class ProductReadRepository(StroymagDbContext db) : IProductReadRepository
    {
        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct) =>
            await db.Set<Product>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Sku == sku, ct);

        public async Task<Product?> GetByArticleAsync(string article, CancellationToken ct) =>
            await db.Set<Product>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Article == article, ct);

        public async Task<(IReadOnlyList<Product> Items, int Total)> SearchByNameAsync(
            string nameLike, int page, int pageSize, CancellationToken ct)
            => await SearchInternal(null, nameLike, null, page, pageSize, ct);

        public async Task<(IReadOnlyList<Product> Items, int Total)> SearchAsync(
            string? sku, string? nameLike, string? article, int page, int pageSize, CancellationToken ct)
            => await SearchInternal(sku, nameLike, article, page, pageSize, ct);

        private async Task<(IReadOnlyList<Product>, int)> SearchInternal(
            string? sku, string? nameLike, string? article, int page, int pageSize, CancellationToken ct)
        {
            var q = db.Set<Product>().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(sku))
                q = q.Where(p => p.Sku == sku);

            if (!string.IsNullOrWhiteSpace(article))
                q = q.Where(p => p.Article == article);

            if (!string.IsNullOrWhiteSpace(nameLike))
            {
#if USE_SQL_SERVER
            var pattern = nameLike.Trim().ToLower();
            q = q.Where(p => p.Name.ToLower().Contains(pattern));
#else
                var pattern = nameLike.Trim();
                q = q.Where(p => EF.Functions.ILike(p.Name, $"%{pattern}%")); // PostgreSQL
#endif
            }

            var total = await q.CountAsync(ct);

            var items = await q
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<(IReadOnlyList<Product> Items, int Total)> SearchSmartAsync(
            string q, int page, int pageSize, CancellationToken ct)
        {
            var qRaw = q.Trim();
            if (string.IsNullOrWhiteSpace(qRaw))
                return (Array.Empty<Product>(), 0);

            var qNorm = CompactSpaces(qRaw);
            var tokens = qNorm.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var isDigits = DigitsRegex().IsMatch(qNorm);

            var set = db.Set<Product>().AsNoTracking();

            var filtered = set.Where(p =>
                   p.Sku == qNorm
                || p.Article == qNorm
                || EF.Functions.ILike(p.Sku, qNorm + "%")
                || (isDigits && p.Article != null && EF.Functions.ILike(p.Article, qNorm + "%"))
                || EF.Functions.ILike(p.Name, "%" + qNorm + "%")
                // все токены должны встретиться в Name (AND)
                || (tokens.Length > 1 && tokens.All(t => EF.Functions.ILike(p.Name, "%" + t + "%")))
            );

            var ranked = filtered.Select(p => new
            {
                Product = p,
                Rank =
                    p.Sku == qNorm ? 100 :
                    p.Article == qNorm ? 95 :
                    EF.Functions.ILike(p.Sku, qNorm + "%") ? 90 :
                    (isDigits && p.Article != null && EF.Functions.ILike(p.Article, qNorm + "%")) ? 85 :
                    (tokens.Length > 1 && tokens.All(t => EF.Functions.ILike(p.Name, t + "%"))) ? 82 :
                    EF.Functions.ILike(p.Name, qNorm + "%") ? 80 :
                    (tokens.Length > 1 && tokens.All(t => EF.Functions.ILike(p.Name, "%" + t + "%"))) ? 70 :
                    EF.Functions.ILike(p.Name, "%" + qNorm + "%") ? 60 :
                    0
            });

            var total = await ranked.CountAsync(ct);

            var pageItems = await ranked
                .OrderByDescending(x => x.Rank)
                .ThenBy(x => x.Product.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.Product)
                .ToListAsync(ct);

            return (pageItems, total);
        }

        public async Task<(IReadOnlyList<ProductListItemDto> Items, int Total)> GetByCategoryIdsAsync(
            IReadOnlyList<Guid> categoryIds,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            // если нет id — возвращаем пусто
            if (categoryIds.Count == 0)
                return (Array.Empty<ProductListItemDto>(), 0);

            var query = db.Set<Product>()
                .AsNoTracking()
                .Where(p => categoryIds.Contains(p.CategoryId));

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItemDto(
                    p.Id,
                    p.Sku,
                    p.Name,
                    p.Brand.Name,
                    new ProductPriceDto("RUB", p.Price, null),
                    p.HasStock
                ))
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<(IReadOnlyList<ProductListItemDto> Items, int Total)> GetAllAsync(
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = db.Set<Product>().AsNoTracking();

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItemDto(
                    p.Id,
                    p.Sku,
                    p.Name,
                    p.Brand.Name,
                    new ProductPriceDto("RUB", p.Price, null),
                    p.HasStock
                ))
                .ToListAsync(ct);

            return (items, total);
        }
        private static string CompactSpaces(string s)
        {
            while (s.Contains("  ")) s = s.Replace("  ", " ");
            return s;
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex DigitsRegex();
    }
}
