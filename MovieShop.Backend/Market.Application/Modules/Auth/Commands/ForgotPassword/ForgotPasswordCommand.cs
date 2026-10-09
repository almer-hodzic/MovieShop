namespace Market.Application.Modules.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommand : IRequest<ForgotPasswordCommandDto>
{
    public string Email { get; init; } = string.Empty;
}
