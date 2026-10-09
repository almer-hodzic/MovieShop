namespace Market.Application.Modules.Catalog.Reviews.Commands.Create;

public sealed class CreateReviewCommand : IRequest<int>
{
    public int MovieId { get; init; }
    public string Comment { get; init; } = string.Empty;
    public decimal Score { get; init; }
}
