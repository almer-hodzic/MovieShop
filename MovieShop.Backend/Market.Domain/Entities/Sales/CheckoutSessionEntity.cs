namespace Market.Domain.Entities.Sales;

public sealed class CheckoutSessionEntity
{
    public string OrderId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public bool IsCaptured { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}
