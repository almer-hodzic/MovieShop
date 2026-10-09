using Market.Application.Abstractions;
using Market.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Market.Infrastructure.Common;

public sealed class DevelopmentAuthEmailService(
    IOptions<EmailOptions> options,
    ILogger<DevelopmentAuthEmailService> logger)
    : IAuthEmailService
{
    private readonly EmailOptions _options = options.Value;

    public Task<AuthEmailDeliveryResult> SendEmailConfirmationAsync(
        string email,
        string displayName,
        string confirmationToken,
        CancellationToken ct)
    {
        var link = BuildFrontendLink(
            "auth/confirm-email",
            ("email", email),
            ("token", confirmationToken));

        LogFallback("EmailConfirmation", email, displayName, link, code: null);
        return Task.FromResult(Result("EmailConfirmation", "Confirmation email was written to development logs."));
    }

    public Task<AuthEmailDeliveryResult> SendPasswordResetAsync(
        string email,
        string displayName,
        string resetToken,
        CancellationToken ct)
    {
        var link = BuildFrontendLink(
            "auth/reset-password",
            ("email", email),
            ("token", resetToken));

        LogFallback("PasswordReset", email, displayName, link, code: null);
        return Task.FromResult(Result("PasswordReset", "Password reset email was written to development logs."));
    }

    public Task<AuthEmailDeliveryResult> SendTwoFactorCodeAsync(
        string email,
        string displayName,
        string verificationCode,
        CancellationToken ct)
    {
        LogFallback("TwoFactorCode", email, displayName, link: null, verificationCode);
        return Task.FromResult(Result("TwoFactorCode", "Two-factor code was written to development logs."));
    }

    private static AuthEmailDeliveryResult Result(string purpose, string message)
    {
        return new AuthEmailDeliveryResult
        {
            Provider = "DevelopmentLogs",
            Purpose = purpose,
            FallbackUsed = true,
            Message = message
        };
    }

    private void LogFallback(string purpose, string recipient, string displayName, string? link, string? code)
    {
        logger.LogWarning(
            "Development email fallback used. Purpose: {Purpose}; Recipient: {Recipient}; DisplayName: {DisplayName}; LocalTestLink: {LocalTestLink}; Code: {Code}",
            purpose,
            recipient,
            displayName,
            link ?? "(not applicable)",
            code ?? "(not applicable)");
    }

    private string BuildFrontendLink(string route, params (string Key, string Value)[] parameters)
    {
        var query = string.Join(
            "&",
            parameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        return $"{_options.FrontendBaseUrl.TrimEnd('/')}/{route}?{query}";
    }
}
