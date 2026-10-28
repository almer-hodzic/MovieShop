using Market.Domain.Entities.Sales;
namespace Market.Infrastructure.Database.Configurations.Sales;

public sealed class CheckoutSessionConfiguration : IEntityTypeConfiguration<CheckoutSessionEntity>
{
    public void Configure(EntityTypeBuilder<CheckoutSessionEntity> b)
    {
        b.ToTable("CheckoutSessions");
        b.HasKey(x => x.OrderId);
        b.Property(x => x.OrderId).HasMaxLength(128);
        b.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
        b.HasIndex(x => x.ExpiresAtUtc);
        b.HasOne<MarketUserEntity>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
