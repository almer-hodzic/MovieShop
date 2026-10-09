namespace Market.Application.Modules.Catalog.Categories.Commands.Update;

public sealed class UpdateCategoryCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var entity = await ctx.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Category with Id {request.Id} not found.");

        var normalizedName = request.CategoryName.Trim();

        var exists = await ctx.Categories
            .AnyAsync(x => x.Id != request.Id && x.CategoryName.ToLower() == normalizedName.ToLower(), ct);

        if (exists)
            throw new MarketConflictException($"Category '{normalizedName}' already exists.");

        entity.CategoryName = normalizedName;

        await ctx.SaveChangesAsync(ct);
    }
}