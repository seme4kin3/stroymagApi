using Application.Admin.Brands.Commands;
using FluentValidation;


namespace Application.Admin.Brands.Validators
{
    public sealed class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}
