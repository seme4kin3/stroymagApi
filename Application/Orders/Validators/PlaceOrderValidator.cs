using Application.Orders.Commands;
using FluentValidation;

namespace Application.Orders.Validators
{
    public sealed class PlaceOrderValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderValidator()
        {
            //RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.Lines).NotEmpty();

            RuleForEach(x => x.Lines).ChildRules(l =>
            {
                l.RuleFor(i => i.ProductId).NotEmpty();
                l.RuleFor(i => i.Sku).NotEmpty().MaximumLength(64);
                l.RuleFor(i => i.Name).NotEmpty().MaximumLength(512);
                l.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
                l.RuleFor(i => i.Quantity).GreaterThan(0);
            });

            RuleFor(x => x.ShippingFee).GreaterThanOrEqualTo(0).When(x => x.ShippingFee.HasValue);
        }
    }
}
