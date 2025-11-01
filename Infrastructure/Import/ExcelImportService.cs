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

    public sealed class ExcelImportService : IExcelImportService
    {
        private readonly StroymagDbContext _db;
        private readonly ILogger<ExcelImportService> _log;

        public ExcelImportService(StroymagDbContext db, ILogger<ExcelImportService> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<ExcelImportResult> ImportAsync(Stream excel, CancellationToken ct = default)
        {
            using var wb = new XLWorkbook(excel);
            var ws = wb.Worksheets.First();

            // ===== 1) Заголовки
            var headerRow = ws.FirstRowUsed();
            var headerCells = headerRow.CellsUsed().ToList();
            var header = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < headerCells.Count; i++)
            {
                var name = (headerCells[i].GetString() ?? "").Trim();
                if (!string.IsNullOrEmpty(name)) header[name] = i + 1; // 1-based
            }

            int C(string name) => header.TryGetValue(name, out var i) ? i : -1;

            var COL = new
            {
                Section = C("Раздел"),
                Group = C("Группа"),
                Subgroup = C("Подгруппа"),
                Article = C("Артикул"),
                Name = C("Наименование"),
                Brand = C("Торговая марка"),
                HasStock = C("Остатки"),
                Price = C("Цена, руб"),
                Rrp = C("РРЦ, руб"),
                Sku = C("Штрих код")
            };

            // ============================================================
            // PASS 1: СОБРАТЬ / СОЗДАТЬ ВСЕ КАТЕГОРИИ (по всему файлу)
            // ============================================================

            // загрузим уже существующие категории из БД
            var categoriesByPath = await LoadCategoryPathsAsync(ct); // "Раздел/Группа/Подгруппа" -> Guid

            int categoriesUpserted = 0;

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var section = S(row, COL.Section);
                var group = S(row, COL.Group);
                var subgroup = S(row, COL.Subgroup);

                var parts = new[] { section, group, subgroup }
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x!.Trim())
                    .ToArray();

                if (parts.Length == 0)
                    continue;

                var path = string.Join("/", parts);

                if (categoriesByPath.ContainsKey(path))
                    continue;

                // создаём всю цепочку
                var id = EnsureCategoryPathFull(parts, categoriesByPath);
                if (id != Guid.Empty)
                    categoriesUpserted++;
            }

            // один сейв после создания категорий
            await _db.SaveChangesAsync(ct);

            // ============================================================
            // PASS 2: ОБРАБОТАТЬ ТОВАРЫ (до первого пустого SKU)
            // ============================================================

            // бренды
            var brandByName = await _db.Brands.AsNoTracking()
                .ToDictionaryAsync(b => b.Name, b => b.Id, StringComparer.OrdinalIgnoreCase, ct);

            // продукты (Guid)
            var existingProductIds = new HashSet<Guid>(await _db.Products.AsNoTracking()
                .Select(p => p.Id)
                .ToListAsync(ct));

            int brandsUpserted = 0;
            int productsUpserted = 0;
            int processedRows = 0;
            int? stoppedAtRow = null;
            string? message = null;

            const int BATCH_SIZE = 500;
            int batchCounter = 0;

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var section = S(row, COL.Section);
                var group = S(row, COL.Group);
                var subgroup = S(row, COL.Subgroup);
                var article = S(row, COL.Article);
                var name = S(row, COL.Name);
                var brandName = S(row, COL.Brand);
                var hasStockStr = S(row, COL.HasStock);
                var sku = S(row, COL.Sku);            // ОБЯЗАТЕЛЬНО
                var price = D(row, COL.Price) ?? 0m;
                var rrp = D(row, COL.Rrp);

                // если штрихкод пуст — СТОП
                if (string.IsNullOrWhiteSpace(sku))
                {
                    stoppedAtRow = row.RowNumber();
                    message = $"Обработка остановлена: пустой 'Штрих код' (SKU) в строке {stoppedAtRow}. " +
                              $"Записано строк: {processedRows}.";
                    _log.LogWarning(message);
                    break;
                }

                // бренд
                Guid? brandId = null;
                if (!string.IsNullOrWhiteSpace(brandName))
                {
                    if (!brandByName.TryGetValue(brandName, out var bid))
                    {
                        var newBrand = new Brand(brandName);
                        _db.Brands.Add(newBrand);
                        brandByName[brandName] = newBrand.Id;
                        brandsUpserted++;
                        bid = newBrand.Id;
                    }
                    brandId = bid;
                }

                // категория
                Guid? categoryId = null;
                var parts = new[] { section, group, subgroup }
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x!.Trim())
                    .ToArray();

                if (parts.Length > 0)
                {
                    var path = string.Join("/", parts);
                    if (categoriesByPath.TryGetValue(path, out var cid))
                    {
                        categoryId = cid;
                    }
                }

                bool hasStock = hasStockStr?.Equals("да", StringComparison.OrdinalIgnoreCase) == true;

                // детерминированный Guid под товар
                var keyForGuid = !string.IsNullOrWhiteSpace(article)
                    ? article!
                    : (!string.IsNullOrWhiteSpace(name) ? name! : sku!);

                var productId = CreateDeterministicGuid(keyForGuid);

                if (!existingProductIds.Contains(productId))
                {
                    var articleVal = !string.IsNullOrWhiteSpace(article) ? article! : keyForGuid;

                    var product = new Product(
                        sku: sku!,
                        name: name ?? keyForGuid,
                        brandId: brandId ?? Guid.Empty,
                        categoryId: categoryId ?? Guid.Empty,
                        price: price,
                        description: null,
                        article: articleVal,
                        recommendedRetailPrice: rrp,
                        hasStock: hasStock
                    );

                    typeof(Product).GetProperty(nameof(Product.Id))!
                        .SetValue(product, productId);

                    _db.Products.Add(product);
                    existingProductIds.Add(productId);

                    productsUpserted++;
                }
                else
                {
                    var product = await _db.Products.FirstAsync(p => p.Id == productId, ct);
                    product.UpdateBasic(name ?? keyForGuid, product.Description, price, rrp, hasStock);
                    if (brandId.HasValue)
                        typeof(Product).GetProperty(nameof(Product.BrandId))!.SetValue(product, brandId.Value);
                    if (categoryId.HasValue)
                        typeof(Product).GetProperty(nameof(Product.CategoryId))!.SetValue(product, categoryId.Value);
                    if (!string.IsNullOrWhiteSpace(article))
                        product.SetArticle(article);
                    typeof(Product).GetProperty(nameof(Product.Sku))!.SetValue(product, sku);
                }

                processedRows++;

                if (++batchCounter % BATCH_SIZE == 0)
                    await _db.SaveChangesAsync(ct);
            }

            await _db.SaveChangesAsync(ct);

            if (stoppedAtRow is null)
                message = "Импорт завершён без остановки: достигнут конец файла.";

            _log.LogInformation("Импорт: бренды={Brands}, категории={Cats}, товары={Prods}, обработано={Rows}. {Msg}",
                brandsUpserted, categoriesUpserted, productsUpserted, processedRows, message);

            return new ExcelImportResult(
                BrandsUpserted: brandsUpserted,
                CategoriesUpserted: categoriesUpserted,
                ProductsUpserted: productsUpserted,
                ProcessedRows: processedRows,
                StoppedAtRow: stoppedAtRow,
                Message: message
            );
        }

        // ===== helpers =====

        private static string? S(IXLRow row, int col)
        {
            if (col <= 0) return null;
            var v = row.Cell(col).GetValue<string>()?.Trim();
            return string.IsNullOrWhiteSpace(v) ? null : v;
        }

        private static decimal? D(IXLRow row, int col)
        {
            if (col <= 0) return null;
            var cell = row.Cell(col);
            if (cell.DataType == XLDataType.Number)
                return Convert.ToDecimal(cell.GetDouble(), CultureInfo.InvariantCulture);
            var s = cell.GetValue<string>()?.Trim().Replace(" ", "").Replace(",", ".");
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null;
        }

        /// <summary>
        /// Подгружаем существующие категории и строим path -> id
        /// </summary>
        private async Task<Dictionary<string, Guid>> LoadCategoryPathsAsync(CancellationToken ct)
        {
            var result = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            var all = await _db.Categories.AsNoTracking().ToListAsync(ct);
            var byId = all.ToDictionary(c => c.Id);

            foreach (var c in all)
            {
                var stack = new Stack<string>();
                var cur = c;
                while (cur != null)
                {
                    stack.Push(cur.Name);
                    cur = cur.ParentId.HasValue && byId.TryGetValue(cur.ParentId.Value, out var p) ? p : null;
                }

                var path = string.Join("/", stack);
                if (!result.ContainsKey(path))
                    result[path] = c.Id;
            }

            return result;
        }

        /// <summary>
        /// Создаём ВСЮ цепочку категорий сразу (root -> ... -> leaf), с нормальным slug
        /// </summary>
        private Guid EnsureCategoryPathFull(string[] parts, Dictionary<string, Guid> cache)
        {
            Guid? parent = null;
            var acc = "";

            for (int i = 0; i < parts.Length; i++)
            {
                var name = parts[i].Trim();
                acc = string.IsNullOrEmpty(acc) ? name : $"{acc}/{name}";

                if (!cache.TryGetValue(acc, out var id))
                {
                    var slug = ToSlug(name);
                    var cat = new Category(name, parent, slug, imageUrl: null);
                    _db.Categories.Add(cat);

                    cache[acc] = cat.Id;
                    id = cat.Id;
                }

                parent = id;
            }

            return parent ?? Guid.Empty;
        }

        /// <summary>
        /// Нормальный простой транслит ru -> en
        /// </summary>
        private static string ToSlug(string name)
        {
            name = name.Trim().ToLowerInvariant();

            var map = new Dictionary<char, string>
            {
                ['а'] = "a",
                ['б'] = "b",
                ['в'] = "v",
                ['г'] = "g",
                ['д'] = "d",
                ['е'] = "e",
                ['ё'] = "e",
                ['ж'] = "zh",
                ['з'] = "z",
                ['и'] = "i",
                ['й'] = "y",
                ['к'] = "k",
                ['л'] = "l",
                ['м'] = "m",
                ['н'] = "n",
                ['о'] = "o",
                ['п'] = "p",
                ['р'] = "r",
                ['с'] = "s",
                ['т'] = "t",
                ['у'] = "u",
                ['ф'] = "f",
                ['х'] = "h",
                ['ц'] = "c",
                ['ч'] = "ch",
                ['ш'] = "sh",
                ['щ'] = "sch",
                ['ъ'] = "",
                ['ы'] = "y",
                ['ь'] = "",
                ['э'] = "e",
                ['ю'] = "yu",
                ['я'] = "ya",
            };

            var sb = new StringBuilder();

            foreach (var ch in name)
            {
                if (map.TryGetValue(ch, out var rep))
                {
                    sb.Append(rep);
                }
                else if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9'))
                {
                    sb.Append(ch);
                }
                else if (char.IsWhiteSpace(ch) || ch == '_' || ch == '-')
                {
                    sb.Append('-');
                }
                else
                {
                    // всё остальное → тире
                    sb.Append('-');
                }
            }

            var slug = sb.ToString();

            // убрать повторяющиеся '-'
            while (slug.Contains("--"))
                slug = slug.Replace("--", "-");

            slug = slug.Trim('-');

            return string.IsNullOrWhiteSpace(slug) ? "category" : slug;
        }

        /// <summary>
        /// Детерминированный Guid по строке
        /// </summary>
        private static Guid CreateDeterministicGuid(string input)
        {
            var ns = Guid.Parse("e5f4a7f4-3e2e-4cf5-9a25-2b0d2e5d3f01");
            var nsBytes = ns.ToByteArray();
            Swap(nsBytes, 0, 3); Swap(nsBytes, 1, 2); Swap(nsBytes, 4, 5); Swap(nsBytes, 6, 7);

            var nameBytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA1.HashData(nsBytes.Concat(nameBytes).ToArray());

            hash[6] = (byte)((hash[6] & 0x0F) | (5 << 4));
            hash[8] = (byte)((hash[8] & 0x3F) | 0x80);

            Swap(hash, 0, 3); Swap(hash, 1, 2); Swap(hash, 4, 5); Swap(hash, 6, 7);
            var g16 = new byte[16];
            Array.Copy(hash, 0, g16, 0, 16);
            return new Guid(g16);

            static void Swap(byte[] a, int i, int j) => (a[i], a[j]) = (a[j], a[i]);
        }
    }
}
