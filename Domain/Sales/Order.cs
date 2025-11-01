
namespace Domain.Sales
{
    public class Order
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Number { get; private set; } = GenerateNumber();
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Placed;

        public string? ShippingAddress { get; private set; } 
        public string? BillingAddress { get; private set; }

        public decimal Subtotal { get; private set; }
        public decimal Discount { get; private set; }
        public decimal Total { get; private set; }

        public List<OrderLine> Lines { get; private set; } = new();

        private Order() { }
        public Order(Guid customerId, IEnumerable<(Guid productId, string name, decimal price, decimal qty)> lines,
                     string? shippingAddress, string? billingAddress, decimal discount = 0)
        {
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;

            foreach (var l in lines)
                AddLine(l.productId, l.name, l.price, l.qty);

            ApplyDiscount(discount);
        }

        public void AddLine(Guid productId, string name, decimal unitPrice, decimal qty)
        {
            Lines.Add(new OrderLine(productId, name, unitPrice, qty));
            Recalculate();
        }

        public void ApplyDiscount(decimal discount)
        {
            Discount = discount < 0 ? 0 : discount;
            Recalculate();
        }

        public void MarkPaid() => Status = OrderStatus.Paid;
        public void MarkShipped() => Status = OrderStatus.Shipped;
        public void MarkCompleted() => Status = OrderStatus.Completed;
        public void Cancel(string? reason = null) => Status = OrderStatus.Cancelled;

        private void Recalculate()
        {
            Subtotal = Lines.Sum(l => l.LineTotal);
            Total = Math.Max(0, Subtotal - Discount);
        }

        private static string GenerateNumber() => $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        public static Order Create(
            Guid customerId,
            string customerEmail,   
            string? customerName,   
            string? customerPhone,  
            string? shippingAddress,
            string? paymentMethod,  
            string? deliveryMethod, 
            decimal shippingFee     
        )
        {
            if (customerEmail == string.Empty)
                throw new ArgumentException("CustomerId is required.", nameof(customerId));

            // В текущей модели заказа нет BillingAddress/ShippingFee/PaymentMethod/DeliveryMethod —
            // сохраняем только то, что поддерживает агрегат.
            return new Order(
                customerId: customerId,
                lines: Array.Empty<(Guid productId, string name, decimal price, decimal qty)>(),
                shippingAddress: shippingAddress,
                billingAddress: null
            );
        }

        public void AddItem(Guid productId, string sku, string name, decimal unitPrice, int quantity)
        {
            if (productId == Guid.Empty) throw new ArgumentException("productId is required.", nameof(productId));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is required.", nameof(name));
            if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "unitPrice must be >= 0");
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "quantity must be > 0");

            // SKU в текущей модели не хранится — параметр принимаем для совместимости с командой/handler.
            AddLine(productId, name, unitPrice, qty: quantity);
        }
    }
}
