namespace Market.Infrastructure.Database.Configurations.Sales;

public sealed class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCartEntity>
{
    public void Configure(EntityTypeBuilder<ShoppingCartEntity> b)
    {
        b.ToTable("ShoppingCarts");

        b.HasKey(x => x.Id);

        b.Property(x => x.UserId)
            .IsRequired();

        b.Property(x => x.LastModifiedAt)
            .IsRequired();

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.CartItems)
            .WithOne(x => x.ShoppingCart)
            .HasForeignKey(x => x.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UserId)
            .IsUnique();
    }
}
