namespace Market.Application.Modules.Sales.ShoppingCart.Commands.MoveSavedItemToCart;

public sealed class MoveSavedItemToCartCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<MoveSavedItemToCartCommand>
{
    public async Task Handle(MoveSavedItemToCartCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var item = await ctx.CartItems
            .Include(x => x.ShoppingCart)
                .ThenInclude(x => x!.CartItems)
            .FirstOrDefaultAsync(
                x => x.Id == request.ItemId &&
                     x.IsSavedForLater &&
                     x.ShoppingCart != null &&
                     x.ShoppingCart.UserId == userId,
                ct)
            ?? throw new MarketNotFoundException($"Saved cart item with Id {request.ItemId} not found.");

        var cart = item.ShoppingCart!;
        var duplicateActiveItem = cart.CartItems
            .FirstOrDefault(x => x.Id != item.Id && x.MovieId == item.MovieId && !x.IsSavedForLater);

        if (duplicateActiveItem is not null)
            ctx.CartItems.Remove(duplicateActiveItem);

        item.IsSavedForLater = false;
        item.AddedAt = DateTime.UtcNow;
        cart.LastModifiedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
    }
}
