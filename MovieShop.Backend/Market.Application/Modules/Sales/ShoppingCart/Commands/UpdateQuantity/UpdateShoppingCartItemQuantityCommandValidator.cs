namespace Market.Application.Modules.Sales.ShoppingCart.Commands.UpdateQuantity;

public sealed class UpdateShoppingCartItemQuantityCommandValidator : AbstractValidator<UpdateShoppingCartItemQuantityCommand>
{
    public UpdateShoppingCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("ItemId must be greater than 0.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(CartItemEntity.Constraints.QuantityMin, CartItemEntity.Constraints.QuantityMax)
            .WithMessage($"Quantity must be between {CartItemEntity.Constraints.QuantityMin} and {CartItemEntity.Constraints.QuantityMax}.");
    }
}
