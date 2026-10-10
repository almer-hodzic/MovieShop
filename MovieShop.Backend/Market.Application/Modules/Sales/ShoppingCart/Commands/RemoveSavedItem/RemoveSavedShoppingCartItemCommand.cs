namespace Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveSavedItem;

public sealed class RemoveSavedShoppingCartItemCommand : IRequest
{
    public int ItemId { get; init; }
}
