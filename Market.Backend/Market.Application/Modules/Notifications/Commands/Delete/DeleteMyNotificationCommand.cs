namespace Market.Application.Modules.Notifications.Commands.Delete;

public sealed class DeleteMyNotificationCommand : IRequest
{
    public int NotificationId { get; init; }
}
