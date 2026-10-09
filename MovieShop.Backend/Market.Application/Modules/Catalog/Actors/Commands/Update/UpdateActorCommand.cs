namespace Market.Application.Modules.Catalog.Actors.Commands.Update;

public sealed class UpdateActorCommand : IRequest
{
    public int Id { get; set; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PhotoBase64 { get; init; } = string.Empty;
    public DateTime BirthDate { get; init; }
    public int CountryId { get; init; }
    public string ImdbLink { get; init; } = string.Empty;
    public string Biography { get; init; } = string.Empty;
}