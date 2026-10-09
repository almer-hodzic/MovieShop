namespace Market.Application.Modules.Auth.Commands.VerifyTwoFactor;

using Market.Application.Modules.Auth.Commands.Login;

public sealed class VerifyTwoFactorCommand : IRequest<LoginCommandDto>
{
    public int UserId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? Fingerprint { get; init; }
}
