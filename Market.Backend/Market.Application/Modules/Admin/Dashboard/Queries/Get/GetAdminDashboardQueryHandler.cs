namespace Market.Application.Modules.Admin.Dashboard.Queries.Get;

public sealed class GetAdminDashboardQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetAdminDashboardQuery, GetAdminDashboardQueryDto>
{
    public async Task<GetAdminDashboardQueryDto> Handle(GetAdminDashboardQuery request, CancellationToken ct)
    {
        return new GetAdminDashboardQueryDto
        {
            MoviesCount = await ctx.Movies.AsNoTracking().CountAsync(x => !x.IsDeleted, ct),
            CategoriesCount = await ctx.Categories.AsNoTracking().CountAsync(x => !x.IsDeleted, ct),
            ActorsCount = await ctx.Actors.AsNoTracking().CountAsync(x => !x.IsDeleted, ct),
            DirectorsCount = await ctx.Directors.AsNoTracking().CountAsync(x => !x.IsDeleted, ct),
            ReviewsCount = await ctx.Reviews.AsNoTracking().CountAsync(x => !x.IsDeleted, ct),
            UsersCount = await ctx.Users.AsNoTracking().CountAsync(x => !x.IsDeleted, ct),
            FavouritesCount = await ctx.FavouriteMovies.AsNoTracking().CountAsync(x => !x.IsDeleted, ct),
            ActiveCartItemsCount = await ctx.CartItems.AsNoTracking().CountAsync(x => !x.IsDeleted, ct),
            NotificationsCount = await ctx.Notifications.AsNoTracking().CountAsync(x => !x.IsDeleted, ct)
        };
    }
}
