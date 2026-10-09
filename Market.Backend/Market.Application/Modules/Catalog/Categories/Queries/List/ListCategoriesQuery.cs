namespace Market.Application.Modules.Catalog.Categories.Queries.List;

public sealed class ListCategoriesQuery : BasePagedQuery<ListCategoriesQueryDto>
{
    public string? CategoryName { get; init; }
}