namespace Market.Application.Modules.Catalog.Movies.Commands.Delete;

public sealed class DeleteMovieCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteMovieCommand>
{
    public async Task Handle(DeleteMovieCommand request, CancellationToken ct)
    {
        var movie = await ctx.Movies
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Movie with Id {request.Id} not found.");

        var movieCategories = await ctx.MovieCategories
            .Where(x => x.MovieId == request.Id)
            .ToListAsync(ct);
        ctx.MovieCategories.RemoveRange(movieCategories);

        var movieActors = await ctx.MovieActors
            .Where(x => x.MovieId == request.Id)
            .ToListAsync(ct);
        ctx.MovieActors.RemoveRange(movieActors);

        ctx.Movies.Remove(movie);

        await ctx.SaveChangesAsync(ct);
    }
}