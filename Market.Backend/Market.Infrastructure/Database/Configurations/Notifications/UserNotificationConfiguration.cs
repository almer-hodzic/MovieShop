namespace Market.Infrastructure.Database.Configurations.Notifications;

public sealed class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotificationEntity>
{
    public void Configure(EntityTypeBuilder<UserNotificationEntity> b)
    {
        b.ToTable("UserNotifications");

        b.HasKey(x => x.Id);

        b.Property(x => x.IsRead)
            .IsRequired();

        b.Property(x => x.NotificationId)
            .IsRequired();

        b.Property(x => x.UserId)
            .IsRequired();

        b.HasOne(x => x.Notification)
            .WithMany(x => x.UserNotifications)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.NotificationId);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.NotificationId, x.UserId })
            .IsUnique();
    }
}
