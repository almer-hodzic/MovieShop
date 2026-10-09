namespace Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveItem;

public sealed class RemoveShoppingCartItemCommand : IRequest
{
    public int ItemId { get; init; }
}
