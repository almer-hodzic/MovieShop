namespace Market.Application.Modules.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandDto
{
    public required string Email { get; init; }
    public required string Message { get; init; }
    public required bool EmailDeliveryFallbackUsed { get; init; }
    public string? EmailDeliveryMessage { get; init; }
}
