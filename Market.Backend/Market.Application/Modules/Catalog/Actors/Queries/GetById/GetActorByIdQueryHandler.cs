namespace Market.Application.Modules.Catalog.Actors.Queries.GetById;

public sealed class GetActorByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetActorByIdQuery, GetActorByIdQueryDto>
{
    public async Task<GetActorByIdQueryDto> Handle(GetActorByIdQuery request, CancellationToken ct)
    {
        var actor = await ctx.Actors
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetActorByIdQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Photo = x.Photo,
                BirthDate = x.BirthDate,
                CountryId = x.CountryId,
                ImdbLink = x.ImdbLink ?? string.Empty,
                Biography = x.Biography ?? string.Empty,
            })
            .FirstOrDefaultAsync(ct);

        if (actor is null)
            throw new MarketNotFoundException($"Actor with Id {request.Id} not found.");

        return actor;
    }
}