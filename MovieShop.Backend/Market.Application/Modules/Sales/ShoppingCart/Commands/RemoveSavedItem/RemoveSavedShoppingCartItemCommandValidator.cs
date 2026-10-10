namespace Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveSavedItem;

public sealed class RemoveSavedShoppingCartItemCommandValidator : AbstractValidator<RemoveSavedShoppingCartItemCommand>
{
    public RemoveSavedShoppingCartItemCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("ItemId must be greater than 0.");
    }
}
