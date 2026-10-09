namespace Market.Infrastructure.Database.Configurations.Catalog;

public sealed class DirectorConfiguration : IEntityTypeConfiguration<DirectorEntity>
{
    public void Configure(EntityTypeBuilder<DirectorEntity> b)
    {
        b.ToTable("Directors");

        b.HasKey(x => x.Id);

        b.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(DirectorEntity.Constraints.FirstNameMaxLength);

        b.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(DirectorEntity.Constraints.LastNameMaxLength);

        b.Property(x => x.BirthDate)
            .IsRequired();

        b.HasIndex(x => new { x.FirstName, x.LastName, x.BirthDate });
    }
}