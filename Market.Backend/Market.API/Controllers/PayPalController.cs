using Market.Application.Modules.Payments.PayPal.Commands.CaptureOrder;
using Market.Application.Modules.Payments.PayPal.Commands.CreateOrder;
using Market.Application.Modules.Payments.PayPal.Queries.GetOrderStatus;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class PayPalController(ISender sender) : ControllerBase
{
    [HttpPost("CreateOrder")]
    public async Task<ActionResult<CreatePayPalOrderCommandDto>> CreateOrder(CancellationToken ct)
    {
        var result = await sender.Send(new CreatePayPalOrderCommand(), ct);
        return Ok(result);
    }

    [HttpPost("CaptureOrder")]
    public async Task<ActionResult<CapturePayPalOrderCommandDto>> CaptureOrder([FromBody] CapturePayPalOrderCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Status == "SUCCESS" ? Ok(result) : BadRequest(result);
    }

    [HttpGet("OrderStatus/{orderId}")]
    public async Task<ActionResult<GetPayPalOrderStatusQueryDto>> GetOrderStatus(string orderId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPayPalOrderStatusQuery { OrderId = orderId }, ct);
        return Ok(result);
    }
}
