namespace Market.Application.Modules.Payments.PayPal.Commands.CaptureOrder;

public sealed class CapturePayPalOrderCommandDto
{
    public required string Status { get; init; }
    public required string Message { get; init; }
}
