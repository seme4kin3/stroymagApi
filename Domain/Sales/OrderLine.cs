
namespace Domain.Sales
{
    public class OrderLine
    {
        public int Id { get; private set; } // PK в таблице строк
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string Name { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Qty { get; private set; }
        public decimal LineTotal => UnitPrice * Qty;

        private OrderLine() { }
        public OrderLine(Guid productId, string name, decimal unitPrice, decimal qty)
        {
            ProductId = productId; Name = name; UnitPrice = unitPrice; Qty = qty;
        }
    }
}
