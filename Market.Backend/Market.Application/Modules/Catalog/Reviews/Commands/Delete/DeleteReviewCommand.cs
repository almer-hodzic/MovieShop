namespace Market.Application.Modules.Catalog.Reviews.Commands.Delete;

public sealed class DeleteReviewCommand : IRequest
{
    public int Id { get; init; }
}
