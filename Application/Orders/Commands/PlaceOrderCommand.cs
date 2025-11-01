using MediatR;
using static Application.Orders.Commands.PlaceOrderRequest;

namespace Application.Orders.Commands
{

    public sealed record PlaceOrderLine(
        Guid ProductId,
        string Sku,
        string Name,
        decimal UnitPrice,
        int Quantity
    );

    public sealed record PlaceOrderCommand(
        Guid CustomerId,
        string CustomerEmail,
        string? CustomerName,
        string? CustomerPhone,
        string? ShippingAddress,
        string? PaymentMethod,
        string? DeliveryMethod,
        decimal? ShippingFee,
        IReadOnlyList<PlaceOrderLine> Lines
    ) : IRequest<OrderAcceptedDto>;

    public sealed record OrderAcceptedDto(Guid OrderId, string Number, string Message);

    public sealed record PlaceOrderRequest(
    Guid CustomerId,
    string CustomerEmail,
    string? CustomerName,
    string? CustomerPhone,
    string? ShippingAddress,
    string? PaymentMethod,
    string? DeliveryMethod,
    decimal? ShippingFee,
    IReadOnlyList<Line> Lines)
    {
        public sealed record Line(Guid ProductId, string Sku, string Name, decimal UnitPrice, int Quantity);
    }
}
