namespace Market.Application.Modules.Payments.PayPal.Commands.CaptureOrder;

public sealed class CapturePayPalOrderCommandValidator : AbstractValidator<CapturePayPalOrderCommand>
{
    public CapturePayPalOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");
    }
}
