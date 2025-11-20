using Application.Admin.Attributes.Commands;
using FluentValidation;

namespace Application.Admin.Attributes.Validators
{
    public sealed class UpdateAttributeDefinitionValidator
        : AbstractValidator<UpdateAttributeDefinitionCommand>
    {
        public UpdateAttributeDefinitionValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Key)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.DataType)
                .IsInEnum();

            RuleFor(x => x.Unit)
                .MaximumLength(50)
                .When(x => x.Unit != null);
        }
    }
}
