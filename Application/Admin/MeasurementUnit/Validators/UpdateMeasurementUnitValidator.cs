using Application.Admin.MeasurementUnit.Commands;
using FluentValidation;


namespace Application.Admin.MeasurementUnit.Validators
{
    public sealed class UpdateMeasurementUnitValidator : AbstractValidator<UpdateMeasurementUnitCommand>
    {
        public UpdateMeasurementUnitValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Symbol).NotEmpty().MaximumLength(50);
        }
    }
}
