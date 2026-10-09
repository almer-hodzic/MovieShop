namespace Market.Application.Modules.Catalog.FavouriteMovies.Commands.Add;

public sealed class AddFavouriteMovieCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<AddFavouriteMovieCommand, int>
{
    public async Task<int> Handle(AddFavouriteMovieCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketUnauthorizedException("Authenticated user is required.");

        var userExists = await ctx.Users.AnyAsync(x => x.Id == userId, ct);
        if (!userExists)
            throw new MarketNotFoundException($"User with Id {userId} not found.");

        var movieExists = await ctx.Movies.AnyAsync(x => x.Id == request.MovieId, ct);
        if (!movieExists)
            throw new MarketNotFoundException($"Movie with Id {request.MovieId} not found.");

        var existingFavourite = await ctx.FavouriteMovies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.MovieId == request.MovieId,
                ct);

        if (existingFavourite is { IsDeleted: false })
            throw new MarketConflictException($"Movie with Id {request.MovieId} is already in favourites.");

        if (existingFavourite is { IsDeleted: true })
        {
            existingFavourite.IsDeleted = false;
            existingFavourite.DateAdded = DateTime.UtcNow;
            await ctx.SaveChangesAsync(ct);

            return existingFavourite.Id;
        }

        var entity = new FavouriteMovieEntity
        {
            UserId = userId,
            MovieId = request.MovieId,
            DateAdded = DateTime.UtcNow,
        };

        ctx.FavouriteMovies.Add(entity);
        await ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}
