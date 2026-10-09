namespace Market.Application.Abstractions;

public sealed class PayPalCreateOrderResult
{
    public required string OrderId { get; init; }
}

public sealed class PayPalCaptureOrderResult
{
    public required bool IsCaptured { get; init; }
    public required string Status { get; init; }
    public required string Message { get; init; }
}

public interface IPayPalService
{
    Task<PayPalCreateOrderResult> CreateOrderAsync(decimal amount, CancellationToken ct);
    Task<PayPalCaptureOrderResult> CaptureOrderAsync(string orderId, CancellationToken ct);
    Task<string> GetOrderStatusAsync(string orderId, CancellationToken ct);
}
