namespace Market.Infrastructure.Database.Configurations.Catalog;

public sealed class MovieConfiguration : IEntityTypeConfiguration<MovieEntity>
{
    public void Configure(EntityTypeBuilder<MovieEntity> b)
    {
        b.ToTable("Movies");

        b.HasKey(x => x.Id);

        b.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(MovieEntity.Constraints.TitleMaxLength);

        b.Property(x => x.ReleaseDate)
            .IsRequired();

        b.Property(x => x.CreationDate)
            .IsRequired();

        b.Property(x => x.Duration)
            .IsRequired();

        b.Property(x => x.DirectorId)
            .IsRequired();

        b.Property(x => x.CountryId)
            .IsRequired();

        b.Property(x => x.TrailerLink)
            .HasMaxLength(MovieEntity.Constraints.TrailerLinkMaxLength);

        b.Property(x => x.StoryLine)
            .IsRequired()
            .HasMaxLength(MovieEntity.Constraints.StoryLineMaxLength);

        b.Property(x => x.Price)
            .IsRequired();

        b.Property(x => x.AverageScore)
            .IsRequired(false);

        b.HasOne(x => x.Director)
            .WithMany()
            .HasForeignKey(x => x.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Title);
    }
}