namespace Market.Application.Modules.Catalog.Directors.Commands.Update;

public sealed class UpdateDirectorCommandValidator : AbstractValidator<UpdateDirectorCommand>
{
    public UpdateDirectorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");

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