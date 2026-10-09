namespace Market.Application.Modules.Payments.PayPal.Queries.GetOrderStatus;

public sealed class GetPayPalOrderStatusQueryValidator : AbstractValidator<GetPayPalOrderStatusQuery>
{
    public GetPayPalOrderStatusQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");
    }
}
