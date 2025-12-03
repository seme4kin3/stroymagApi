
namespace Domain.Catalog
{
    public class MeasurementUnit
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>Человекочитаемое название: "Штука", "Метр", "Килограмм".</summary>
        public string Name { get; private set; }

        /// <summary>Короткий символ: "шт", "м", "кг".</summary>
        public string Symbol { get; private set; }

        /// <summary>Код: "PCS", "M", "KG".</summary>
        public string? Code { get; private set; }

        public bool IsActive { get; private set; } = true;

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; private set; } = new List<CategoryAttribute>();
        public virtual ICollection<Product> Products { get; private set; } = new List<Product>();


        private MeasurementUnit() { }

        public MeasurementUnit(string name, string symbol, string? code = null)
        {
            SetName(name);
            SetSymbol(symbol);
            SetCode(code);
        }

        public void Rename(string name) => SetName(name);
        public void ChangeSymbol(string symbol) => SetSymbol(symbol);
        public void ChangeCode(string? code) => SetCode(code);

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Unit name is required", nameof(name));
            Name = name.Trim();
        }

        private void SetSymbol(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Unit symbol is required", nameof(symbol));
            Symbol = symbol.Trim();
        }

        private void SetCode(string? code)
        {
            Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim();
        }
    }
}
