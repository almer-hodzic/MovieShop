namespace Market.Application.Modules.Catalog.Directors.Queries.List;

public sealed class ListDirectorsQuery : BasePagedQuery<ListDirectorsQueryDto>
{
    public string? Name { get; init; }
}