using Market.Domain.Common;

namespace Market.Domain.Entities.Notifications;

public sealed class NotificationEntity : BaseEntity
{
    public string NotificationText { get; set; } = string.Empty;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
    public int CreatorId { get; set; }
    public int MovieId { get; set; }

    public ICollection<UserNotificationEntity> UserNotifications { get; set; } = new List<UserNotificationEntity>();

    public static class Constraints
    {
        public const int NotificationTextMaxLength = 500;
    }
}
