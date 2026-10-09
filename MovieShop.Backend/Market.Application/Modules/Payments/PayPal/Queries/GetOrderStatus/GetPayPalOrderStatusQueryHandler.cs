namespace Market.Application.Modules.Payments.PayPal.Queries.GetOrderStatus;

public sealed class GetPayPalOrderStatusQueryHandler(IPayPalService payPalService)
    : IRequestHandler<GetPayPalOrderStatusQuery, GetPayPalOrderStatusQueryDto>
{
    public async Task<GetPayPalOrderStatusQueryDto> Handle(GetPayPalOrderStatusQuery request, CancellationToken ct)
    {
        var status = await payPalService.GetOrderStatusAsync(request.OrderId, ct);
        return new GetPayPalOrderStatusQueryDto
        {
            Status = status,
        };
    }
}
