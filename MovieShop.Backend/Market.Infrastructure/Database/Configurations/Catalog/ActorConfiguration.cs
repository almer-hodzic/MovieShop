namespace Market.Infrastructure.Database.Configurations.Catalog;

public sealed class ActorConfiguration : IEntityTypeConfiguration<ActorEntity>
{
    public void Configure(EntityTypeBuilder<ActorEntity> b)
    {
        b.ToTable("Actors");

        b.HasKey(x => x.Id);

        b.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(ActorEntity.Constraints.FirstNameMaxLength);

        b.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(ActorEntity.Constraints.LastNameMaxLength);

        b.Property(x => x.BirthDate)
            .IsRequired();

        b.Property(x => x.CountryId)
            .IsRequired();

        b.Property(x => x.ImdbLink)
            .HasMaxLength(ActorEntity.Constraints.ImdbLinkMaxLength);

        b.Property(x => x.Biography)
            .HasMaxLength(ActorEntity.Constraints.BiographyMaxLength);

        b.HasIndex(x => new { x.FirstName, x.LastName, x.BirthDate });
    }
}