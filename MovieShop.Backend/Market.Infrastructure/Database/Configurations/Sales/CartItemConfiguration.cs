namespace Market.Infrastructure.Database.Configurations.Sales;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItemEntity>
{
    public void Configure(EntityTypeBuilder<CartItemEntity> b)
    {
        b.ToTable("CartItems");

        b.HasKey(x => x.Id);

        b.Property(x => x.ShoppingCartId)
            .IsRequired();

        b.Property(x => x.MovieId)
            .IsRequired();

        b.Property(x => x.Price)
            .IsRequired();

        b.Property(x => x.Quantity)
            .IsRequired();

        b.Property(x => x.AddedAt)
            .IsRequired();

        b.Property(x => x.IsSavedForLater)
            .IsRequired()
            .HasDefaultValue(false);

        b.HasOne(x => x.ShoppingCart)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Movie)
            .WithMany()
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.ShoppingCartId);
        b.HasIndex(x => x.MovieId);
        b.HasIndex(x => new { x.ShoppingCartId, x.MovieId, x.IsSavedForLater });
    }
}
