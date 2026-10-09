namespace Market.Application.Modules.Catalog.Directors.Commands.Delete;

public sealed class DeleteDirectorCommand : IRequest
{
    public int Id { get; init; }
}