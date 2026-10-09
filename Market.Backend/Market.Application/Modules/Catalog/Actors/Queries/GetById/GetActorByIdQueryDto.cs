namespace Market.Application.Modules.Catalog.Actors.Queries.GetById;

public sealed class GetActorByIdQueryDto
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public byte[]? Photo { get; init; }
    public required DateTime BirthDate { get; init; }
    public required int CountryId { get; init; }
    public required string ImdbLink { get; init; }
    public required string Biography { get; init; }
}