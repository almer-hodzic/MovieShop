namespace Market.Application.Modules.Catalog.Categories.Queries.GetById;

public sealed class GetCategoryByIdQueryDto
{
    public required int Id { get; init; }
    public required string CategoryName { get; init; }
}