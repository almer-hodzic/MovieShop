namespace Market.Infrastructure.Database.Configurations.Catalog;

public sealed class FavouriteMovieConfiguration : IEntityTypeConfiguration<FavouriteMovieEntity>
{
    public void Configure(EntityTypeBuilder<FavouriteMovieEntity> b)
    {
        b.ToTable("FavouriteMovies");

        b.HasKey(x => x.Id);

        b.Property(x => x.UserId)
            .IsRequired();

        b.Property(x => x.MovieId)
            .IsRequired();

        b.Property(x => x.DateAdded)
            .IsRequired();

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Movie)
            .WithMany()
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.UserId, x.MovieId })
            .IsUnique();
    }
}
