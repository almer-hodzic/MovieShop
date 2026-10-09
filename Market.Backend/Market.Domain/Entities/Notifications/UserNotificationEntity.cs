using Market.Domain.Common;
using Market.Domain.Entities.Identity;

namespace Market.Domain.Entities.Notifications;

public sealed class UserNotificationEntity : BaseEntity
{
    public bool IsRead { get; set; }

    public int NotificationId { get; set; }
    public NotificationEntity? Notification { get; set; }

    public int UserId { get; set; }
    public MarketUserEntity? User { get; set; }
}
