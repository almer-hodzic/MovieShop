namespace Market.Application.Modules.Catalog.Actors.Commands.Delete;

public sealed class DeleteActorCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteActorCommand>
{
    public async Task Handle(DeleteActorCommand request, CancellationToken ct)
    {
        var entity = await ctx.Actors
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Actor with Id {request.Id} not found.");

        ctx.Actors.Remove(entity);
        await ctx.SaveChangesAsync(ct);
    }
}