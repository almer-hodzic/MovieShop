using Market.Domain.Common;

namespace Market.Domain.Entities.Catalog;

public sealed class MovieCategoryEntity : BaseEntity
{
    public int MovieId { get; set; }
    public MovieEntity? Movie { get; set; }

    public int CategoryId { get; set; }
    public CategoryEntity? Category { get; set; }
}