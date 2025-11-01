
namespace Domain.Sales
{
    public class Cart
    {
        public Guid Id { get; private set; } // = CustomerId (PK = FK)
        public List<CartItem> Items { get; private set; } = new();

        private Cart() { }
        public Cart(Guid customerId) { Id = customerId; }

        public void AddItem(Guid productId, string name, decimal price, decimal qty = 1)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing is null) Items.Add(new CartItem(productId, name, price, qty));
            else existing.Increase(qty);
        }
        public void RemoveItem(Guid productId) => Items.RemoveAll(i => i.ProductId == productId);
        public void UpdateQty(Guid productId, decimal qty)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return;
            item.SetQty(qty);
            if (item.Qty <= 0) RemoveItem(productId);
        }
        public decimal Total() => Items.Sum(i => i.LineTotal());
    }

    public class CartItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ProductId { get; private set; }
        public string Name { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Qty { get; private set; }

        private CartItem() { }
        public CartItem(Guid productId, string name, decimal unitPrice, decimal qty)
        {
            ProductId = productId; Name = name; UnitPrice = unitPrice; Qty = qty;
        }

        internal void Increase(decimal delta) => Qty += delta;
        internal void SetQty(decimal qty) => Qty = qty;
        public decimal LineTotal() => UnitPrice * Qty;
    }
}
