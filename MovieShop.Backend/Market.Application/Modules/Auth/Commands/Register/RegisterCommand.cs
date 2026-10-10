namespace Market.Application.Modules.Auth.Commands.Register;

/// <summary>
/// Command for registering a regular MovieShop user.
/// </summary>
public sealed class RegisterCommand : IRequest<RegisterCommandDto>
{
    public string Firstname { get; init; } = string.Empty;
    public string Lastname { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
