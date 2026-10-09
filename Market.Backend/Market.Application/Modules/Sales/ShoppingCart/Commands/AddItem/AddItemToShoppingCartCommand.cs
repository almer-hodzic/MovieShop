namespace Market.Application.Modules.Sales.ShoppingCart.Commands.AddItem;

public sealed class AddItemToShoppingCartCommand : IRequest<int>
{
    public int MovieId { get; init; }
    public int Quantity { get; init; } = 1;
}
