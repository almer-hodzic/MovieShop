namespace Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveItem;

public sealed class RemoveShoppingCartItemCommandValidator : AbstractValidator<RemoveShoppingCartItemCommand>
{
    public RemoveShoppingCartItemCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("ItemId must be greater than 0.");
    }
}
