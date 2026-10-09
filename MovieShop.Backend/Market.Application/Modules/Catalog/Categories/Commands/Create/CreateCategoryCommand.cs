namespace Market.Application.Modules.Catalog.Categories.Commands.Create;

public sealed class CreateCategoryCommand : IRequest<int>
{
    public string CategoryName { get; init; } = string.Empty;
}