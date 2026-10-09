namespace Market.Application.Modules.Notifications.Commands.Create;

public sealed class CreateNotificationCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<CreateNotificationCommand, int>
{
    public async Task<int> Handle(CreateNotificationCommand request, CancellationToken ct)
    {
        var creatorId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        if (!currentUser.IsAdmin)
            throw new MarketForbiddenException("Only administrators can create notifications.");

        var movieExists = await ctx.Movies.AnyAsync(x => x.Id == request.MovieId, ct);
        if (!movieExists)
            throw new MarketNotFoundException($"Movie with Id {request.MovieId} not found.");

        var notification = new NotificationEntity
        {
            NotificationText = request.NotificationText.Trim(),
            DateTime = DateTime.UtcNow,
            CreatorId = creatorId,
            MovieId = request.MovieId,
        };

        ctx.Notifications.Add(notification);
        await ctx.SaveChangesAsync(ct);

        var recipientIds = await ctx.Users
            .AsNoTracking()
            .Where(x => x.Id != creatorId)
            .Select(x => x.Id)
            .ToListAsync(ct);

        foreach (var userId in recipientIds)
        {
            ctx.UserNotifications.Add(new UserNotificationEntity
            {
                NotificationId = notification.Id,
                UserId = userId,
                IsRead = false,
            });
        }

        await ctx.SaveChangesAsync(ct);

        return notification.Id;
    }
}
