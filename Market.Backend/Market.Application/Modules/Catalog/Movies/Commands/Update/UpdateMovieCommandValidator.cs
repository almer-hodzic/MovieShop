namespace Market.Application.Modules.Catalog.Movies.Commands.Update;

public sealed class UpdateMovieCommandValidator : AbstractValidator<UpdateMovieCommand>
{
    public UpdateMovieCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(MovieEntity.Constraints.TitleMaxLength)
            .WithMessage($"Title can be at most {MovieEntity.Constraints.TitleMaxLength} characters long.");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty().WithMessage("Release date is required.");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.");

        RuleFor(x => x.DirectorId)
            .GreaterThan(0).WithMessage("DirectorId must be greater than 0.");

        RuleFor(x => x.Categories)
            .NotNull().WithMessage("Categories are required.")
            .Must(x => x.Count > 0).WithMessage("At least one category is required.");

        RuleFor(x => x.Actors)
            .NotNull().WithMessage("Actors are required.")
            .Must(x => x.Count > 0).WithMessage("At least one actor is required.");

        RuleForEach(x => x.Actors).ChildRules(actor =>
        {
            actor.RuleFor(a => a.ActorId)
                .GreaterThan(0).WithMessage("ActorId must be greater than 0.");

            actor.RuleFor(a => a.CharacterName)
                .NotEmpty().WithMessage("Character name is required.")
                .MaximumLength(MovieActorEntity.Constraints.CharacterNameMaxLength)
                .WithMessage($"Character name can be at most {MovieActorEntity.Constraints.CharacterNameMaxLength} characters long.");
        });

        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("CountryId must be greater than 0.");

        RuleFor(x => x.TrailerLink)
            .NotEmpty().WithMessage("Trailer link is required.")
            .MaximumLength(MovieEntity.Constraints.TrailerLinkMaxLength)
            .WithMessage($"Trailer link can be at most {MovieEntity.Constraints.TrailerLinkMaxLength} characters long.");

        RuleFor(x => x.ImageBase64)
            .NotEmpty().WithMessage("Image is required.");

        RuleFor(x => x.StoryLine)
            .NotEmpty().WithMessage("Story line is required.")
            .MaximumLength(MovieEntity.Constraints.StoryLineMaxLength)
            .WithMessage($"Story line can be at most {MovieEntity.Constraints.StoryLineMaxLength} characters long.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}