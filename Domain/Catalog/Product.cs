
namespace Domain.Catalog
{
    public class Product
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Sku { get; private set; }           // штрихкод
        public string Article { get; private set; }       // артикул производителя
        public string Name { get; private set; }
        public string? Description { get; private set; }

        public Guid BrandId { get; private set; }
        public Brand? Brand { get; set; }
        public Guid CategoryId { get; private set; }
        public Category? Category { get; set; }

        public decimal Price { get; private set; }
        public decimal? RecommendedRetailPrice { get; private set; }

        public bool HasStock { get; private set; }        // есть ли остаток

        //public List<ProductAttribute> Attributes { get; private set; } = new();
        public List<ProductImage> Images { get; private set; } = new();
        public List<ProductAttributeValue> Attributes { get; private set; } = new();

        private Product() { }

        public Product(
            string sku,
            string name,
            Guid brandId,
            Guid categoryId,
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
            Price = price >= 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            Article = article?.Trim() ?? sku;
            RecommendedRetailPrice = recommendedRetailPrice;
            HasStock = hasStock;
        }

        public void UpdateBasic(string name, string? description, decimal price, decimal? rrp = null, bool? hasStock = null)
        {
            Name = GuardText(name, 500, nameof(name));
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            Price = price >= 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            RecommendedRetailPrice = rrp ?? RecommendedRetailPrice;
            if (hasStock.HasValue) HasStock = hasStock.Value;
        }

        public void SetHasStock(bool value) => HasStock = value;
        public void SetRecommendedPrice(decimal? value) => RecommendedRetailPrice = value;
        public void SetArticle(string value) => Article = GuardText(value, 128, nameof(value));

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

            var allowedAttrIds = category.Attributes
                .Select(a => a.AttributeDefinitionId)
                .ToHashSet();

            Attributes.RemoveAll(a => !allowedAttrIds.Contains(a.AttributeDefinitionId));

            foreach (var catAttr in category.Attributes)
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

        private static string GuardText(string value, int max, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} required");
            var trimmed = value.Trim();
            if (trimmed.Length > max) throw new ArgumentException($"{name} too long");
            return trimmed;
        }
    }
}
