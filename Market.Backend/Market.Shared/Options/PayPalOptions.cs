using System.ComponentModel.DataAnnotations;

namespace Market.Shared.Options;

public sealed class PayPalOptions
{
    public const string SectionName = "PayPal";

    [Required] public string ClientId { get; init; } = default!;
    [Required] public string ClientSecret { get; init; } = default!;
    [Required] public string Mode { get; init; } = "sandbox";
    [Required] public string BaseUrl { get; init; } = "https://api-m.sandbox.paypal.com";
    [Required] public string CurrencyCode { get; init; } = "USD";
}
