namespace Market.Application.Modules.Sales.ShoppingCart.Commands.RemoveItem;

public sealed class RemoveShoppingCartItemCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<RemoveShoppingCartItemCommand>
{
    public async Task Handle(RemoveShoppingCartItemCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var item = await ctx.CartItems
            .Include(x => x.ShoppingCart)
            .FirstOrDefaultAsync(
                x => x.Id == request.ItemId &&
                     x.ShoppingCart != null &&
                     x.ShoppingCart.UserId == userId,
                ct)
            ?? throw new MarketNotFoundException($"Cart item with Id {request.ItemId} not found.");

        item.ShoppingCart!.LastModifiedAt = DateTime.UtcNow;
        ctx.CartItems.Remove(item);

        await ctx.SaveChangesAsync(ct);
    }
}
