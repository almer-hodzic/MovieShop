namespace Market.Application.Modules.Catalog.FavouriteMovies.Commands.Remove;

public sealed class RemoveFavouriteMovieCommandValidator : AbstractValidator<RemoveFavouriteMovieCommand>
{
    public RemoveFavouriteMovieCommandValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("MovieId must be greater than 0.");
    }
}
