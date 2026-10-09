namespace Market.Application.Modules.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandDto
{
    public required string Email { get; init; }
    public required string Message { get; init; }
}
