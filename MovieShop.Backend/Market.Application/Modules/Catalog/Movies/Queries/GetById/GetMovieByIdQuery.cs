namespace Market.Application.Modules.Catalog.Movies.Queries.GetById;

public sealed class GetMovieByIdQuery : IRequest<GetMovieByIdQueryDto>
{
    public int Id { get; init; }
}