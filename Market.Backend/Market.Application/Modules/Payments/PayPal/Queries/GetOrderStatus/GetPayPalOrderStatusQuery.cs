namespace Market.Application.Modules.Payments.PayPal.Queries.GetOrderStatus;

public sealed class GetPayPalOrderStatusQuery : IRequest<GetPayPalOrderStatusQueryDto>
{
    public string OrderId { get; init; } = string.Empty;
}
