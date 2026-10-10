using Market.Shared.Options;
using Microsoft.Extensions.Options;

namespace Market.Application.Modules.Payments.PayPal.Commands.CreateOrder;

public sealed class CreatePayPalOrderCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser,
    IPayPalService payPalService,
    IPayPalCheckoutSessionStore checkoutSessionStore,
    IOptions<PayPalOptions> payPalOptions)
    : IRequestHandler<CreatePayPalOrderCommand, CreatePayPalOrderCommandDto>
{
    public async Task<CreatePayPalOrderCommandDto> Handle(CreatePayPalOrderCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var amount = await ctx.ShoppingCarts
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .SelectMany(x => x.CartItems)
            .Where(x => !x.IsSavedForLater)
            .SumAsync(x => (decimal?)(x.Price * x.Quantity), ct)
            ?? 0m;

        if (amount <= 0)
            throw new MarketBadRequestException("The shopping cart is empty.");

        var result = await payPalService.CreateOrderAsync(amount, ct);
        var currencyCode = payPalOptions.Value.CurrencyCode;

        checkoutSessionStore.Store(new PayPalCheckoutSession
        {
            OrderId = result.OrderId,
            UserId = userId,
            Amount = amount,
            CurrencyCode = currencyCode,
        });

        return new CreatePayPalOrderCommandDto
        {
            OrderId = result.OrderId,
            Amount = amount,
            CurrencyCode = currencyCode,
        };
    }
}
