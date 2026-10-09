namespace Market.Application.Modules.Catalog.Movies.Commands.Update;

public sealed class UpdateMovieActorItem
{
    public int ActorId { get; init; }
    public string CharacterName { get; init; } = string.Empty;
}

public sealed class UpdateMovieCommand : IRequest
{
    public int Id { get; set; }
    public string Title { get; init; } = string.Empty;
    public DateTime ReleaseDate { get; init; }
    public int Duration { get; init; }
    public int DirectorId { get; init; }
    public List<int> Categories { get; init; } = new();
    public List<UpdateMovieActorItem> Actors { get; init; } = new();
    public int CountryId { get; init; }
    public string TrailerLink { get; init; } = string.Empty;
    public string ImageBase64 { get; init; } = string.Empty;
    public string StoryLine { get; init; } = string.Empty;
    public decimal Price { get; init; }
}