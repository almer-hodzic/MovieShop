namespace Market.Infrastructure.Database.Configurations.Catalog;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<ReviewEntity>
{
    public void Configure(EntityTypeBuilder<ReviewEntity> b)
    {
        b.ToTable("Reviews");

        b.HasKey(x => x.Id);

        b.Property(x => x.ReviewDate)
            .IsRequired();

        b.Property(x => x.Score)
            .IsRequired();

        b.Property(x => x.Comment)
            .IsRequired()
            .HasMaxLength(ReviewEntity.Constraints.CommentMaxLength);

        b.Property(x => x.UserId)
            .IsRequired();

        b.Property(x => x.MovieId)
            .IsRequired();

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Movie)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.MovieId);
        b.HasIndex(x => new { x.MovieId, x.ReviewDate });
    }
}
