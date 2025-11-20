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
                    if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal))
                        throw new FormatException($"Cannot parse '{raw}' as integer for attribute '{def.Name}'.");
                    StringValue = raw;
                    NumericValue = intVal;
                    BoolValue = null;
                    break;

                case AttributeDataType.Decimal:
                    if (!decimal.TryParse(raw.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var decVal))
                        throw new FormatException($"Cannot parse '{raw}' as decimal for attribute '{def.Name}'.");
                    StringValue = raw;
                    NumericValue = decVal;
                    BoolValue = null;
                    break;

                case AttributeDataType.Boolean:
                    var n = raw.ToLowerInvariant();
                    bool boolVal = n is "1" or "true" or "да" or "yes";
                    StringValue = n;
                    NumericValue = null;
                    BoolValue = boolVal;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(def.DataType), $"Unsupported data type {def.DataType}");
            }
        }
    }
}
