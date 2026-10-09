namespace Market.Application.Modules.Catalog.Movies.Queries.List;

public sealed class ListMoviesQuery : BasePagedQuery<ListMoviesQueryDto>
{
    public string? Title { get; init; }
    public int? CategoryId { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public string? SortedColumn { get; init; }
    public bool? IsDescending { get; init; }
}