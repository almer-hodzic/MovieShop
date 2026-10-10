namespace Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveSavedItem;

public sealed class RemoveSavedShoppingCartItemCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<RemoveSavedShoppingCartItemCommand>
{
    public async Task Handle(RemoveSavedShoppingCartItemCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var item = await ctx.CartItems
            .Include(x => x.ShoppingCart)
            .FirstOrDefaultAsync(
                x => x.Id == request.ItemId &&
                     x.IsSavedForLater &&
                     x.ShoppingCart != null &&
                     x.ShoppingCart.UserId == userId,
                ct)
            ?? throw new MarketNotFoundException($"Saved cart item with Id {request.ItemId} not found.");

        item.ShoppingCart!.LastModifiedAt = DateTime.UtcNow;
        ctx.CartItems.Remove(item);

        await ctx.SaveChangesAsync(ct);
    }
}
