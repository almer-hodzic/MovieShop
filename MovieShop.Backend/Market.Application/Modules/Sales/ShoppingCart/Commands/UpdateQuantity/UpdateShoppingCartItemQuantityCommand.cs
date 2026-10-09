namespace Market.Application.Modules.Sales.ShoppingCart.Commands.UpdateQuantity;

public sealed class UpdateShoppingCartItemQuantityCommand : IRequest
{
    public int ItemId { get; set; }
    public int Quantity { get; init; }
}
