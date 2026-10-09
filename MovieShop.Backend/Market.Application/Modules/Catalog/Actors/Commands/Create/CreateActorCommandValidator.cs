namespace Market.Application.Modules.Catalog.Actors.Commands.Create;

public sealed class CreateActorCommandValidator : AbstractValidator<CreateActorCommand>
{
    public CreateActorCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("First name is required.")
            .MaximumLength(ActorEntity.Constraints.FirstNameMaxLength)
            .WithMessage($"First name can be at most {ActorEntity.Constraints.FirstNameMaxLength} characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Last name is required.")
            .MaximumLength(ActorEntity.Constraints.LastNameMaxLength)
            .WithMessage($"Last name can be at most {ActorEntity.Constraints.LastNameMaxLength} characters long.");

        RuleFor(x => x.PhotoBase64)
            .NotEmpty().WithMessage("Photo is required.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow.Date).WithMessage("Birth date must be in the past.");

        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("CountryId must be greater than 0.");

        RuleFor(x => x.ImdbLink)
            .NotEmpty().WithMessage("IMDb link is required.")
            .MaximumLength(ActorEntity.Constraints.ImdbLinkMaxLength)
            .WithMessage($"IMDb link can be at most {ActorEntity.Constraints.ImdbLinkMaxLength} characters long.");

        RuleFor(x => x.Biography)
            .NotEmpty().WithMessage("Biography is required.")
            .MaximumLength(ActorEntity.Constraints.BiographyMaxLength)
            .WithMessage($"Biography can be at most {ActorEntity.Constraints.BiographyMaxLength} characters long.");
    }
}