namespace Market.Application.Modules.Sales.ShoppingCart.Commands.MoveSavedItemToCart;

public sealed class MoveSavedItemToCartCommand : IRequest
{
    public int ItemId { get; init; }
}
