using Market.Domain.Common;
using Market.Domain.Entities.Identity;

namespace Market.Domain.Entities.Catalog;

public sealed class FavouriteMovieEntity : BaseEntity
{
    public int UserId { get; set; }
    public MarketUserEntity? User { get; set; }

    public int MovieId { get; set; }
    public MovieEntity? Movie { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
