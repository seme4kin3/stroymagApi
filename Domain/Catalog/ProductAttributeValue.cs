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
            StringValue = null;
            NumericValue = null;
            BoolValue = null;

            if (string.IsNullOrWhiteSpace(raw))
                return;

            raw = raw.Trim();

            switch (def.DataType)
            {
                case AttributeDataType.String:
                    StringValue = raw;
                    return;

                case AttributeDataType.Integer:
                    if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                        throw new FormatException($"Cannot parse '{raw}' as int for attribute '{def.Name}'.");
                    NumericValue = i;
                    return;

                case AttributeDataType.Decimal:
                    // лучше не Replace(",", "."), а парсить и InvariantCulture и текущую культуру при необходимости
                    if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
                    {
                        // fallback на текущую культуру, если пользователь вводит "15,2" в ru-RU
                        if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out d))
                            throw new FormatException($"Cannot parse '{raw}' as decimal for attribute '{def.Name}'.");
                    }
                    NumericValue = d;
                    return;

                case AttributeDataType.Boolean:
                    var n = raw.Trim().ToLowerInvariant();
                    BoolValue = n is "1" or "true" or "yes" or "да";
                    return;

                default:
                    throw new ArgumentOutOfRangeException(nameof(def.DataType), $"Unsupported type {def.DataType}");
            }
        }
    }
}
