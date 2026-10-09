using Market.Application.Modules.Profile.Commands.ChangeEmail;
using Market.Application.Modules.Profile.Commands.ChangePassword;
using Market.Application.Modules.Profile.Commands.UpdateProfileImage;
using Market.Application.Modules.Profile.Queries.GetMine;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class ProfileController(ISender sender) : ControllerBase
{
    [HttpGet("my")]
    public async Task<GetMyProfileQueryDto> GetMine(CancellationToken ct)
    {
        return await sender.Send(new GetMyProfileQuery(), ct);
    }

    [HttpPut("password")]
    public async Task ChangePassword(ChangeMyPasswordCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
    }

    [HttpPut("email")]
    public async Task ChangeEmail(ChangeMyEmailCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
    }

    [HttpPut("profile-image")]
    public async Task UpdateProfileImage(UpdateMyProfileImageCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
    }
}

