namespace Market.Application.Modules.Admin.Dashboard.Queries.Get;

public sealed class GetAdminDashboardQueryDto
{
    public required int MoviesCount { get; init; }
    public required int CategoriesCount { get; init; }
    public required int ActorsCount { get; init; }
    public required int DirectorsCount { get; init; }
    public required int ReviewsCount { get; init; }
    public required int UsersCount { get; init; }
    public required int FavouritesCount { get; init; }
    public required int ActiveCartItemsCount { get; init; }
    public required int NotificationsCount { get; init; }
}
