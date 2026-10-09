namespace Market.Application.Modules.Catalog.Movies.Queries.GetById;

public sealed class GetMovieByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetMovieByIdQuery, GetMovieByIdQueryDto>
{
    public async Task<GetMovieByIdQueryDto> Handle(GetMovieByIdQuery request, CancellationToken ct)
    {
        var movie = await ctx.Movies
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetMovieByIdQueryDto
            {
                Id = x.Id,
                Title = x.Title,
                ReleaseDate = x.ReleaseDate,
                CreationDate = x.CreationDate,
                Duration = x.Duration,
                DirectorId = x.DirectorId,
                DirectorName = x.Director != null ? (x.Director.FirstName + " " + x.Director.LastName) : string.Empty,
                CountryId = x.CountryId,
                TrailerLink = x.TrailerLink,
                Image = x.Image,
                StoryLine = x.StoryLine,
                Price = x.Price,
                AverageScore = x.AverageScore,
            })
            .FirstOrDefaultAsync(ct);

        if (movie is null)
            throw new MarketNotFoundException($"Movie with Id {request.Id} not found.");

        movie.Categories = await ctx.MovieCategories
            .AsNoTracking()
            .Where(x => x.MovieId == request.Id)
            .Select(x => new GetMovieByIdCategoryDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.Category != null ? x.Category.CategoryName : string.Empty,
            })
            .ToListAsync(ct);

        movie.Actors = await ctx.MovieActors
            .AsNoTracking()
            .Where(x => x.MovieId == request.Id)
            .Select(x => new GetMovieByIdActorDto
            {
                ActorId = x.ActorId,
                FirstName = x.Actor != null ? x.Actor.FirstName : string.Empty,
                LastName = x.Actor != null ? x.Actor.LastName : string.Empty,
                CharacterName = x.CharacterName,
            })
            .ToListAsync(ct);

        return movie;
    }
}