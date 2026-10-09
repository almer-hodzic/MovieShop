namespace Market.Application.Modules.Catalog.Directors.Commands.Create;

public sealed class CreateDirectorCommandValidator : AbstractValidator<CreateDirectorCommand>
{
    public CreateDirectorCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("First name is required.")
            .MaximumLength(DirectorEntity.Constraints.FirstNameMaxLength)
            .WithMessage($"First name can be at most {DirectorEntity.Constraints.FirstNameMaxLength} characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Last name is required.")
            .MaximumLength(DirectorEntity.Constraints.LastNameMaxLength)
            .WithMessage($"Last name can be at most {DirectorEntity.Constraints.LastNameMaxLength} characters long.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow.Date).WithMessage("Birth date must be in the past.");
    }
}