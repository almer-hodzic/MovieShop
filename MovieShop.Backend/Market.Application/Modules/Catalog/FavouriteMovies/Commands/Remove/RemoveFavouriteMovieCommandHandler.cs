namespace Market.Application.Modules.Catalog.FavouriteMovies.Commands.Remove;

public sealed class RemoveFavouriteMovieCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<RemoveFavouriteMovieCommand>
{
    public async Task Handle(RemoveFavouriteMovieCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketUnauthorizedException("Authenticated user is required.");

        var entity = await ctx.FavouriteMovies
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == request.MovieId, ct)
            ?? throw new MarketNotFoundException($"Movie with Id {request.MovieId} is not in favourites.");

        ctx.FavouriteMovies.Remove(entity);
        await ctx.SaveChangesAsync(ct);
    }
}
