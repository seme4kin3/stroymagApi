using Application.Abstractions;
using Application.Orders.Commands;
using Domain.Sales;
using MediatR;


namespace Application.Orders.Handlers
{
    public sealed class PlaceOrderHandler(IOrderRepository orders)
        : IRequestHandler<PlaceOrderCommand, OrderAcceptedDto>
    {
        public async Task<OrderAcceptedDto> Handle(PlaceOrderCommand request, CancellationToken ct)
        {
            
            var order = Order.Create(
                customerId: request.CustomerId,
                customerEmail: request.CustomerEmail,
                customerName: request.CustomerName,
                customerPhone: request.CustomerPhone,
                shippingAddress: request.ShippingAddress,
                paymentMethod: request.PaymentMethod,
                deliveryMethod: request.DeliveryMethod,
                shippingFee: request.ShippingFee ?? 0m
            );

            foreach (var l in request.Lines)
                order.AddItem(l.ProductId, l.Sku, l.Name, l.UnitPrice, l.Quantity);

            await orders.AddAsync(order, ct);
            await orders.SaveChangesAsync(ct);

            return new OrderAcceptedDto(order.Id, order.Number, "Заказ принят в обработку");
        }
    }
}
