namespace Market.Application.Modules.Sales.ShoppingCart.Commands.UpdateQuantity;

public sealed class UpdateShoppingCartItemQuantityCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateShoppingCartItemQuantityCommand>
{
    public async Task Handle(UpdateShoppingCartItemQuantityCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var item = await ctx.CartItems
            .Include(x => x.ShoppingCart)
            .FirstOrDefaultAsync(
                x => x.Id == request.ItemId &&
                     !x.IsSavedForLater &&
                     x.ShoppingCart != null &&
                     x.ShoppingCart.UserId == userId,
                ct)
            ?? throw new MarketNotFoundException($"Cart item with Id {request.ItemId} not found.");

        item.Quantity = request.Quantity;
        item.ShoppingCart!.LastModifiedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
    }
}
