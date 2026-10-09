using Market.Domain.Common;
using Market.Domain.Entities.Identity;

namespace Market.Domain.Entities.Catalog;

public sealed class ReviewEntity : BaseEntity
{
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    public decimal Score { get; set; }
    public string Comment { get; set; } = string.Empty;

    public int UserId { get; set; }
    public MarketUserEntity? User { get; set; }

    public int MovieId { get; set; }
    public MovieEntity? Movie { get; set; }

    public static class Constraints
    {
        public const int CommentMinLength = 4;
        public const int CommentMaxLength = 81;
    }
}
