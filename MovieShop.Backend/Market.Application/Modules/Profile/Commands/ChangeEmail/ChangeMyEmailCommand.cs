namespace Market.Application.Modules.Profile.Commands.ChangeEmail;

public sealed class ChangeMyEmailCommand : IRequest
{
    public string Email { get; init; } = string.Empty;
}

