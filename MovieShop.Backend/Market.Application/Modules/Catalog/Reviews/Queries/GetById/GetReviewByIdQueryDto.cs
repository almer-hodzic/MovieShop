namespace Market.Application.Modules.Catalog.Reviews.Queries.GetById;

public sealed class GetReviewByIdQueryDto
{
    public required int Id { get; init; }
    public required DateTime ReviewDate { get; init; }
    public required decimal Score { get; init; }
    public required string Comment { get; init; }
    public required int UserId { get; init; }
    public required int MovieId { get; init; }
    public required string UserName { get; init; }
}
