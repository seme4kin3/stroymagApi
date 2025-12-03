using Application.Admin.MeasurementUnit.Commands;
using FluentValidation;


namespace Application.Admin.MeasurementUnit.Validators
{
    public sealed class CreateMeasurementUnitValidator : AbstractValidator<CreateMeasurementUnitCommand>
    {
        public CreateMeasurementUnitValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Symbol).NotEmpty().MaximumLength(50);
        }
    }
}
