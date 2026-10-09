namespace Market.Application.Modules.Catalog.Directors.Queries.GetById;

public sealed class GetDirectorByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetDirectorByIdQuery, GetDirectorByIdQueryDto>
{
    public async Task<GetDirectorByIdQueryDto> Handle(GetDirectorByIdQuery request, CancellationToken ct)
    {
        var director = await ctx.Directors
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetDirectorByIdQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                BirthDate = x.BirthDate,
            })
            .FirstOrDefaultAsync(ct);

        if (director is null)
            throw new MarketNotFoundException($"Director with Id {request.Id} not found.");

        return director;
    }
}