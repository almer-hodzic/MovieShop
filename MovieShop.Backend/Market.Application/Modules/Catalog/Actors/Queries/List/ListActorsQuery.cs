namespace Market.Application.Modules.Catalog.Actors.Queries.List;

public sealed class ListActorsQuery : BasePagedQuery<ListActorsQueryDto>
{
    public string? Name { get; init; }
}