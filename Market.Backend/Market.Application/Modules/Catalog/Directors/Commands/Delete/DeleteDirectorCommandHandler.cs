namespace Market.Application.Modules.Catalog.Directors.Commands.Delete;

public sealed class DeleteDirectorCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteDirectorCommand>
{
    public async Task Handle(DeleteDirectorCommand request, CancellationToken ct)
    {
        var entity = await ctx.Directors
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Director with Id {request.Id} not found.");

        ctx.Directors.Remove(entity);
        await ctx.SaveChangesAsync(ct);
    }
}