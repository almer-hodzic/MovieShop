namespace Market.Application.Modules.Catalog.Actors.Queries.GetById;

public sealed class GetActorByIdQuery : IRequest<GetActorByIdQueryDto>
{
    public int Id { get; init; }
}