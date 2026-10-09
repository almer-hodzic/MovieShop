using Market.Application.Modules.Admin.Dashboard.Queries.Get;

namespace Market.API.Controllers;

[ApiController]
[Route("Admin/Dashboard")]
[Authorize(Policy = "AdminOnly")]
public sealed class AdminDashboardController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<GetAdminDashboardQueryDto> Get(CancellationToken ct)
    {
        return await sender.Send(new GetAdminDashboardQuery(), ct);
    }
}
