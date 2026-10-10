namespace Market.Application.Modules.Sales.ShoppingCart.Commands.MoveSavedItemToCart;

public sealed class MoveSavedItemToCartCommandValidator : AbstractValidator<MoveSavedItemToCartCommand>
{
    public MoveSavedItemToCartCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("ItemId must be greater than 0.");
    }
}
