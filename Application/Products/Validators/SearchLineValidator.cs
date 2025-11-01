using Application.Products.Queries;
using FluentValidation;


namespace Application.Products.Validators
{
    public sealed class SearchLineValidator : AbstractValidator<SearchLineQuery>
    {
        public SearchLineValidator()
        {
            RuleFor(x => x.Q).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        }
    }
}
