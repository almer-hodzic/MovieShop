namespace Market.Application.Modules.Sales.ShoppingCart.Commands.AddItem;

public sealed class AddItemToShoppingCartCommandValidator : AbstractValidator<AddItemToShoppingCartCommand>
{
    public AddItemToShoppingCartCommandValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0).WithMessage("MovieId must be greater than 0.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(CartItemEntity.Constraints.QuantityMin, CartItemEntity.Constraints.QuantityMax)
            .WithMessage($"Quantity must be between {CartItemEntity.Constraints.QuantityMin} and {CartItemEntity.Constraints.QuantityMax}.");
    }
}
