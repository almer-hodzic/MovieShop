namespace Market.Application.Modules.Payments.PayPal.Commands.CreateOrder;

public sealed class CreatePayPalOrderCommandDto
{
    public required string OrderId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
}
