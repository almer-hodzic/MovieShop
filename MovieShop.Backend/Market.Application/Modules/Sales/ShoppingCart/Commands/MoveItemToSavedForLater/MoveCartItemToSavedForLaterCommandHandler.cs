namespace Market.Application.Modules.Sales.ShoppingCart.Commands.MoveItemToSavedForLater;

public sealed class MoveCartItemToSavedForLaterCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<MoveCartItemToSavedForLaterCommand>
{
    public async Task Handle(MoveCartItemToSavedForLaterCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var item = await ctx.CartItems
            .Include(x => x.ShoppingCart)
                .ThenInclude(x => x!.CartItems)
            .FirstOrDefaultAsync(
                x => x.Id == request.ItemId &&
                     !x.IsSavedForLater &&
                     x.ShoppingCart != null &&
                     x.ShoppingCart.UserId == userId,
                ct)
            ?? throw new MarketNotFoundException($"Active cart item with Id {request.ItemId} not found.");

        var cart = item.ShoppingCart!;
        var duplicateSavedItem = cart.CartItems
            .FirstOrDefault(x => x.Id != item.Id && x.MovieId == item.MovieId && x.IsSavedForLater);

        if (duplicateSavedItem is not null)
            ctx.CartItems.Remove(duplicateSavedItem);

        item.IsSavedForLater = true;
        item.AddedAt = DateTime.UtcNow;
        cart.LastModifiedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
    }
}
