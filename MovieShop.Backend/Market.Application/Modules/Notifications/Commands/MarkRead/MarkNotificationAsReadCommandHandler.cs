namespace Market.Application.Modules.Notifications.Commands.MarkRead;

public sealed class MarkNotificationAsReadCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<MarkNotificationAsReadCommand>
{
    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var userNotification = await ctx.UserNotifications
            .FirstOrDefaultAsync(
                x => x.NotificationId == request.NotificationId && x.UserId == userId,
                ct)
            ?? throw new MarketNotFoundException($"Notification with Id {request.NotificationId} not found for current user.");

        userNotification.IsRead = true;
        await ctx.SaveChangesAsync(ct);
    }
}
