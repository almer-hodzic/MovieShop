namespace Market.Application.Modules.Sales.ShoppingCart.Queries.GetMine;

public sealed class GetMyShoppingCartQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyShoppingCartQuery, GetMyShoppingCartQueryDto>
{
    public async Task<GetMyShoppingCartQueryDto> Handle(GetMyShoppingCartQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var cart = await ctx.ShoppingCarts
            .AsNoTracking()
            .Include(x => x.CartItems)
                .ThenInclude(x => x.Movie)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        if (cart is null)
        {
            return new GetMyShoppingCartQueryDto
            {
                CartId = 0,
                UserId = userId,
                LastModifiedAt = DateTime.UtcNow,
                Items = [],
                SavedForLaterItems = [],
                TotalQuantity = 0,
                TotalAmount = 0m,
            };
        }

        var items = cart.CartItems
            .Where(x => !x.IsSavedForLater)
            .OrderByDescending(x => x.AddedAt)
            .Select(x => new GetMyShoppingCartItemDto
            {
                ItemId = x.Id,
                MovieId = x.MovieId,
                MovieTitle = x.Movie != null ? x.Movie.Title : string.Empty,
                MovieImage = x.Movie?.Image,
                UnitPrice = x.Price,
                Quantity = x.Quantity,
                TotalPrice = x.Price * x.Quantity,
                AddedAt = x.AddedAt,
            })
            .ToList();

        var savedForLaterItems = cart.CartItems
            .Where(x => x.IsSavedForLater)
            .OrderByDescending(x => x.AddedAt)
            .Select(x => new GetMySavedForLaterItemDto
            {
                ItemId = x.Id,
                MovieId = x.MovieId,
                MovieTitle = x.Movie != null ? x.Movie.Title : string.Empty,
                MovieImage = x.Movie?.Image,
                UnitPrice = x.Price,
                AddedAt = x.AddedAt,
            })
            .ToList();

        return new GetMyShoppingCartQueryDto
        {
            CartId = cart.Id,
            UserId = cart.UserId,
            LastModifiedAt = cart.LastModifiedAt,
            Items = items,
            SavedForLaterItems = savedForLaterItems,
            TotalQuantity = items.Sum(x => x.Quantity),
            TotalAmount = items.Sum(x => x.TotalPrice),
        };
    }
}
