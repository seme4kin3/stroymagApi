
namespace Domain.Catalog
{
    public class AttributeDefinition
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; }      // "Мощность"
        public string Key { get; private set; }       // "power"
        public AttributeDataType DataType { get; private set; }

        /// <summary>Единица измерения: "Вт", "мм", "кг" и т.п.</summary>
        public string? Unit { get; private set; }

        /// <summary>Активен ли атрибут (мягкое удаление).</summary>
        public bool IsActive { get; private set; } = true;

        private AttributeDefinition() { }

        public AttributeDefinition(string name, string key, AttributeDataType dataType, string? unit)
        {
            SetName(name);
            SetKey(key);
            DataType = dataType;
            SetUnit(unit);
        }

        public void Rename(string name) => SetName(name);

        public void ChangeKey(string key) => SetKey(key);

        public void ChangeType(AttributeDataType dataType) => DataType = dataType;

        public void ChangeUnit(string? unit) => SetUnit(unit);

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Attribute name is required", nameof(name));
            Name = name.Trim();
        }

        private void SetKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Attribute key is required", nameof(key));
            Key = key.Trim().ToLowerInvariant();
        }

        private void SetUnit(string? unit)
        {
            Unit = string.IsNullOrWhiteSpace(unit) ? null : unit.Trim();
        }
    }
}
