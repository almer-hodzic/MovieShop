namespace Market.Application.Modules.Catalog.Directors.Queries.List;

public sealed class ListDirectorsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListDirectorsQuery, PageResult<ListDirectorsQueryDto>>
{
    public async Task<PageResult<ListDirectorsQueryDto>> Handle(ListDirectorsQuery request, CancellationToken ct)
    {
        var q = ctx.Directors.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var term = request.Name.Trim();
            q = q.Where(x => x.FirstName.Contains(term) || x.LastName.Contains(term));
        }

        var projectedQuery = q
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new ListDirectorsQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                BirthDate = x.BirthDate,
            });

        return await PageResult<ListDirectorsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
