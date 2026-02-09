using Application.Admin.Categories.Commands;
using FluentValidation;


namespace Application.Admin.Categories.Validators
{
    public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).MaximumLength(200).When(x => x.Slug != null);

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
