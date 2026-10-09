using Market.Application.Abstractions;
using Market.Application.Common.Exceptions;
using Market.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Market.Infrastructure.Common;

public sealed class SendGridAuthEmailService(
    HttpClient httpClient,
    IOptions<EmailOptions> options,
    ILogger<SendGridAuthEmailService> logger)
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

        var safeName = WebUtility.HtmlEncode(displayName);
        var content =
            $"Hello {safeName},<br><br>" +
            "Thank you for registering with MovieShop. Confirm your email by clicking the link below:<br><br>" +
            $"<a href=\"{WebUtility.HtmlEncode(link)}\">Confirm email</a><br><br>" +
            "If you did not register, you can ignore this email.<br><br>MovieShop";

        return SendAsync(email, "Confirm Your Email", content, "EmailConfirmation", ct);
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

        var safeName = WebUtility.HtmlEncode(displayName);
        var content =
            $"Dear {safeName},<br><br>" +
            "A password reset was requested for your MovieShop account. " +
            "Use the link below within 24 hours:<br><br>" +
            $"<a href=\"{WebUtility.HtmlEncode(link)}\">Reset password</a><br><br>" +
            "If you did not request this, you can ignore this email.<br><br>MovieShop";

        return SendAsync(email, "Reset Password", content, "PasswordReset", ct);
    }

    public Task<AuthEmailDeliveryResult> SendTwoFactorCodeAsync(
        string email,
        string displayName,
        string verificationCode,
        CancellationToken ct)
    {
        var safeName = WebUtility.HtmlEncode(displayName);
        var safeCode = WebUtility.HtmlEncode(verificationCode);
        var content =
            $"Hello {safeName},<br><br>" +
            $"Your MovieShop verification code is: <strong>{safeCode}</strong><br><br>" +
            "This code expires in 5 minutes. If you did not attempt to sign in, change your password.<br><br>MovieShop";

        return SendAsync(email, "Your Verification Code", content, "TwoFactorCode", ct);
    }

    public static bool CanUse(EmailOptions options)
    {
        return !string.IsNullOrWhiteSpace(options.ApiKey)
            && !options.ApiKey.StartsWith("REPLACE_WITH", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(options.FromEmail)
            && !string.IsNullOrWhiteSpace(options.FromName)
            && !string.IsNullOrWhiteSpace(options.FrontendBaseUrl);
    }

    private async Task<AuthEmailDeliveryResult> SendAsync(
        string recipient,
        string subject,
        string htmlContent,
        string purpose,
        CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "v3/mail/send");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = JsonContent.Create(new
        {
            personalizations = new[]
            {
                new
                {
                    to = new[] { new { email = recipient } }
                }
            },
            from = new
            {
                email = _options.FromEmail,
                name = _options.FromName
            },
            subject,
            content = new[]
            {
                new
                {
                    type = "text/html",
                    value = htmlContent
                }
            }
        });

        using var response = await httpClient.SendAsync(request, ct);
        if (response.IsSuccessStatusCode)
            return new AuthEmailDeliveryResult
            {
                Provider = "SendGrid",
                Purpose = purpose,
                FallbackUsed = false
            };

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        logger.LogError(
            "SendGrid email delivery failed. Status: {StatusCode}, Body: {Body}",
            response.StatusCode,
            responseBody);
        var quotaExceeded = responseBody.Contains(
            "Maximum credits exceeded",
            StringComparison.OrdinalIgnoreCase);

        throw new MarketBusinessRuleException(
            "EMAIL_DELIVERY_FAILED",
            quotaExceeded
                ? "Email service quota has been exceeded. Please try again later."
                : "Email could not be sent. Please try again later.");
    }

    private string BuildFrontendLink(string route, params (string Key, string Value)[] parameters)
    {
        var query = string.Join(
            "&",
            parameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        return $"{_options.FrontendBaseUrl.TrimEnd('/')}/{route}?{query}";
    }
}
