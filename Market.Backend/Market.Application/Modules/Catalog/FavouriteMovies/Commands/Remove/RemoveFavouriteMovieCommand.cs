namespace Market.Application.Modules.Catalog.FavouriteMovies.Commands.Remove;

public sealed class RemoveFavouriteMovieCommand : IRequest
{
    public int MovieId { get; init; }
}
