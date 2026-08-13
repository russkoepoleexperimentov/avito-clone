using FluentValidation;

namespace ResalePlatform.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Slug)
            .NotEmpty().MaximumLength(100)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug: только строчные латинские буквы, цифры и дефисы.");
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
