namespace Market.Application.Modules.Catalog.Movies.Commands.Update;

public sealed class UpdateMovieCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateMovieCommand>
{
    public async Task Handle(UpdateMovieCommand request, CancellationToken ct)
    {
        var movie = await ctx.Movies
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Movie with Id {request.Id} not found.");

        var directorExists = await ctx.Directors.AnyAsync(x => x.Id == request.DirectorId, ct);
        if (!directorExists)
            throw new MarketNotFoundException($"Director with Id {request.DirectorId} not found.");

        var distinctCategoryIds = request.Categories.Distinct().ToList();
        var categoriesCount = await ctx.Categories
            .CountAsync(x => distinctCategoryIds.Contains(x.Id), ct);
        if (categoriesCount != distinctCategoryIds.Count)
            throw new MarketNotFoundException("One or more categories do not exist.");

        var distinctActorIds = request.Actors.Select(x => x.ActorId).Distinct().ToList();
        var actorsCount = await ctx.Actors
            .CountAsync(x => distinctActorIds.Contains(x.Id), ct);
        if (actorsCount != distinctActorIds.Count)
            throw new MarketNotFoundException("One or more actors do not exist.");

        movie.Title = request.Title.Trim();
        movie.ReleaseDate = request.ReleaseDate;
        movie.Duration = request.Duration;
        movie.DirectorId = request.DirectorId;
        movie.CountryId = request.CountryId;
        movie.TrailerLink = request.TrailerLink.Trim();
        movie.Image = DecodeBase64(request.ImageBase64);
        movie.StoryLine = request.StoryLine.Trim();
        movie.Price = request.Price;

        var movieCategories = await ctx.MovieCategories
            .Where(x => x.MovieId == request.Id)
            .ToListAsync(ct);
        ctx.MovieCategories.RemoveRange(movieCategories);

        var movieActors = await ctx.MovieActors
            .Where(x => x.MovieId == request.Id)
            .ToListAsync(ct);
        ctx.MovieActors.RemoveRange(movieActors);

        foreach (var categoryId in distinctCategoryIds)
        {
            ctx.MovieCategories.Add(new MovieCategoryEntity
            {
                MovieId = request.Id,
                CategoryId = categoryId,
            });
        }

        foreach (var actor in request.Actors.GroupBy(x => x.ActorId).Select(x => x.First()))
        {
            ctx.MovieActors.Add(new MovieActorEntity
            {
                MovieId = request.Id,
                ActorId = actor.ActorId,
                CharacterName = actor.CharacterName.Trim(),
            });
        }

        await ctx.SaveChangesAsync(ct);
    }

    private static byte[] DecodeBase64(string input)
    {
        try
        {
            var base64 = input;
            var commaIndex = input.IndexOf(',');
            if (commaIndex >= 0)
                base64 = input[(commaIndex + 1)..];

            return Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            throw new MarketConflictException("Invalid image format.");
        }
    }
}