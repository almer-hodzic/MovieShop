namespace Market.Application.Modules.Notifications.Queries.GetMine;

public sealed class GetMyNotificationsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyNotificationsQuery, IReadOnlyList<GetMyNotificationsQueryDto>>
{
    public async Task<IReadOnlyList<GetMyNotificationsQueryDto>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        return await ctx.UserNotifications
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Notification != null)
            .OrderByDescending(x => x.Notification!.DateTime)
            .Select(x => new GetMyNotificationsQueryDto
            {
                NotificationId = x.NotificationId,
                IsRead = x.IsRead,
                NotificationText = x.Notification!.NotificationText,
                NotificationDate = x.Notification.DateTime,
                CreatorId = x.Notification.CreatorId,
                MovieId = x.Notification.MovieId,
            })
            .ToListAsync(ct);
    }
}
