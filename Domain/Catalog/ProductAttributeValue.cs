using System.Globalization;

namespace Domain.Catalog
{
    public class ProductAttributeValue
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid ProductId { get; private set; }
        public Guid AttributeDefinitionId { get; private set; }

        public string? StringValue { get; private set; }
        public decimal? NumericValue { get; private set; }
        public bool? BoolValue { get; private set; }

        public virtual Product Product { get; private set; } = default!;
        public virtual AttributeDefinition AttributeDefinition { get; private set; } = default!;

        private ProductAttributeValue() { }

        private ProductAttributeValue(Guid productId, Guid attributeDefinitionId)
        {
            ProductId = productId;
            AttributeDefinitionId = attributeDefinitionId;
        }

        public static ProductAttributeValue CreateFromRaw(Guid productId, AttributeDefinition def, string? raw)
        {
            var v = new ProductAttributeValue(productId, def.Id);
            v.SetValueFromRaw(def, raw);
            return v;
        }

        public void SetValueFromRaw(AttributeDefinition def, string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                StringValue = null;
                NumericValue = null;
                BoolValue = null;
                return;
            }

            raw = raw.Trim();

            switch (def.DataType)
            {
                case AttributeDataType.String:
                    StringValue = raw;
                    NumericValue = null;
                    BoolValue = null;
                    break;

                case AttributeDataType.Integer:
                    if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                        throw new FormatException($"Cannot parse '{raw}' as int for attribute '{def.Name}'.");
                    StringValue = raw;
                    NumericValue = i;
                    BoolValue = null;
                    break;

                case AttributeDataType.Decimal:
                    if (!decimal.TryParse(raw.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                        throw new FormatException($"Cannot parse '{raw}' as decimal for attribute '{def.Name}'.");
                    StringValue = raw;
                    NumericValue = d;
                    BoolValue = null;
                    break;

                case AttributeDataType.Boolean:
                    var n = raw.ToLowerInvariant();
                    var b = n is "1" or "true" or "yes" or "да";
                    StringValue = n;
                    NumericValue = null;
                    BoolValue = b;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(def.DataType), $"Unsupported type {def.DataType}");
            }
        }
    }
}
