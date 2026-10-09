namespace Market.Application.Modules.Catalog.Directors.Commands.Update;

public sealed class UpdateDirectorCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateDirectorCommand>
{
    public async Task Handle(UpdateDirectorCommand request, CancellationToken ct)
    {
        var entity = await ctx.Directors
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new MarketNotFoundException($"Director with Id {request.Id} not found.");

        entity.FirstName = request.FirstName.Trim();
        entity.LastName = request.LastName.Trim();
        entity.BirthDate = request.BirthDate.Date;

        await ctx.SaveChangesAsync(ct);
    }
}