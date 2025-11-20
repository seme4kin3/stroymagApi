using Application.Admin.Categories.Commands;
using FluentValidation;


namespace Application.Admin.Categories.Validators
{
    public sealed class CreateCategoryWithAttributesValidator
        : AbstractValidator<CreateCategoryWithAttributesCommand>
    {
        public CreateCategoryWithAttributesValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).MaximumLength(200).When(x => x.Slug != null);
            RuleFor(x => x.ImageUrl).MaximumLength(500).When(x => x.ImageUrl != null);

            RuleFor(x => x.Attributes)
                .NotNull()
                .NotEmpty().WithMessage("Категория должна иметь хотя бы один атрибут.");

            RuleForEach(x => x.Attributes).ChildRules(r =>
            {
                r.RuleFor(a => a.AttributeDefinitionId).NotEmpty();
                r.RuleFor(a => a.SortOrder).GreaterThanOrEqualTo(0);
            });
        }
    }
}
