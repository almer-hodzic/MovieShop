namespace Market.Application.Modules.Catalog.Directors.Queries.GetById;

public sealed class GetDirectorByIdQuery : IRequest<GetDirectorByIdQueryDto>
{
    public int Id { get; init; }
}