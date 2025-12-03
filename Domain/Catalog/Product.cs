
namespace Domain.Catalog
{
    public class Product
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>Штрихкод.</summary>
        public string Sku { get; private set; }

        /// <summary>Артикул производителя.</summary>
        public string Article { get; private set; }

        public string Name { get; private set; }
        public string? Description { get; private set; }

        public Guid BrandId { get; private set; }
        public virtual Brand Brand { get; private set; } = default!;

        public Guid CategoryId { get; private set; }
        public virtual Category Category { get; private set; } = default!;

        public Guid UnitId { get; private set; }
        public virtual MeasurementUnit Unit { get; private set; } = default!;

        public decimal Price { get; private set; }
        public decimal? RecommendedRetailPrice { get; private set; }
        public bool HasStock { get; private set; }

        public List<string> Advantages { get; private set; } = new();
        public List<string> Complectation { get; private set; } = new();
        public List<ProductImage> Images { get; private set; } = new();
        public List<ProductAttributeValue> Attributes { get; private set; } = new();

        private Product() { }

        public Product(
            string sku,
            string name,
            Guid brandId,
            Guid categoryId,
            Guid unitId,
            decimal price,
            string? description = null,
            string? article = null,
            decimal? recommendedRetailPrice = null,
            bool hasStock = false)
        {
            Sku = GuardText(sku, 64, nameof(sku));
            Name = GuardText(name, 500, nameof(name));
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            BrandId = brandId;
            CategoryId = categoryId;
            UnitId = unitId;

            Price = price >= 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            Article = article?.Trim() ?? sku;
            RecommendedRetailPrice = recommendedRetailPrice;
            HasStock = hasStock;
        }

        public void UpdateBasic(
            string name,
            string? description,
            decimal price,
            decimal? rrp = null,
            bool? hasStock = null,
            Guid? unitId = null)
        {
            Name = GuardText(name, 500, nameof(name));
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            Price = price >= 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));

            if (rrp.HasValue)
                RecommendedRetailPrice = rrp.Value;

            if (hasStock.HasValue)
                HasStock = hasStock.Value;

            if (unitId.HasValue)
                UnitId = unitId.Value;
        }

        public void SetHasStock(bool value) => HasStock = value;
        public void SetRecommendedPrice(decimal? value) => RecommendedRetailPrice = value;
        public void SetArticle(string value) => Article = GuardText(value, 128, nameof(value));
        public void SetUnit(Guid unitId) => UnitId = unitId;

        /// <summary>
        /// Применить значения атрибутов с учётом того, какие атрибуты привязаны к категории.
        /// values: key = AttributeDefinition.Id, value = сырая строка из UI/импорта.
        /// </summary>
        public void ApplyAttributeValues(
            Category category,
            IReadOnlyDictionary<Guid, string?> values,
            IReadOnlyDictionary<Guid, AttributeDefinition> attributeDefinitions)
        {
            if (category.Id != CategoryId)
                throw new InvalidOperationException("Product category does not match provided category.");

            // ВАЖНО: используем коллекцию CategoryAttributes (а не старое Attributes)
            var allowedAttrIds = category.CategoryAttributes
                .Select(a => a.AttributeDefinitionId)
                .ToHashSet();

            // вычищаем значения по атрибутам, которых больше нет в категории
            Attributes.RemoveAll(a => !allowedAttrIds.Contains(a.AttributeDefinitionId));

            foreach (var catAttr in category.CategoryAttributes)
            {
                if (!attributeDefinitions.TryGetValue(catAttr.AttributeDefinitionId, out var def))
                    throw new InvalidOperationException("Attribute definition not found for category attribute.");

                values.TryGetValue(def.Id, out var rawValue);

                var existing = Attributes.SingleOrDefault(a => a.AttributeDefinitionId == def.Id);
                if (existing is null)
                {
                    var val = ProductAttributeValue.CreateFromRaw(Id, def, rawValue);
                    Attributes.Add(val);
                }
                else
                {
                    existing.SetValueFromRaw(def, rawValue);
                }
            }
        }

        public void SetAdvantages(IEnumerable<string>? advantages)
        {
            Advantages.Clear();

            if (advantages is null)
                return;

            foreach (var a in advantages)
            {
                if (string.IsNullOrWhiteSpace(a))
                    continue;

                var trimmed = a.Trim();
                if (trimmed.Length > 1000)
                    trimmed = trimmed[..1000];

                Advantages.Add(trimmed);
            }
        }

        public void SetComplectation(IEnumerable<string>? complectation)
        {
            Complectation.Clear();

            if (complectation is null)
                return;

            foreach (var item in complectation)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                var trimmed = item.Trim();
                if (trimmed.Length > 1000)
                    trimmed = trimmed[..1000];

                Complectation.Add(trimmed);
            }
        }

        private static string GuardText(string value, int max, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} required");

            var trimmed = value.Trim();
            if (trimmed.Length > max)
                throw new ArgumentException($"{name} too long");

            return trimmed;
        }
    }
}
