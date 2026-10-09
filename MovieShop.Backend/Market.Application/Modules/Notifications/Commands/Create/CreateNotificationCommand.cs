namespace Market.Application.Modules.Notifications.Commands.Create;

public sealed class CreateNotificationCommand : IRequest<int>
{
    public string NotificationText { get; init; } = string.Empty;
    public int MovieId { get; init; }
}
