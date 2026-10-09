using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.Encodings.Web;
using Market.Application.Abstractions;
using Market.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Market.Infrastructure.Common;

public sealed class DevelopmentSmtpAuthEmailService(
    IOptions<DevelopmentEmailOptions> developmentOptions,
    IOptions<EmailOptions> emailOptions,
    ILogger<DevelopmentSmtpAuthEmailService> logger)
    : IAuthEmailService
{
    private readonly DevelopmentEmailOptions _developmentOptions = developmentOptions.Value;
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    public Task<AuthEmailDeliveryResult> SendEmailConfirmationAsync(
        string email,
        string displayName,
        string confirmationToken,
        CancellationToken ct)
    {
        var link = BuildFrontendLink("auth/confirm-email", email, confirmationToken);
        var body = BuildHtmlMessage(
            "Confirm your MovieShop account",
            displayName,
            $"Please confirm your MovieShop account for {HtmlEncoder.Default.Encode(email)}.",
            "Confirm account",
            link,
            "This confirmation link expires in 24 hours.");

        return SendAsync(
            email,
            displayName,
            "Confirm your MovieShop account",
            body,
            "EmailConfirmation",
            "Confirmation email sent through development SMTP.",
            ct);
    }

    public Task<AuthEmailDeliveryResult> SendPasswordResetAsync(
        string email,
        string displayName,
        string resetToken,
        CancellationToken ct)
    {
        var link = BuildFrontendLink("auth/reset-password", email, resetToken);
        var body = BuildHtmlMessage(
            "Reset your MovieShop password",
            displayName,
            "A password reset was requested for your MovieShop account.",
            "Reset password",
            link,
            "This reset link expires in 24 hours. If you did not request it, you can ignore this email.");

        return SendAsync(
            email,
            displayName,
            "Reset your MovieShop password",
            body,
            "PasswordReset",
            "Password reset email sent through development SMTP.",
            ct);
    }

    public Task<AuthEmailDeliveryResult> SendTwoFactorCodeAsync(
        string email,
        string displayName,
        string verificationCode,
        CancellationToken ct)
    {
        var encodedCode = HtmlEncoder.Default.Encode(verificationCode);
        var body = BuildHtmlShell(
            "Your MovieShop verification code",
            displayName,
            $"""
            <p>Use this code to finish signing in to MovieShop:</p>
            <p style="font-size:24px;font-weight:700;letter-spacing:4px;margin:24px 0;">{encodedCode}</p>
            <p>This code expires in 5 minutes.</p>
            """);

        return SendAsync(
            email,
            displayName,
            "Your MovieShop verification code",
            body,
            "TwoFactorCode",
            "Two-factor code sent through development SMTP.",
            ct);
    }

    public static bool CanUse(DevelopmentEmailOptions options)
    {
        return options.HasRequiredSmtpSettings()
            && !IsSandboxInterceptionHost(options.Host);
    }

    public static bool IsSandboxInterceptionHost(string? host)
    {
        return string.Equals(
            host?.Trim(),
            "sandbox.smtp.mailtrap.io",
            StringComparison.OrdinalIgnoreCase);
    }

    private async Task<AuthEmailDeliveryResult> SendAsync(
        string email,
        string displayName,
        string subject,
        string htmlBody,
        string purpose,
        string message,
        CancellationToken ct)
    {
        using var mail = new MailMessage
        {
            From = new MailAddress(_developmentOptions.FromEmail!, _developmentOptions.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        mail.To.Add(new MailAddress(email, displayName));
        mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            StripHtmlFallback(htmlBody),
            null,
            MediaTypeNames.Text.Plain));

        using var client = new SmtpClient(_developmentOptions.Host!, _developmentOptions.Port)
        {
            EnableSsl = _developmentOptions.UseSsl,
            Credentials = new NetworkCredential(_developmentOptions.Username, _developmentOptions.Password)
        };

        await client.SendMailAsync(mail, ct);

        logger.LogInformation(
            "Development email delivered through SMTP. Purpose: {Purpose}; Recipient: {Recipient}; Host: {Host}; Port: {Port}; UseSsl: {UseSsl}",
            purpose,
            email,
            _developmentOptions.Host,
            _developmentOptions.Port,
            _developmentOptions.UseSsl);

        return new AuthEmailDeliveryResult
        {
            Provider = "DevelopmentSmtp",
            Purpose = purpose,
            FallbackUsed = false,
            Message = message
        };
    }

    private string BuildFrontendLink(string path, string email, string token)
    {
        var baseUrl = _emailOptions.FrontendBaseUrl.TrimEnd('/');
        var encodedEmail = Uri.EscapeDataString(email);
        var encodedToken = Uri.EscapeDataString(token);

        return $"{baseUrl}/{path}?email={encodedEmail}&token={encodedToken}";
    }

    private static string BuildHtmlMessage(
        string title,
        string displayName,
        string intro,
        string actionText,
        string actionUrl,
        string footer)
    {
        var encodedIntro = HtmlEncoder.Default.Encode(intro);
        var encodedActionText = HtmlEncoder.Default.Encode(actionText);
        var encodedActionUrl = HtmlEncoder.Default.Encode(actionUrl);
        var encodedFooter = HtmlEncoder.Default.Encode(footer);

        return BuildHtmlShell(
            title,
            displayName,
            $"""
            <p>{encodedIntro}</p>
            <p style="margin:24px 0;">
              <a href="{encodedActionUrl}" style="background:#111827;color:#ffffff;padding:12px 18px;text-decoration:none;border-radius:6px;display:inline-block;">{encodedActionText}</a>
            </p>
            <p>If the button does not work, open this link:</p>
            <p><a href="{encodedActionUrl}">{encodedActionUrl}</a></p>
            <p>{encodedFooter}</p>
            """);
    }

    private static string BuildHtmlShell(string title, string displayName, string content)
    {
        var safeTitle = HtmlEncoder.Default.Encode(title);
        var safeName = string.IsNullOrWhiteSpace(displayName)
            ? "MovieShop user"
            : HtmlEncoder.Default.Encode(displayName);

        return $$"""
        <!doctype html>
        <html>
        <body style="font-family:Arial,sans-serif;line-height:1.5;color:#111827;">
          <main style="max-width:560px;margin:0 auto;padding:24px;">
            <h1 style="font-size:22px;margin:0 0 16px;">{{safeTitle}}</h1>
            <p>Hello {{safeName}},</p>
            {{content}}
            <p style="margin-top:32px;color:#6b7280;">MovieShop</p>
          </main>
        </body>
        </html>
        """;
    }

    private static string StripHtmlFallback(string html)
    {
        return WebUtility.HtmlDecode(
            html.Replace("<br>", Environment.NewLine, StringComparison.OrdinalIgnoreCase)
                .Replace("<br />", Environment.NewLine, StringComparison.OrdinalIgnoreCase)
                .Replace("</p>", Environment.NewLine, StringComparison.OrdinalIgnoreCase)
                .Replace("</h1>", Environment.NewLine, StringComparison.OrdinalIgnoreCase)
                .Replace("<", " <", StringComparison.Ordinal));
    }
}
