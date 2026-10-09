namespace Market.Application.Modules.Catalog.Categories.Queries.GetById;

public sealed class GetCategoryByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetCategoryByIdQuery, GetCategoryByIdQueryDto>
{
    public async Task<GetCategoryByIdQueryDto> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await ctx.Categories
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetCategoryByIdQueryDto
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
            })
            .FirstOrDefaultAsync(ct);

        if (category is null)
            throw new MarketNotFoundException($"Category with Id {request.Id} not found.");

        return category;
    }
}