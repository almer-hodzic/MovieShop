namespace Market.Application.Modules.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("Firstname is required.")
            .MaximumLength(100).WithMessage("Firstname can be at most 100 characters long.");

        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Lastname is required.")
            .MaximumLength(100).WithMessage("Lastname can be at most 100 characters long.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.")
            .MaximumLength(200).WithMessage("Email can be at most 200 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100).WithMessage("Password can be at most 100 characters long.");
    }
}
