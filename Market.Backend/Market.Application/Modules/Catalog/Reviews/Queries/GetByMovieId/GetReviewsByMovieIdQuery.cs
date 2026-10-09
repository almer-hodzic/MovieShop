namespace Market.Application.Modules.Catalog.Reviews.Queries.GetByMovieId;

public sealed class GetReviewsByMovieIdQuery : BasePagedQuery<GetReviewsByMovieIdQueryDto>
{
    public int MovieId { get; init; }
    public string? UserName { get; init; }
}
