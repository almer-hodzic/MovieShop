namespace Market.Application.Modules.Auth.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
{
    public VerifyTwoFactorCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User id must be greater than zero.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Matches("^[0-9]{6}$").WithMessage("Verification code must contain exactly six digits.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.Fingerprint)
            .MaximumLength(256).WithMessage("Fingerprint can be up to 256 characters long.")
            .When(x => x.Fingerprint is not null);
    }
}
