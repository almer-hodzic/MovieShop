namespace Market.Application.Modules.Catalog.Movies.Commands.Create;

public sealed class CreateMovieActorItem
{
    public int ActorId { get; init; }
    public string CharacterName { get; init; } = string.Empty;
}

public sealed class CreateMovieCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public DateTime ReleaseDate { get; init; }
    public int Duration { get; init; }
    public int DirectorId { get; init; }
    public List<int> Categories { get; init; } = new();
    public List<CreateMovieActorItem> Actors { get; init; } = new();
    public int CountryId { get; init; }
    public string TrailerLink { get; init; } = string.Empty;
    public string ImageBase64 { get; init; } = string.Empty;
    public string StoryLine { get; init; } = string.Empty;
    public decimal Price { get; init; }
}