namespace Market.Application.Modules.Catalog.FavouriteMovies.Queries.GetMine;

public sealed class GetMyFavouriteMoviesQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyFavouriteMoviesQuery, IReadOnlyList<GetMyFavouriteMoviesQueryDto>>
{
    public async Task<IReadOnlyList<GetMyFavouriteMoviesQueryDto>> Handle(GetMyFavouriteMoviesQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketUnauthorizedException("Authenticated user is required.");

        return await ctx.FavouriteMovies
            .AsNoTracking()
            .Include(x => x.Movie)
            .ThenInclude(x => x!.Director)
            .Where(x => x.UserId == userId && x.Movie != null)
            .OrderByDescending(x => x.DateAdded)
            .Select(x => new GetMyFavouriteMoviesQueryDto
            {
                FavouriteMovieId = x.Id,
                MovieId = x.MovieId,
                DateAdded = x.DateAdded,
                Title = x.Movie!.Title,
                ReleaseDate = x.Movie.ReleaseDate,
                CreationDate = x.Movie.CreationDate,
                Duration = x.Movie.Duration,
                DirectorId = x.Movie.DirectorId,
                DirectorName = x.Movie.Director != null
                    ? (x.Movie.Director.FirstName + " " + x.Movie.Director.LastName)
                    : string.Empty,
                CountryId = x.Movie.CountryId,
                TrailerLink = x.Movie.TrailerLink,
                Image = x.Movie.Image,
                StoryLine = x.Movie.StoryLine,
                Price = x.Movie.Price,
                AverageScore = x.Movie.AverageScore,
            })
            .ToListAsync(ct);
    }
}
