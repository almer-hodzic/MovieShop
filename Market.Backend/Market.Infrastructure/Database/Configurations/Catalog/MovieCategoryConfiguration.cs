namespace Market.Infrastructure.Database.Configurations.Catalog;

public sealed class MovieCategoryConfiguration : IEntityTypeConfiguration<MovieCategoryEntity>
{
    public void Configure(EntityTypeBuilder<MovieCategoryEntity> b)
    {
        b.ToTable("MovieCategories");

        b.HasKey(x => x.Id);

        b.Property(x => x.MovieId)
            .IsRequired();

        b.Property(x => x.CategoryId)
            .IsRequired();

        b.HasOne(x => x.Movie)
            .WithMany(x => x.MovieCategories)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.MovieId, x.CategoryId })
            .IsUnique();
    }
}