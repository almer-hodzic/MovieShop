namespace Market.Application.Modules.Catalog.Actors.Commands.Delete;

public sealed class DeleteActorCommand : IRequest
{
    public int Id { get; init; }
}