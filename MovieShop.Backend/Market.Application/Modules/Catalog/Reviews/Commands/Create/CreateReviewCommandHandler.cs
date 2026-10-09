namespace Market.Application.Modules.Catalog.Reviews.Commands.Create;

public sealed class CreateReviewCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<CreateReviewCommand, int>
{
    public async Task<int> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var movie = await ctx.Movies
            .FirstOrDefaultAsync(x => x.Id == request.MovieId, ct)
            ?? throw new MarketNotFoundException($"Movie with Id {request.MovieId} not found.");

        var entity = new ReviewEntity
        {
            UserId = userId,
            MovieId = request.MovieId,
            Comment = request.Comment.Trim(),
            Score = request.Score,
            ReviewDate = DateTime.UtcNow,
        };

        ctx.Reviews.Add(entity);
        await ctx.SaveChangesAsync(ct);

        await RecalculateMovieAverageScoreAsync(movie, ct);
        await ctx.SaveChangesAsync(ct);

        return entity.Id;
    }

    private async Task RecalculateMovieAverageScoreAsync(MovieEntity movie, CancellationToken ct)
    {
        var reviewsQuery = ctx.Reviews
            .Where(x => x.MovieId == movie.Id)
            .Select(x => x.Score);

        var hasReviews = await reviewsQuery.AnyAsync(ct);
        if (!hasReviews)
        {
            movie.AverageScore = 0;
            return;
        }

        var averageScore = await reviewsQuery.AverageAsync(ct);
        movie.AverageScore = Math.Round(averageScore, 2);
    }
}
