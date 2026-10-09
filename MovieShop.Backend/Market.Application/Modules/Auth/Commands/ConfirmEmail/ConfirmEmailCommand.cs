namespace Market.Application.Modules.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommand : IRequest<ConfirmEmailCommandDto>
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}
