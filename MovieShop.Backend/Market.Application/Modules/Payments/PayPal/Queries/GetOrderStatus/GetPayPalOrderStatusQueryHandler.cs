namespace Market.Application.Modules.Payments.PayPal.Queries.GetOrderStatus;

public sealed class GetPayPalOrderStatusQueryHandler(IPayPalService payPalService,
    IAppCurrentUser currentUser, IPayPalCheckoutSessionStore checkoutSessionStore)
    : IRequestHandler<GetPayPalOrderStatusQuery, GetPayPalOrderStatusQueryDto>
{
    public async Task<GetPayPalOrderStatusQueryDto> Handle(GetPayPalOrderStatusQuery request, CancellationToken ct)
    {
        var session = checkoutSessionStore.Get(request.OrderId)
            ?? throw new MarketNotFoundException("PayPal checkout session was not found or has expired.");
        if (session.UserId != currentUser.UserId)
            throw new MarketForbiddenException("This PayPal order does not belong to the current user.");
        var status = await payPalService.GetOrderStatusAsync(request.OrderId, ct);
        return new GetPayPalOrderStatusQueryDto
        {
            Status = status,
        };
    }
}
