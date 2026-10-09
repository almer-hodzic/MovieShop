namespace Market.Infrastructure.Database.Configurations.Catalog;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> b)
    {
        b.ToTable("Categories");

        b.HasKey(x => x.Id);

        b.Property(x => x.CategoryName)
            .IsRequired()
            .HasMaxLength(CategoryEntity.Constraints.CategoryNameMaxLength);

        b.HasIndex(x => x.CategoryName)
            .IsUnique();
    }
}