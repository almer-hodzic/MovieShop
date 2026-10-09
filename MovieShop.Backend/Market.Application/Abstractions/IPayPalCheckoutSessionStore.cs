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
    void Store(PayPalCheckoutSession session);
    PayPalCheckoutSession? Get(string orderId);
    void MarkCaptured(string orderId);
}
