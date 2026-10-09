namespace Market.Application.Modules.Catalog.Directors.Commands.Update;

public sealed class UpdateDirectorCommand : IRequest
{
    public int Id { get; set; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime BirthDate { get; init; }
}