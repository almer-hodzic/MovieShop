namespace Market.Application.Modules.Sales.ShoppingCart.Commands.MoveItemToSavedForLater;

public sealed class MoveCartItemToSavedForLaterCommand : IRequest
{
    public int ItemId { get; init; }
}
