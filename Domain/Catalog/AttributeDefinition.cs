
namespace Domain.Catalog
{
    public class AttributeDefinition
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; }      // "Мощность"
        public string Key { get; private set; }       // "power"
        public AttributeDataType DataType { get; private set; }

        /// <summary>Активен ли атрибут (мягкое удаление).</summary>
        public bool IsActive { get; private set; } = true;
        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; private set; } = new List<CategoryAttribute>();
        public virtual ICollection<ProductAttributeValue> ProductValues { get; private set; } = new List<ProductAttributeValue>();

        private AttributeDefinition() { }

        public AttributeDefinition(string name, string key, AttributeDataType dataType)
        {
            SetName(name);
            SetKey(key);
            DataType = dataType;
        }

        public void Rename(string name) => SetName(name);

        public void ChangeKey(string key) => SetKey(key);

        public void ChangeType(AttributeDataType dataType) => DataType = dataType;


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
    }
}
