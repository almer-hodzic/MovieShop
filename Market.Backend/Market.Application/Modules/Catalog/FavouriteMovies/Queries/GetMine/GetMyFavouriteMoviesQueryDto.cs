namespace Market.Application.Modules.Catalog.FavouriteMovies.Queries.GetMine;

public sealed class GetMyFavouriteMoviesQueryDto
{
    public required int FavouriteMovieId { get; init; }
    public required int MovieId { get; init; }
    public required DateTime DateAdded { get; init; }
    public required string Title { get; init; }
    public required DateTime ReleaseDate { get; init; }
    public required DateTime CreationDate { get; init; }
    public required int Duration { get; init; }
    public required int DirectorId { get; init; }
    public required string DirectorName { get; init; }
    public required int CountryId { get; init; }
    public string? TrailerLink { get; init; }
    public byte[]? Image { get; init; }
    public required string StoryLine { get; init; }
    public required decimal Price { get; init; }
    public decimal? AverageScore { get; init; }
}
