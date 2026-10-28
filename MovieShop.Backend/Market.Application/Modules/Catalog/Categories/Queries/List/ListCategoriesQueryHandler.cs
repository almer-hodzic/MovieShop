namespace Market.Application.Modules.Catalog.Categories.Queries.List;

public sealed class ListCategoriesQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListCategoriesQuery, PageResult<ListCategoriesQueryDto>>
{
    public async Task<PageResult<ListCategoriesQueryDto>> Handle(ListCategoriesQuery request, CancellationToken ct)
    {
        var q = ctx.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.CategoryName))
        {
            var term = request.CategoryName.Trim().ToLower();
            q = q.Where(x => x.CategoryName.ToLower().Contains(term));
        }

        var projectedQuery = q
            .OrderBy(x => x.CategoryName)
            .Select(x => new ListCategoriesQueryDto
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
            });

        return await PageResult<ListCategoriesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}