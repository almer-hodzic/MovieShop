namespace Market.Application.Modules.Catalog.Categories.Commands.Create;

public sealed class CreateCategoryCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var normalizedName = request.CategoryName.Trim();

        var exists = await ctx.Categories
            .AnyAsync(x => x.CategoryName.ToLower() == normalizedName.ToLower(), ct);

        if (exists)
            throw new MarketConflictException($"Category '{normalizedName}' already exists.");

        var entity = new CategoryEntity
        {
            CategoryName = normalizedName,
        };

        ctx.Categories.Add(entity);
        await ctx.SaveChangesAsync(ct);

        return entity.Id;
    }
}