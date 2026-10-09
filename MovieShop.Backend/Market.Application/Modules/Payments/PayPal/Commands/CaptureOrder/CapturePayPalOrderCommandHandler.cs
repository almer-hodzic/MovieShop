namespace Market.Application.Modules.Payments.PayPal.Commands.CaptureOrder;

public sealed class CapturePayPalOrderCommandHandler(
    IPayPalService payPalService,
    IAppCurrentUser currentUser,
    IPayPalCheckoutSessionStore checkoutSessionStore)
    : IRequestHandler<CapturePayPalOrderCommand, CapturePayPalOrderCommandDto>
{
    public async Task<CapturePayPalOrderCommandDto> Handle(CapturePayPalOrderCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new MarketConflictException("Authenticated user is required.");

        var session = checkoutSessionStore.Get(request.OrderId)
            ?? throw new MarketNotFoundException("PayPal checkout session was not found or has expired.");

        if (session.UserId != userId)
            throw new MarketForbiddenException("This PayPal order does not belong to the current user.");

        if (session.IsCaptured)
            throw new MarketConflictException("This PayPal order has already been captured.");

        var captureResult = await payPalService.CaptureOrderAsync(request.OrderId, ct);
        if (captureResult.IsCaptured)
            checkoutSessionStore.MarkCaptured(request.OrderId);

        return new CapturePayPalOrderCommandDto
        {
            Status = captureResult.IsCaptured ? "SUCCESS" : "FAILED",
            Message = captureResult.Message,
        };
    }
}
