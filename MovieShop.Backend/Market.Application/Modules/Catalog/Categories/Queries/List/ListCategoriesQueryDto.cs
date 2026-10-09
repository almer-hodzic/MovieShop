namespace Market.Application.Modules.Catalog.Categories.Queries.List;

public sealed class ListCategoriesQueryDto
{
    public required int Id { get; init; }
    public required string CategoryName { get; init; }
}