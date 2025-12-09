using ClosedXML.Excel;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Import
{
    public interface IExcelImportService
    {
        Task<ExcelImportResult> ImportAsync(Stream excel, CancellationToken ct = default);
    }

    public sealed record ExcelImportResult(
        int BrandsUpserted,
        int CategoriesUpserted,
        int ProductsUpserted,
        int ProcessedRows,
        int? StoppedAtRow,
        string? Message
    );

    //public sealed class ExcelImportService : IExcelImportService
    //{
    //    private readonly StroymagDbContext _db;
    //    private readonly ILogger<ExcelImportService> _log;

    //    public ExcelImportService(StroymagDbContext db, ILogger<ExcelImportService> log)
    //    {
    //        _db = db;
    //        _log = log;
    //    }

    //    public async Task<ExcelImportResult> ImportAsync(Stream excel, CancellationToken ct = default)
    //    {
    //        using var wb = new XLWorkbook(excel);
    //        var ws = wb.Worksheets.First();

    //        // ===== 1) Заголовки
    //        var headerRow = ws.FirstRowUsed();
    //        var headerCells = headerRow.CellsUsed().ToList();
    //        var header = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    //        for (int i = 0; i < headerCells.Count; i++)
    //        {
    //            var name = (headerCells[i].GetString() ?? "").Trim();
    //            if (!string.IsNullOrEmpty(name)) header[name] = i + 1; // 1-based
    //        }

    //        int C(string name) => header.TryGetValue(name, out var i) ? i : -1;

    //        var COL = new
    //        {
    //            Section = C("Раздел"),
    //            Group = C("Группа"),
    //            Subgroup = C("Подгруппа"),
    //            Article = C("Артикул"),
    //            Name = C("Наименование"),
    //            Brand = C("Торговая марка"),
    //            HasStock = C("Остатки"),
    //            Price = C("Цена, руб"),
    //            Rrp = C("РРЦ, руб"),
    //            Sku = C("Штрих код")
    //        };

    //        // ============================================================
    //        // PASS 1: категории (собираем весь справочник)
    //        // ============================================================

    //        var categoriesByPath = await LoadCategoryPathsAsync(ct); // "Раздел/Группа/Подгруппа" -> Guid
    //        int categoriesUpserted = 0;

    //        foreach (var row in ws.RowsUsed().Skip(1))
    //        {
    //            var parts = new[]
    //            {
    //                S(row, COL.Section),
    //                S(row, COL.Group),
    //                S(row, COL.Subgroup)
    //            }
    //            .Where(x => !string.IsNullOrWhiteSpace(x))
    //            .Select(x => x!.Trim())
    //            .ToArray();

    //            if (parts.Length == 0) continue;

    //            var path = string.Join("/", parts);
    //            if (categoriesByPath.ContainsKey(path)) continue;

    //            var id = EnsureCategoryPathFull(parts, categoriesByPath);
    //            if (id != Guid.Empty) categoriesUpserted++;
    //        }

    //        await _db.SaveChangesAsync(ct);

    //        // ============================================================
    //        // PASS 2: справочники и текущие товары
    //        // ============================================================

    //        var brandByName = await _db.Brands.AsNoTracking()
    //            .ToDictionaryAsync(b => b.Name, b => b.Id, StringComparer.OrdinalIgnoreCase, ct);

    //        // Быстрый доступ к продуктам по SKU и по Id
    //        var productsBySku = await _db.Products.AsNoTracking()
    //            .Where(p => p.Sku != null && p.Sku != "")
    //            .ToDictionaryAsync(p => p.Sku!, p => p.Id, StringComparer.OrdinalIgnoreCase, ct);

    //        var productsById = new HashSet<Guid>(await _db.Products.AsNoTracking()
    //            .Select(p => p.Id)
    //            .ToListAsync(ct));

    //        int brandsUpserted = 0;
    //        int productsUpserted = 0;
    //        int processedRows = 0;
    //        int? stoppedAtRow = null;      // больше не используется как «жёсткая» остановка
    //        string? message = null;

    //        const int BATCH_SIZE = 500;
    //        int batchCounter = 0;

    //        var warnings = new List<string>();

    //        foreach (var row in ws.RowsUsed().Skip(1))
    //        {
    //            var section = S(row, COL.Section);
    //            var group = S(row, COL.Group);
    //            var subgroup = S(row, COL.Subgroup);
    //            var article = S(row, COL.Article);
    //            var name = S(row, COL.Name);
    //            var brandNameRaw = S(row, COL.Brand);
    //            var hasStockStr = S(row, COL.HasStock);
    //            var sku = S(row, COL.Sku); // желателен
    //            var price = D(row, COL.Price) ?? 0m;
    //            var rrp = D(row, COL.Rrp);

    //            // 1) SKU пуст — просто пропускаем строку (но не останавливаем импорт)
    //            if (string.IsNullOrWhiteSpace(sku))
    //            {
    //                warnings.Add($"Строка {row.RowNumber()}: пустой SKU — пропуск.");
    //                continue;
    //            }

    //            // 2) Бренд (нормализация)
    //            Guid? brandId = null;
    //            if (!string.IsNullOrWhiteSpace(brandNameRaw))
    //            {
    //                var brandName = brandNameRaw!.Trim();
    //                if (!brandByName.TryGetValue(brandName, out var bid))
    //                {
    //                    var newBrand = new Brand(brandName);
    //                    _db.Brands.Add(newBrand);
    //                    brandByName[brandName] = newBrand.Id;
    //                    brandsUpserted++;
    //                    bid = newBrand.Id;
    //                }
    //                brandId = bid;
    //            }

    //            // 3) Категория
    //            Guid? categoryId = null;
    //            var parts = new[] { section, group, subgroup }
    //                .Where(x => !string.IsNullOrWhiteSpace(x))
    //                .Select(x => x!.Trim())
    //                .ToArray();

    //            if (parts.Length > 0)
    //            {
    //                var path = string.Join("/", parts);
    //                if (categoriesByPath.TryGetValue(path, out var cid))
    //                    categoryId = cid;
    //            }

    //            bool hasStock = hasStockStr?.Equals("да", StringComparison.OrdinalIgnoreCase) == true;

    //            // 4) Определяем идентичность товара
    //            //    — приоритетно по SKU
    //            Guid productId;
    //            if (productsBySku.TryGetValue(sku!, out var existingId))
    //            {
    //                productId = existingId;
    //            }
    //            else
    //            {
    //                // новый — генерим детерминированный Guid от SKU
    //                productId = CreateDeterministicGuid(sku!);
    //            }

    //            // 5) Upsert товара
    //            if (!productsById.Contains(productId))
    //            {
    //                var articleVal = !string.IsNullOrWhiteSpace(article) ? article! : sku!;

    //                var product = new Product(
    //                    sku: sku!,
    //                    name: name ?? articleVal,
    //                    brandId: brandId ?? Guid.Empty,
    //                    categoryId: categoryId ?? Guid.Empty,
    //                    price: price,
    //                    description: null,
    //                    article: articleVal,
    //                    recommendedRetailPrice: rrp,
    //                    hasStock: hasStock
    //                );

    //                // подставляем Id (детерминированный по SKU)
    //                typeof(Product).GetProperty(nameof(Product.Id))!
    //                    .SetValue(product, productId);

    //                _db.Products.Add(product);
    //                productsById.Add(productId);
    //                productsBySku[sku!] = productId;

    //                productsUpserted++;
    //            }
    //            else
    //            {
    //                // Товар существует: обновляем «лёгкие» поля и FK, если есть
    //                var product = new Product(
    //                    sku: sku!,
    //                    name: name ?? (article ?? sku),
    //                    brandId: brandId ?? Guid.Empty,
    //                    categoryId: categoryId ?? Guid.Empty,
    //                    price: price,
    //                    description: null,
    //                    article: !string.IsNullOrWhiteSpace(article) ? article! : (name ?? sku),
    //                    recommendedRetailPrice: rrp,
    //                    hasStock: hasStock
    //                );

    //                typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, productId);

    //                _db.Attach(product);
    //                // Обновляем только те поля, которые должны меняться
    //                _db.Entry(product).Property(p => p.Name).IsModified = true;
    //                _db.Entry(product).Property(p => p.Price).IsModified = true;
    //                _db.Entry(product).Property(p => p.RecommendedRetailPrice).IsModified = true;
    //                _db.Entry(product).Property(p => p.HasStock).IsModified = true;
    //                _db.Entry(product).Property(p => p.Sku).IsModified = true;
    //                _db.Entry(product).Property(p => p.Article).IsModified = true;

    //                if (brandId.HasValue)
    //                    _db.Entry(product).Property(p => p.BrandId).IsModified = true;
    //                if (categoryId.HasValue)
    //                    _db.Entry(product).Property(p => p.CategoryId).IsModified = true;
    //            }

    //            processedRows++;

    //            if (++batchCounter % BATCH_SIZE == 0)
    //                await _db.SaveChangesAsync(ct);
    //        }

    //        await _db.SaveChangesAsync(ct);

    //        if (warnings.Count > 0)
    //            message = string.Join(" | ", warnings.Take(5)) + (warnings.Count > 5 ? $" | и ещё {warnings.Count - 5} предупреждений" : "");
    //        else
    //            message = "Импорт завершён без предупреждений.";

    //        _log.LogInformation("Импорт: бренды={Brands}, категории={Cats}, товары={Prods}, обработано={Rows}. {Msg}",
    //            brandsUpserted, categoriesUpserted, productsUpserted, processedRows, message);

    //        return new ExcelImportResult(
    //            BrandsUpserted: brandsUpserted,
    //            CategoriesUpserted: categoriesUpserted,
    //            ProductsUpserted: productsUpserted,
    //            ProcessedRows: processedRows,
    //            StoppedAtRow: stoppedAtRow, // оставил для совместимости с контрактом
    //            Message: message
    //        );
    //    }

    //    // ===== helpers =====

    //    private static string? S(IXLRow row, int col)
    //    {
    //        if (col <= 0) return null;
    //        var v = row.Cell(col).GetValue<string>()?.Trim();
    //        return string.IsNullOrWhiteSpace(v) ? null : v;
    //    }

    //    private static decimal? D(IXLRow row, int col)
    //    {
    //        if (col <= 0) return null;
    //        var cell = row.Cell(col);
    //        if (cell.DataType == XLDataType.Number)
    //            return Convert.ToDecimal(cell.GetDouble(), CultureInfo.InvariantCulture);
    //        var s = cell.GetValue<string>()?.Trim().Replace(" ", "").Replace(",", ".");
    //        return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null;
    //    }

    //    private async Task<Dictionary<string, Guid>> LoadCategoryPathsAsync(CancellationToken ct)
    //    {
    //        var result = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
    //        var all = await _db.Categories.AsNoTracking().ToListAsync(ct);
    //        var byId = all.ToDictionary(c => c.Id);

    //        foreach (var c in all)
    //        {
    //            var stack = new Stack<string>();
    //            var cur = c;
    //            while (cur != null)
    //            {
    //                stack.Push(cur.Name);
    //                cur = cur.ParentId.HasValue && byId.TryGetValue(cur.ParentId.Value, out var p) ? p : null;
    //            }

    //            var path = string.Join("/", stack);
    //            if (!result.ContainsKey(path))
    //                result[path] = c.Id;
    //        }

    //        return result;
    //    }

    //    private Guid EnsureCategoryPathFull(string[] parts, Dictionary<string, Guid> cache)
    //    {
    //        Guid? parent = null;
    //        var acc = "";

    //        for (int i = 0; i < parts.Length; i++)
    //        {
    //            var name = parts[i].Trim();
    //            acc = string.IsNullOrEmpty(acc) ? name : $"{acc}/{name}";

    //            if (!cache.TryGetValue(acc, out var id))
    //            {
    //                var slug = ToSlug(name);
    //                var cat = new Category(name, parent, slug, imageUrl: null);
    //                _db.Categories.Add(cat);

    //                cache[acc] = cat.Id;
    //                id = cat.Id;
    //            }

    //            parent = id;
    //        }

    //        return parent ?? Guid.Empty;
    //    }

    //    private static string ToSlug(string name)
    //    {
    //        name = name.Trim().ToLowerInvariant();

    //        var map = new Dictionary<char, string>
    //        {
    //            ['а'] = "a",
    //            ['б'] = "b",
    //            ['в'] = "v",
    //            ['г'] = "g",
    //            ['д'] = "d",
    //            ['е'] = "e",
    //            ['ё'] = "e",
    //            ['ж'] = "zh",
    //            ['з'] = "z",
    //            ['и'] = "i",
    //            ['й'] = "y",
    //            ['к'] = "k",
    //            ['л'] = "l",
    //            ['м'] = "m",
    //            ['н'] = "n",
    //            ['о'] = "o",
    //            ['п'] = "p",
    //            ['р'] = "r",
    //            ['с'] = "s",
    //            ['т'] = "t",
    //            ['у'] = "u",
    //            ['ф'] = "f",
    //            ['х'] = "h",
    //            ['ц'] = "c",
    //            ['ч'] = "ch",
    //            ['ш'] = "sh",
    //            ['щ'] = "sch",
    //            ['ъ'] = "",
    //            ['ы'] = "y",
    //            ['ь'] = "",
    //            ['э'] = "e",
    //            ['ю'] = "yu",
    //            ['я'] = "ya",
    //        };

    //        var sb = new StringBuilder();
    //        foreach (var ch in name)
    //        {
    //            if (map.TryGetValue(ch, out var rep)) sb.Append(rep);
    //            else if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9')) sb.Append(ch);
    //            else if (char.IsWhiteSpace(ch) || ch == '_' || ch == '-') sb.Append('-');
    //            else sb.Append('-');
    //        }

    //        var slug = sb.ToString();
    //        while (slug.Contains("--")) slug = slug.Replace("--", "-");
    //        slug = slug.Trim('-');
    //        return string.IsNullOrWhiteSpace(slug) ? "category" : slug;
    //    }

    //    private static Guid CreateDeterministicGuid(string input)
    //    {
    //        var ns = Guid.Parse("e5f4a7f4-3e2e-4cf5-9a25-2b0d2e5d3f01");
    //        var nsBytes = ns.ToByteArray();
    //        Swap(nsBytes, 0, 3); Swap(nsBytes, 1, 2); Swap(nsBytes, 4, 5); Swap(nsBytes, 6, 7);

    //        var nameBytes = Encoding.UTF8.GetBytes(input);
    //        var hash = SHA1.HashData(nsBytes.Concat(nameBytes).ToArray());

    //        hash[6] = (byte)((hash[6] & 0x0F) | (5 << 4));
    //        hash[8] = (byte)((hash[8] & 0x3F) | 0x80);

    //        Swap(hash, 0, 3); Swap(hash, 1, 2); Swap(hash, 4, 5); Swap(hash, 6, 7);
    //        var g16 = new byte[16];
    //        Array.Copy(hash, 0, g16, 0, 16);
    //        return new Guid(g16);

    //        static void Swap(byte[] a, int i, int j) => (a[i], a[j]) = (a[j], a[i]);
    //    }
    //}
}
