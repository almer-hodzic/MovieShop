namespace Market.Application.Modules.Catalog.Reviews.Queries.GetByMovieId;

public sealed class GetReviewsByMovieIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetReviewsByMovieIdQuery, PageResult<GetReviewsByMovieIdQueryDto>>
{
    public async Task<PageResult<GetReviewsByMovieIdQueryDto>> Handle(GetReviewsByMovieIdQuery request, CancellationToken ct)
    {
        var q = ctx.Reviews
            .AsNoTracking()
            .Where(x => x.MovieId == request.MovieId);

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            var term = request.UserName.Trim();

            q = q.Where(x => x.User != null &&
                (
                    (x.User.Firstname + " " + x.User.Lastname).Contains(term) ||
                    x.User.Email.Contains(term)
                ));
        }

        var projectedQuery = q
            .OrderByDescending(x => x.ReviewDate)
            .Select(x => new GetReviewsByMovieIdQueryDto
            {
                Id = x.Id,
                ReviewDate = x.ReviewDate,
                Score = x.Score,
                Comment = x.Comment,
                UserId = x.UserId,
                MovieId = x.MovieId,
                UserName = x.User != null ? x.User.Email : string.Empty,
            });

        return await PageResult<GetReviewsByMovieIdQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
