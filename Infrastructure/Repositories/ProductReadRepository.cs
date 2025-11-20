using Application.Abstractions;
using Application.Products;
using Application.Products.DTOs;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;


namespace Infrastructure.Repositories
{
    public partial class ProductReadRepository(StroymagDbContext db) : IProductReadRepository
    {
        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct) =>
            await db.Set<Product>().AsNoTracking().FirstOrDefaultAsync(p => p.Sku == sku, ct);

        public async Task<Product?> GetByArticleAsync(string article, CancellationToken ct) =>
            await db.Set<Product>().AsNoTracking().FirstOrDefaultAsync(p => p.Article == article, ct);

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
                q = q.Where(p => EF.Functions.ILike(p.Name, "%" + pattern + "%")); // PostgreSQL
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

        public async Task<(IReadOnlyList<ProductListItemDto> Items, int Total)> SearchSmartAsync(
            string q, int page, int pageSize, CancellationToken ct)
        {
            var qRaw = q.Trim();
            if (string.IsNullOrWhiteSpace(qRaw))
                return (Array.Empty<ProductListItemDto>(), 0);

            var qNorm = CompactSpaces(qRaw);
            var tokens = qNorm.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var isDigits = DigitsRegex().IsMatch(qNorm);

            var set = db.Set<Product>().AsNoTracking();

#if USE_SQL_SERVER
    var qLower = qNorm.ToLower();
    IQueryable<Product> filtered = set.Where(p =>
           p.Sku == qNorm
        || p.Article == qNorm
        || p.Sku.StartsWith(qNorm)
        || (isDigits && p.Article.StartsWith(qNorm))
        || p.Name.ToLower().Contains(qLower)
    );
    if (tokens.Length > 1)
    {
        foreach (var t in tokens)
        {
            var tl = t.ToLower();
            filtered = filtered.Where(p => p.Name.ToLower().Contains(tl));
        }
    }

    var ranked = filtered.Select(p => new
    {
        P = p,
        Rank =
            p.Sku == qNorm ? 100 :
            p.Article == qNorm ? 95 :
            p.Sku.StartsWith(qNorm) ? 90 :
            (isDigits && p.Article.StartsWith(qNorm)) ? 85 :
            p.Name.ToLower().StartsWith(qLower) ? 80 :
            60
    });
#else
            IQueryable<Product> filtered = set.Where(p =>
                   p.Sku == qNorm
                || p.Article == qNorm
                || EF.Functions.ILike(p.Sku, qNorm + "%")
                || (isDigits && EF.Functions.ILike(p.Article, qNorm + "%"))
                || EF.Functions.ILike(p.Name, "%" + qNorm + "%")
            );

            if (tokens.Length > 1)
            {
                foreach (var t in tokens)
                    filtered = filtered.Where(p => EF.Functions.ILike(p.Name, "%" + t + "%"));
            }

            var ranked = filtered.Select(p => new
            {
                P = p,
                Rank =
                    p.Sku == qNorm ? 100 :
                    p.Article == qNorm ? 95 :
                    EF.Functions.ILike(p.Sku, qNorm + "%") ? 90 :
                    (isDigits && EF.Functions.ILike(p.Article, qNorm + "%")) ? 85 :
                    EF.Functions.ILike(p.Name, qNorm + "%") ? 80 :
                    60
            });
#endif

            var total = await ranked.CountAsync(ct);

            var items = await ranked
                .OrderByDescending(x => x.Rank)
                .ThenBy(x => x.P.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductListItemDto(
                    x.P.Id,
                    x.P.Sku,
                    x.P.Name,
                    x.P.Brand != null ? x.P.Brand.Name : null,
                    new PriceDto(x.P.Price, x.P.RecommendedRetailPrice, "RUB"),
                    x.P.HasStock
                ))
                .ToListAsync(ct);

            return (items, total);
        }


        public async Task<(IReadOnlyList<ProductListItemDto> Items, int Total)> GetByCategoryIdsAsync(
            IReadOnlyList<Guid> categoryIds,
            int page,
            int pageSize,
            CancellationToken ct)
        {
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
                    p.Brand != null ? p.Brand.Name : null,
                    new PriceDto(p.Price, p.RecommendedRetailPrice, "RUB"),
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
                    p.Brand != null ? p.Brand.Name : null,
                    new PriceDto(p.Price, p.RecommendedRetailPrice, "RUB"),
                    p.HasStock
                ))
                .ToListAsync(ct);

            return (items, total);
        }

        //public async Task<ProductDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct)
        //{
        //    // базовый срез (товар + имя бренда + краткая категория)
        //    var baseInfo = await db.Set<Product>()
        //        .AsNoTracking()
        //        .Where(p => p.Id == id)
        //        .Select(p => new
        //        {
        //            p.Id,
        //            p.Sku,
        //            p.Article,
        //            p.Name,
        //            p.Description,
        //            p.Price,
        //            p.RecommendedRetailPrice,
        //            p.HasStock,
        //            BrandName = p.Brand != null
        //                ? p.Brand.Name
        //                : db.Set<Brand>().Where(b => b.Id == p.BrandId).Select(b => b.Name).FirstOrDefault(),
        //            CategoryBrief = p.Category != null
        //                ? new { p.Category.Id, p.Category.Name, p.Category.Slug }
        //                : db.Set<Category>().Where(c => c.Id == p.CategoryId)
        //                                    .Select(c => new { c.Id, c.Name, c.Slug })
        //                                    .FirstOrDefault()
        //        })
        //        .FirstOrDefaultAsync(ct);

        //    if (baseInfo is null)
        //        return null;

        //    // изображения: только URL
        //    var images = await db.Set<ProductImage>()
        //        .AsNoTracking()
        //        .Where(i => i.ProductId == baseInfo.Id)
        //        .OrderBy(i => i.SortOrder)
        //        .Select(i => i.Url)
        //        .ToListAsync(ct);

        //    // атрибуты: словарь (Name/Key → Value). Оставляю Key, если у тебя именно так в модели.
        //    var attributes = await db.Set<ProductAttribute>()
        //        .AsNoTracking()
        //        .Where(a => a.ProductId == baseInfo.Id)
        //        .ToDictionaryAsync(a => a.Key, a => a.Value, ct);

        //    // сборка DTO (ужатый формат)
        //    return new ProductDetailsDto(
        //        Id: baseInfo.Id,
        //        Sku: baseInfo.Sku,
        //        Article: baseInfo.Article,
        //        Name: baseInfo.Name,
        //        Description: baseInfo.Description,
        //        Brand: baseInfo.BrandName ?? string.Empty,
        //        CategoryId: baseInfo.CategoryBrief?.Id ?? Guid.Empty,
        //        CategoryName: baseInfo.CategoryBrief?.Name ?? string.Empty,
        //        CategorySlug: baseInfo.CategoryBrief?.Slug ?? string.Empty,
        //        Price: new PriceDto(baseInfo.Price, baseInfo.RecommendedRetailPrice),
        //        InStock: baseInfo.HasStock,
        //        Images: images,
        //        Attributes: attributes
        //    );
        //}

        private static string CompactSpaces(string s)
        {
            while (s.Contains("  ")) s = s.Replace("  ", " ");
            return s;
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex DigitsRegex();
    }
}
