using Market.Domain.Common;
using Market.Domain.Entities.Catalog;

namespace Market.Domain.Entities.Sales;

public sealed class CartItemEntity : BaseEntity
{
    public int ShoppingCartId { get; set; }
    public ShoppingCartEntity? ShoppingCart { get; set; }

    public int MovieId { get; set; }
    public MovieEntity? Movie { get; set; }

    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public bool IsSavedForLater { get; set; }

    public static class Constraints
    {
        public const int QuantityMin = 1;
        public const int QuantityMax = 100;
    }
}
