using Market.Application.Modules.Notifications.Commands.Create;
using Market.Application.Modules.Notifications.Commands.Delete;
using Market.Application.Modules.Notifications.Commands.MarkRead;
using Market.Application.Modules.Notifications.Queries.GetMine;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class NotificationsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateNotificationCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return Ok(id);
    }

    [HttpGet("my")]
    public async Task<IReadOnlyList<GetMyNotificationsQueryDto>> GetMine(CancellationToken ct)
    {
        return await sender.Send(new GetMyNotificationsQuery(), ct);
    }

    [HttpPut("{notificationId:int}/read")]
    public async Task<IActionResult> MarkRead(int notificationId, CancellationToken ct)
    {
        await sender.Send(new MarkNotificationAsReadCommand { NotificationId = notificationId }, ct);
        return NoContent();
    }

    [HttpDelete("{notificationId:int}")]
    public async Task<IActionResult> Delete(int notificationId, CancellationToken ct)
    {
        await sender.Send(new DeleteMyNotificationCommand { NotificationId = notificationId }, ct);
        return NoContent();
    }
}
