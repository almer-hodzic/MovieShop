namespace Market.Application.Modules.Catalog.Categories.Commands.Delete;

public sealed class DeleteCategoryCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var entity = await ctx.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Category with Id {request.Id} not found.");

        ctx.Categories.Remove(entity);
        await ctx.SaveChangesAsync(ct);
    }
}