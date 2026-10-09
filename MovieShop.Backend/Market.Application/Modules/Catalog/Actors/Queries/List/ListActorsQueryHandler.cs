namespace Market.Application.Modules.Catalog.Actors.Queries.List;

public sealed class ListActorsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListActorsQuery, PageResult<ListActorsQueryDto>>
{
    public async Task<PageResult<ListActorsQueryDto>> Handle(ListActorsQuery request, CancellationToken ct)
    {
        var q = ctx.Actors.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var term = request.Name.Trim();
            q = q.Where(x => x.FirstName.Contains(term) || x.LastName.Contains(term));
        }

        var projectedQuery = q
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new ListActorsQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Photo = x.Photo,
                BirthDate = x.BirthDate,
                CountryId = x.CountryId,
                ImdbLink = x.ImdbLink ?? string.Empty,
                Biography = x.Biography ?? string.Empty,
            });

        return await PageResult<ListActorsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}