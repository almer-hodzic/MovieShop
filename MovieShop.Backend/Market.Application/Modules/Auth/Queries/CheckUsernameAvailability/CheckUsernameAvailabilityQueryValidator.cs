namespace Market.Application.Modules.Auth.Queries.CheckUsernameAvailability;

public sealed class CheckUsernameAvailabilityQueryValidator : AbstractValidator<CheckUsernameAvailabilityQuery>
{
    public CheckUsernameAvailabilityQueryValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(100).WithMessage("Username can be at most 100 characters long.")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can contain letters, numbers, dots, underscores, and hyphens.");
    }
}
