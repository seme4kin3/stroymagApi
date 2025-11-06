
namespace Application.Products.DTOs
{
    public sealed record PriceDto(decimal Amount, decimal? OldAmount, string Currency = "RUB");
}
