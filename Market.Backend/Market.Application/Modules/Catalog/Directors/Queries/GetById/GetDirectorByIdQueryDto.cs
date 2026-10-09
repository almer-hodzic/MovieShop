namespace Market.Application.Modules.Catalog.Directors.Queries.GetById;

public sealed class GetDirectorByIdQueryDto
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required DateTime BirthDate { get; init; }
}