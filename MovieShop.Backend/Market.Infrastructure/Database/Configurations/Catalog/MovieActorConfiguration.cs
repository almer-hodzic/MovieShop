namespace Market.Infrastructure.Database.Configurations.Catalog;

public sealed class MovieActorConfiguration : IEntityTypeConfiguration<MovieActorEntity>
{
    public void Configure(EntityTypeBuilder<MovieActorEntity> b)
    {
        b.ToTable("MovieActors");

        b.HasKey(x => x.Id);

        b.Property(x => x.MovieId)
            .IsRequired();

        b.Property(x => x.ActorId)
            .IsRequired();

        b.Property(x => x.CharacterName)
            .IsRequired()
            .HasMaxLength(MovieActorEntity.Constraints.CharacterNameMaxLength);

        b.HasOne(x => x.Movie)
            .WithMany(x => x.MovieActors)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Actor)
            .WithMany()
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.MovieId, x.ActorId })
            .IsUnique();
    }
}