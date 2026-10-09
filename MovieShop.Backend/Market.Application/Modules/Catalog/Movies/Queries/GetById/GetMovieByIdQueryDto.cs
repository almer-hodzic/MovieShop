namespace Market.Application.Modules.Catalog.Movies.Queries.GetById;

public sealed class GetMovieByIdCategoryDto
{
    public required int CategoryId { get; init; }
    public required string CategoryName { get; init; }
}

public sealed class GetMovieByIdActorDto
{
    public required int ActorId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string CharacterName { get; init; }
}

public sealed class GetMovieByIdQueryDto
{
    public required int Id { get; init; }
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
    public IReadOnlyList<GetMovieByIdCategoryDto> Categories { get; set; } = new List<GetMovieByIdCategoryDto>();
    public IReadOnlyList<GetMovieByIdActorDto> Actors { get; set; } = new List<GetMovieByIdActorDto>();
}