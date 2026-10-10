namespace Market.Application.Modules.Catalog.Reviews.Queries.GetById;

public sealed class GetReviewByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetReviewByIdQuery, GetReviewByIdQueryDto>
{
    public async Task<GetReviewByIdQueryDto> Handle(GetReviewByIdQuery request, CancellationToken ct)
    {
        var review = await ctx.Reviews
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetReviewByIdQueryDto
            {
                Id = x.Id,
                ReviewDate = x.ReviewDate,
                Score = x.Score,
                Comment = x.Comment,
                UserId = x.UserId,
                MovieId = x.MovieId,
                UserName = x.User != null ? x.User.Username : string.Empty,
            })
            .FirstOrDefaultAsync(ct);

        if (review is null)
            throw new MarketNotFoundException($"Review with Id {request.Id} not found.");

        return review;
    }
}
