namespace Market.Application.Modules.Catalog.Actors.Commands.Create;

public sealed class CreateActorCommand : IRequest<int>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhotoBase64 { get; init; } = string.Empty;
    public DateTime BirthDate { get; init; }
    public int CountryId { get; init; }
    public string ImdbLink { get; init; } = string.Empty;
    public string Biography { get; init; } = string.Empty;
}