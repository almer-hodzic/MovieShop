namespace Market.Infrastructure.Database.Configurations.Identity;

public sealed class UserEntityConfiguration : IEntityTypeConfiguration<MarketUserEntity>
{
    public void Configure(EntityTypeBuilder<MarketUserEntity> b)
    {
        b.ToTable("Users");

        b.HasKey(x => x.Id);

        b.HasIndex(x => x.Email)
            .IsUnique();

        b.HasIndex(x => x.Username)
            .IsUnique();

        b.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.PasswordHash)
            .IsRequired();

        b.Property(x => x.ProfileImage)
            .HasColumnType("varbinary(max)");

        // Roles
        b.Property(x => x.IsAdmin)
            .HasDefaultValue(false);

        b.Property(x => x.IsManager)
            .HasDefaultValue(false);

        b.Property(x => x.IsEmployee)
            .HasDefaultValue(true); // Default: regular user

        b.Property(x => x.TokenVersion)
            .HasDefaultValue(0);

        b.Property(x => x.IsEnabled)
            .HasDefaultValue(true);

        b.Property(x => x.IsEmailConfirmed)
            .HasDefaultValue(false);

        b.Property(x => x.EmailConfirmationTokenHash)
            .HasMaxLength(128);

        b.Property(x => x.PasswordResetTokenHash)
            .HasMaxLength(128);

        b.Property(x => x.IsTwoFactorEnabled)
            .HasDefaultValue(false);

        b.Property(x => x.TwoFactorCodeHash)
            .HasMaxLength(512);

        b.Property(x => x.TwoFactorFailedAttempts)
            .HasDefaultValue(0);

        // Navigation
        b.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}
