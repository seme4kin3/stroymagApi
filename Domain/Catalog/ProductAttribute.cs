
namespace Domain.Catalog
{
    public class ProductAttribute
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ProductId { get; private set; } = default!; 
        public string Key { get; private set; }
        public string Value { get; private set; }

        private ProductAttribute() { }
        public ProductAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        internal void BindProduct(Guid productId) => ProductId = productId;
        internal void Update(string value) => Value = value;
    }
}
