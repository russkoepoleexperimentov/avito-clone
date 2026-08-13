using FluentValidation;

namespace ResalePlatform.Application.Features.Listings.Commands.UpdateListing;

public class UpdateListingValidator : AbstractValidator<UpdateListingCommand>
{
    public UpdateListingValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Condition).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.City).NotEmpty().MaximumLength(80);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
