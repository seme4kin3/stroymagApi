using Application.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public sealed class OrdersController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Place([FromBody] PlaceOrderRequest body, CancellationToken ct)
        {
            var cmd = new PlaceOrderCommand(
                body.CustomerId,
                body.CustomerEmail,
                body.CustomerName,
                body.CustomerPhone,
                body.ShippingAddress,
                body.PaymentMethod,
                body.DeliveryMethod,
                body.ShippingFee,
                body.Lines.Select(l => new PlaceOrderLine(l.ProductId, l.Sku, l.Name, l.UnitPrice, l.Quantity)).ToList()
            );

            var result = await mediator.Send(cmd, ct);
            return Accepted(new { result.OrderId, result.Number, message = result.Message });
        }
    }
}
