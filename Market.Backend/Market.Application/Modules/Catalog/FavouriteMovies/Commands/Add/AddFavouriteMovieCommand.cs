namespace Market.Application.Modules.Catalog.FavouriteMovies.Commands.Add;

public sealed class AddFavouriteMovieCommand : IRequest<int>
{
    public int MovieId { get; init; }
}
