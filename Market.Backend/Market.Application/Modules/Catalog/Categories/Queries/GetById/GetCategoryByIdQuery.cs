namespace Market.Application.Modules.Catalog.Categories.Queries.GetById;

public sealed class GetCategoryByIdQuery : IRequest<GetCategoryByIdQueryDto>
{
    public int Id { get; init; }
}