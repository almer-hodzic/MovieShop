namespace Market.Application.Modules.Catalog.Directors.Commands.Create;

public sealed class CreateDirectorCommand : IRequest<int>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime BirthDate { get; init; }
}