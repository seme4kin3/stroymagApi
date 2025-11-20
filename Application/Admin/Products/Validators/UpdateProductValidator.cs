using Application.Admin.Products.Commands;
using FluentValidation;

namespace Application.Admin.Products.Validators
{
    public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Article).MaximumLength(128).When(x => x.Article != null);
        }
    }
}
