namespace Market.Application.Abstractions;

public interface IAuthEmailService
{
    Task<AuthEmailDeliveryResult> SendEmailConfirmationAsync(
        string email,
        string displayName,
        string confirmationToken,
        CancellationToken ct);

    Task<AuthEmailDeliveryResult> SendPasswordResetAsync(
        string email,
        string displayName,
        string resetToken,
        CancellationToken ct);

    Task<AuthEmailDeliveryResult> SendTwoFactorCodeAsync(
        string email,
        string displayName,
        string verificationCode,
        CancellationToken ct);
}

public sealed class AuthEmailDeliveryResult
{
    public required string Provider { get; init; }
    public required string Purpose { get; init; }
    public required bool FallbackUsed { get; init; }
    public string? Message { get; init; }
}
