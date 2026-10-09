namespace Market.Application.Modules.Catalog.Movies.Queries.List;

public sealed class ListMoviesQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListMoviesQuery, PageResult<ListMoviesQueryDto>>
{
    public async Task<PageResult<ListMoviesQueryDto>> Handle(ListMoviesQuery request, CancellationToken ct)
    {
        var q = ctx.Movies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            var term = request.Title.Trim();
            q = q.Where(x => x.Title.Contains(term));
        }

        if (request.CategoryId is not null)
        {
            var categoryId = request.CategoryId.Value;
            q = q.Where(x => ctx.MovieCategories.Any(mc => mc.MovieId == x.Id && mc.CategoryId == categoryId));
        }

        if (request.PriceFrom is not null)
            q = q.Where(x => x.Price >= request.PriceFrom.Value);

        if (request.PriceTo is not null)
            q = q.Where(x => x.Price <= request.PriceTo.Value);

        var isDescending = request.IsDescending ?? false;
        q = request.SortedColumn switch
        {
            "AverageScore" => isDescending ? q.OrderByDescending(x => x.AverageScore) : q.OrderBy(x => x.AverageScore),
            "LastAddedMovies" => isDescending ? q.OrderByDescending(x => x.CreationDate) : q.OrderBy(x => x.CreationDate),
            _ => q.OrderByDescending(x => x.CreationDate)
        };

        var totalItems = await q.CountAsync(ct);

        var pageItems = await q
            .Skip(request.Paging.SkipCount)
            .Take(request.Paging.PageSize)
            .Select(x => new ListMoviesQueryDto
            {
                Id = x.Id,
                Title = x.Title,
                ReleaseDate = x.ReleaseDate,
                CreationDate = x.CreationDate,
                Duration = x.Duration,
                DirectorId = x.DirectorId,
                DirectorName = x.Director != null ? (x.Director.FirstName + " " + x.Director.LastName) : string.Empty,
                CountryId = x.CountryId,
                TrailerLink = x.TrailerLink,
                Image = x.Image,
                StoryLine = x.StoryLine,
                Price = x.Price,
                AverageScore = x.AverageScore,
            })
            .ToListAsync(ct);

        var movieIds = pageItems.Select(x => x.Id).ToList();

        var categories = await ctx.MovieCategories
            .AsNoTracking()
            .Where(x => movieIds.Contains(x.MovieId))
            .Select(x => new
            {
                x.MovieId,
                Item = new ListMoviesCategoryDto
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.CategoryName : string.Empty,
                }
            })
            .ToListAsync(ct);

        var actors = await ctx.MovieActors
            .AsNoTracking()
            .Where(x => movieIds.Contains(x.MovieId))
            .Select(x => new
            {
                x.MovieId,
                Item = new ListMoviesActorDto
                {
                    ActorId = x.ActorId,
                    FirstName = x.Actor != null ? x.Actor.FirstName : string.Empty,
                    LastName = x.Actor != null ? x.Actor.LastName : string.Empty,
                    CharacterName = x.CharacterName,
                }
            })
            .ToListAsync(ct);

        var categoriesByMovie = categories
            .GroupBy(x => x.MovieId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<ListMoviesCategoryDto>)g.Select(x => x.Item).ToList());

        var actorsByMovie = actors
            .GroupBy(x => x.MovieId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<ListMoviesActorDto>)g.Select(x => x.Item).ToList());

        foreach (var item in pageItems)
        {
            item.Categories = categoriesByMovie.GetValueOrDefault(item.Id, new List<ListMoviesCategoryDto>());
            item.Actors = actorsByMovie.GetValueOrDefault(item.Id, new List<ListMoviesActorDto>());
        }

        return new PageResult<ListMoviesQueryDto>
        {
            Items = pageItems,
            PageSize = request.Paging.PageSize,
            CurrentPage = request.Paging.Page,
            IncludedTotal = true,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.Paging.PageSize)
        };
    }
}