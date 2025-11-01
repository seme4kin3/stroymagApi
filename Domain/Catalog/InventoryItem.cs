
namespace Domain.Catalog
{
    public class InventoryItem
    {
        public Guid ProductId { get; private set; } = default!; // PK = FK на Product
        public decimal Quantity { get; private set; }

        private InventoryItem() { }
        public InventoryItem(Guid productId, decimal quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public void Add(decimal qty) => Quantity += qty;
        public bool TryReserve(decimal qty)
        {
            if (qty <= 0 || Quantity < qty) return false;
            Quantity -= qty;
            return true;
        }
    }
}
