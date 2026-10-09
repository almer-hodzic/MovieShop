namespace Market.Application.Modules.Catalog.Categories.Commands.Delete;

public sealed class DeleteCategoryCommand : IRequest
{
    public int Id { get; init; }
}