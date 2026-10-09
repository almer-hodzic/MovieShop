namespace Market.Application.Modules.Catalog.Categories.Commands.Update;

public sealed class UpdateCategoryCommand : IRequest
{
    public int Id { get; set; }
    public string CategoryName { get; init; } = string.Empty;
}