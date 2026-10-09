namespace Market.Application.Modules.Sales.ShoppingCart.Commands.Clear;

public sealed class ClearShoppingCartCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ClearShoppingCartCommand>
{
    public async Task Handle(ClearShoppingCartCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var cart = await ctx.ShoppingCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        if (cart is null || !cart.CartItems.Any())
            return;

        ctx.CartItems.RemoveRange(cart.CartItems);
        cart.LastModifiedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
    }
}
