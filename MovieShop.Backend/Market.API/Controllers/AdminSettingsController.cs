using Market.Application.Modules.Admin.Settings.Commands.UpdateMine;
using Market.Application.Modules.Admin.Settings.Queries.GetMine;

namespace Market.API.Controllers;

[ApiController]
[Route("Admin/Settings")]
[Authorize(Policy = "AdminOnly")]
public sealed class AdminSettingsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<GetAdminSettingsQueryDto> Get(CancellationToken ct)
    {
        return await sender.Send(new GetAdminSettingsQuery(), ct);
    }

    [HttpPut]
    public async Task<UpdateAdminSettingsCommandDto> Update(UpdateAdminSettingsCommand command, CancellationToken ct)
    {
        return await sender.Send(command, ct);
    }
}
