namespace Market.Application.Modules.Catalog.Directors.Queries.List;

public sealed class ListDirectorsQueryDto
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required DateTime BirthDate { get; init; }
}