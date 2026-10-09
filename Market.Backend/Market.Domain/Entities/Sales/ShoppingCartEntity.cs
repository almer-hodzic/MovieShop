using Market.Domain.Common;
using Market.Domain.Entities.Identity;

namespace Market.Domain.Entities.Sales;

public sealed class ShoppingCartEntity : BaseEntity
{
    public int UserId { get; set; }
    public MarketUserEntity? User { get; set; }

    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
}
