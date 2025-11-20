using Application.Admin.Brands.Commands;
using FluentValidation;

namespace Application.Admin.Brands.Validators
{
    public sealed class UpdateBrandValidator : AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);
        }

    }
}
