namespace Market.Application.Modules.Notifications.Commands.MarkRead;

public sealed class MarkNotificationAsReadCommand : IRequest
{
    public int NotificationId { get; init; }
}
