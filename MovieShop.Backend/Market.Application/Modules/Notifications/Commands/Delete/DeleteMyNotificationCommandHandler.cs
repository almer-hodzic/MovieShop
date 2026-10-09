namespace Market.Application.Modules.Notifications.Commands.Delete;

public sealed class DeleteMyNotificationCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteMyNotificationCommand>
{
    public async Task Handle(DeleteMyNotificationCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var userNotification = await ctx.UserNotifications
            .FirstOrDefaultAsync(
                x => x.NotificationId == request.NotificationId && x.UserId == userId,
                ct)
            ?? throw new MarketNotFoundException($"Notification with Id {request.NotificationId} not found for current user.");

        ctx.UserNotifications.Remove(userNotification);
        await ctx.SaveChangesAsync(ct);
    }
}
