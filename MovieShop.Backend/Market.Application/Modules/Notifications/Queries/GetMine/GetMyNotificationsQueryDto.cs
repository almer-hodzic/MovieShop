namespace Market.Application.Modules.Notifications.Queries.GetMine;

public sealed class GetMyNotificationsQueryDto
{
    public required int NotificationId { get; init; }
    public required bool IsRead { get; init; }
    public required string NotificationText { get; init; }
    public required DateTime NotificationDate { get; init; }
    public required int CreatorId { get; init; }
    public required int MovieId { get; init; }
}
