namespace Market.Application.Modules.Profile.Commands.ChangeEmail;

public sealed class ChangeMyEmailCommandValidator : AbstractValidator<ChangeMyEmailCommand>
{
    public ChangeMyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is not valid.")
            .MaximumLength(200).WithMessage("Email can be at most 200 characters long.");
    }
}

