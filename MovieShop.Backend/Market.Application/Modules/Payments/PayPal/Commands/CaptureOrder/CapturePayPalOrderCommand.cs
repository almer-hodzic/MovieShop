namespace Market.Application.Modules.Payments.PayPal.Commands.CaptureOrder;

public sealed class CapturePayPalOrderCommand : IRequest<CapturePayPalOrderCommandDto>
{
    public string OrderId { get; init; } = string.Empty;
}
