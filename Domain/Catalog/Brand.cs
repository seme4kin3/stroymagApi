
namespace Domain.Catalog
{
    public class Brand
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public ICollection<Product> Products { get; private set; } = new List<Product>();

        private Brand() { }
        public Brand(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Brand name required") : name.Trim();
        }
    }
}
