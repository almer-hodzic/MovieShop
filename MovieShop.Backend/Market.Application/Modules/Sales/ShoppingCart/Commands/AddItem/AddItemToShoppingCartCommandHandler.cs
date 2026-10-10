namespace Market.Application.Modules.Sales.ShoppingCart.Commands.AddItem;

public sealed class AddItemToShoppingCartCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<AddItemToShoppingCartCommand, int>
{
    public async Task<int> Handle(AddItemToShoppingCartCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var userExists = await ctx.Users.AnyAsync(x => x.Id == userId, ct);
        if (!userExists)
            throw new MarketNotFoundException($"User with Id {userId} not found.");

        var movie = await ctx.Movies
            .FirstOrDefaultAsync(x => x.Id == request.MovieId, ct)
            ?? throw new MarketNotFoundException($"Movie with Id {request.MovieId} not found.");

        var cart = await ctx.ShoppingCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        if (cart is null)
        {
            cart = new ShoppingCartEntity
            {
                UserId = userId,
                LastModifiedAt = DateTime.UtcNow,
            };
            ctx.ShoppingCarts.Add(cart);
        }

        var existingItem = cart.CartItems.FirstOrDefault(x => x.MovieId == request.MovieId);
        if (existingItem is { IsSavedForLater: false })
            throw new MarketConflictException($"Movie with Id {request.MovieId} is already in the cart.");

        if (existingItem is { IsSavedForLater: true })
        {
            existingItem.IsSavedForLater = false;
            existingItem.Quantity = request.Quantity;
            existingItem.Price = movie.Price;
            existingItem.AddedAt = DateTime.UtcNow;
            cart.LastModifiedAt = DateTime.UtcNow;

            await ctx.SaveChangesAsync(ct);
            return existingItem.Id;
        }

        var item = new CartItemEntity
        {
            ShoppingCart = cart,
            MovieId = request.MovieId,
            Price = movie.Price,
            Quantity = request.Quantity,
            AddedAt = DateTime.UtcNow,
            IsSavedForLater = false,
        };

        cart.LastModifiedAt = DateTime.UtcNow;
        ctx.CartItems.Add(item);

        await ctx.SaveChangesAsync(ct);

        return item.Id;
    }
}
