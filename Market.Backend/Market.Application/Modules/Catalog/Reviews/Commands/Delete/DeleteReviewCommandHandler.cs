namespace Market.Application.Modules.Catalog.Reviews.Commands.Delete;

public sealed class DeleteReviewCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteReviewCommand>
{
    public async Task Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var review = await ctx.Reviews
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Review with Id {request.Id} not found.");

        if (review.UserId != userId && !currentUser.IsAdmin)
            throw new MarketBusinessRuleException("reviews.forbidden", "You can only delete your own reviews.");

        var movie = await ctx.Movies
            .FirstOrDefaultAsync(x => x.Id == review.MovieId, ct);

        ctx.Reviews.Remove(review);
        await ctx.SaveChangesAsync(ct);

        if (movie is not null)
        {
            await RecalculateMovieAverageScoreAsync(movie, ct);
            await ctx.SaveChangesAsync(ct);
        }
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
