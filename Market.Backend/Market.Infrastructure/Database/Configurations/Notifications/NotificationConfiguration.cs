namespace Market.Infrastructure.Database.Configurations.Notifications;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
{
    public void Configure(EntityTypeBuilder<NotificationEntity> b)
    {
        b.ToTable("Notifications");

        b.HasKey(x => x.Id);

        b.Property(x => x.NotificationText)
            .IsRequired()
            .HasMaxLength(NotificationEntity.Constraints.NotificationTextMaxLength);

        b.Property(x => x.DateTime)
            .IsRequired();

        b.Property(x => x.CreatorId)
            .IsRequired();

        b.Property(x => x.MovieId)
            .IsRequired();

        b.HasOne<MarketUserEntity>()
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<MovieEntity>()
            .WithMany()
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.UserNotifications)
            .WithOne(x => x.Notification)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
