namespace Market.Application.Modules.Catalog.Movies.Commands.Delete;

public sealed class DeleteMovieCommand : IRequest
{
    public int Id { get; init; }
}