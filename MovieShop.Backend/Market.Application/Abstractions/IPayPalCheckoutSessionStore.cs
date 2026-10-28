namespace Market.Application.Abstractions;

public sealed class PayPalCheckoutSession
{
    public required string OrderId { get; init; }
    public required int UserId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public bool IsCaptured { get; set; }
}

public interface IPayPalCheckoutSessionStore
{
    Task StoreAsync(PayPalCheckoutSession session, CancellationToken ct);
    Task<PayPalCheckoutSession?> GetAsync(string orderId, CancellationToken ct);
    Task MarkCapturedAsync(string orderId, CancellationToken ct);
}
