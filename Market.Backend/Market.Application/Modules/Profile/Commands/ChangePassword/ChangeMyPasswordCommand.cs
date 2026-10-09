namespace Market.Application.Modules.Profile.Commands.ChangePassword;

public sealed class ChangeMyPasswordCommand : IRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

