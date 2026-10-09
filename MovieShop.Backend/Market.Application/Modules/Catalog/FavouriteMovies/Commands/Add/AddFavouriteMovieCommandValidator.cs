namespace Market.Application.Modules.Catalog.FavouriteMovies.Commands.Add;

public sealed class AddFavouriteMovieCommandValidator : AbstractValidator<AddFavouriteMovieCommand>
{
    public AddFavouriteMovieCommandValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("MovieId must be greater than 0.");
    }
}
