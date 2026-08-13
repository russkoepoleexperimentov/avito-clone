using FluentValidation;

namespace ResalePlatform.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Slug)
            .NotEmpty().MaximumLength(100)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug: только строчные латинские буквы, цифры и дефисы.");
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
