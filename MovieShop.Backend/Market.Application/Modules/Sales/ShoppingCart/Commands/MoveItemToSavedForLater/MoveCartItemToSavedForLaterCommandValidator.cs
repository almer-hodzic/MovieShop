namespace Market.Application.Modules.Sales.ShoppingCart.Commands.MoveItemToSavedForLater;

public sealed class MoveCartItemToSavedForLaterCommandValidator : AbstractValidator<MoveCartItemToSavedForLaterCommand>
{
    public MoveCartItemToSavedForLaterCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("ItemId must be greater than 0.");
    }
}
