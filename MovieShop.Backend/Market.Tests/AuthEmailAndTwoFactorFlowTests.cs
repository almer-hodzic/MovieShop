using Market.Application.Abstractions;
using Market.Application.Modules.Auth.Commands.Login;
using Market.Application.Modules.Auth.Commands.Register;
using Market.Infrastructure.Common;
using Market.Shared.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Market.Tests;

[Collection("IntegrationTests")]
public sealed class AuthEmailAndTwoFactorFlowTests
{
    [Fact]
    public async Task Development_email_sender_reports_explicit_fallback_delivery()
    {
        var service = new DevelopmentAuthEmailService(
            Options.Create(new EmailOptions
            {
                ApiKey = "not-used-in-development",
                FromEmail = "noreply@example.test",
                FromName = "MovieShop",
                FrontendBaseUrl = "http://localhost:4200"
            }),
            NullLogger<DevelopmentAuthEmailService>.Instance);

        var confirmation = await service.SendEmailConfirmationAsync(
            "dev-user@example.test",
            "Dev User",
            "confirm-token",
            CancellationToken.None);

        var reset = await service.SendPasswordResetAsync(
            "dev-user@example.test",
            "Dev User",
            "reset-token",
            CancellationToken.None);

        var twoFactor = await service.SendTwoFactorCodeAsync(
            "dev-user@example.test",
            "Dev User",
            "123456",
            CancellationToken.None);

        Assert.All(new[] { confirmation, reset, twoFactor }, result =>
        {
            Assert.Equal("DevelopmentLogs", result.Provider);
            Assert.True(result.FallbackUsed);
            Assert.Contains("development logs", result.Message, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public void Development_smtp_sender_requires_complete_enabled_configuration()
    {
        Assert.False(DevelopmentSmtpAuthEmailService.CanUse(new DevelopmentEmailOptions
        {
            Enabled = false,
            Host = "sandbox.smtp.test",
            Port = 587,
            Username = "user",
            Password = "password",
            FromEmail = "noreply@example.test"
        }));

        Assert.False(DevelopmentSmtpAuthEmailService.CanUse(new DevelopmentEmailOptions
        {
            Enabled = true,
            Host = "sandbox.smtp.test",
            Port = 587,
            Username = "user",
            FromEmail = "noreply@example.test"
        }));

        Assert.True(DevelopmentSmtpAuthEmailService.CanUse(new DevelopmentEmailOptions
        {
            Enabled = true,
            Host = "sandbox.smtp.test",
            Port = 587,
            Username = "user",
            Password = "password",
            FromEmail = "noreply@example.test"
        }));

        Assert.False(DevelopmentSmtpAuthEmailService.CanUse(new DevelopmentEmailOptions
        {
            Enabled = true,
            Host = "sandbox.smtp.mailtrap.io",
            Port = 587,
            Username = "user",
            Password = "password",
            FromEmail = "noreply@example.test"
        }));
    }

    [Fact]
    public void SendGrid_sender_requires_real_configuration()
    {
        Assert.False(SendGridAuthEmailService.CanUse(new EmailOptions
        {
            ApiKey = "REPLACE_WITH_SENDGRID_API_KEY",
            FromEmail = "noreply@example.test",
            FromName = "MovieShop",
            FrontendBaseUrl = "http://localhost:4200"
        }));

        Assert.True(SendGridAuthEmailService.CanUse(new EmailOptions
        {
            ApiKey = "configured-api-key",
            FromEmail = "noreply@example.test",
            FromName = "MovieShop",
            FrontendBaseUrl = "http://localhost:4200"
        }));
    }

    [Fact]
    public async Task Registration_confirmation_two_factor_and_password_reset_complete_without_exposing_secrets()
    {
        await using var factory = new AuthFlowFactory();
        using var client = factory.CreateClient();
        var email = $"auth-flow-{Guid.NewGuid():N}@example.test";
        const string password = "StrongPass123!";

        var registerResponse = await client.PostAsJsonAsync("api/auth/register", new
        {
            firstname = "Auth",
            lastname = "Flow",
            email,
            password
        });

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        var registerBody = await registerResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain("confirmationToken", registerBody, StringComparison.OrdinalIgnoreCase);
        var registration = JsonSerializer.Deserialize<RegisterCommandDto>(
            registerBody,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(registration);
        Assert.False(registration.EmailDeliveryFallbackUsed);
        Assert.False(string.IsNullOrWhiteSpace(factory.EmailService.ConfirmationToken));

        var invalidConfirmResponse = await client.PostAsJsonAsync("api/auth/confirm-email", new
        {
            email,
            token = "invalid-confirmation-token"
        });
        Assert.Equal(HttpStatusCode.Conflict, invalidConfirmResponse.StatusCode);

        var confirmResponse = await client.PostAsJsonAsync("api/auth/confirm-email", new
        {
            email,
            token = factory.EmailService.ConfirmationToken
        });
        confirmResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password,
            fingerprint = "integration-test"
        });
        loginResponse.EnsureSuccessStatusCode();

        var loginBody = await loginResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain("twoFactorCode", loginBody, StringComparison.OrdinalIgnoreCase);
        var challenge = JsonSerializer.Deserialize<LoginCommandDto>(
            loginBody,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(challenge);
        Assert.True(challenge.RequiresTwoFactor);
        Assert.False(challenge.EmailDeliveryFallbackUsed);
        Assert.False(string.IsNullOrWhiteSpace(factory.EmailService.TwoFactorCode));

        var wrongCodeResponse = await client.PostAsJsonAsync("api/auth/verify-2fa", new
        {
            userId = challenge.UserId,
            code = "000000",
            password,
            fingerprint = "integration-test"
        });
        Assert.Equal(HttpStatusCode.Conflict, wrongCodeResponse.StatusCode);

        var verifyResponse = await client.PostAsJsonAsync("api/auth/verify-2fa", new
        {
            userId = challenge.UserId,
            code = factory.EmailService.TwoFactorCode,
            password,
            fingerprint = "integration-test"
        });
        verifyResponse.EnsureSuccessStatusCode();

        var verified = await verifyResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(verified);
        Assert.False(verified.RequiresTwoFactor);
        Assert.False(string.IsNullOrWhiteSpace(verified.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(verified.RefreshToken));

        var reusedCodeResponse = await client.PostAsJsonAsync("api/auth/verify-2fa", new
        {
            userId = challenge.UserId,
            code = factory.EmailService.TwoFactorCode,
            password,
            fingerprint = "integration-test"
        });
        Assert.Equal(HttpStatusCode.Conflict, reusedCodeResponse.StatusCode);

        var expiredChallengeResponse = await client.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password,
            fingerprint = "integration-test"
        });
        expiredChallengeResponse.EnsureSuccessStatusCode();

        var expiredChallenge = await expiredChallengeResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(expiredChallenge);
        Assert.True(expiredChallenge.RequiresTwoFactor);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var user = await db.Users.SingleAsync(x => x.Id == expiredChallenge.UserId);
            user.TwoFactorCodeExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1);
            await db.SaveChangesAsync();
        }

        var expiredCodeResponse = await client.PostAsJsonAsync("api/auth/verify-2fa", new
        {
            userId = expiredChallenge.UserId,
            code = factory.EmailService.TwoFactorCode,
            password,
            fingerprint = "integration-test"
        });
        Assert.Equal(HttpStatusCode.Conflict, expiredCodeResponse.StatusCode);

        var forgotResponse = await client.PostAsJsonAsync("api/auth/forgot-password", new { email });
        forgotResponse.EnsureSuccessStatusCode();
        var forgotBody = await forgotResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain("passwordResetToken", forgotBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"emailDeliveryFallbackUsed\":false", forgotBody, StringComparison.OrdinalIgnoreCase);
        Assert.False(string.IsNullOrWhiteSpace(factory.EmailService.PasswordResetToken));

        var resetResponse = await client.PostAsJsonAsync("api/auth/reset-password", new
        {
            email,
            token = factory.EmailService.PasswordResetToken,
            newPassword = "NewStrongPass123!",
            confirmPassword = "NewStrongPass123!"
        });
        resetResponse.EnsureSuccessStatusCode();
    }

    private sealed class AuthFlowFactory : WebApplicationFactory<Program>
    {
        public CapturingAuthEmailService EmailService { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTests");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IAuthEmailService>();
                services.AddSingleton<IAuthEmailService>(EmailService);
            });
        }
    }

    private sealed class CapturingAuthEmailService : IAuthEmailService
    {
        public string? ConfirmationToken { get; private set; }
        public string? PasswordResetToken { get; private set; }
        public string? TwoFactorCode { get; private set; }

        public Task<AuthEmailDeliveryResult> SendEmailConfirmationAsync(
            string email,
            string displayName,
            string confirmationToken,
            CancellationToken ct)
        {
            ConfirmationToken = confirmationToken;
            return Task.FromResult(Delivered("EmailConfirmation"));
        }

        public Task<AuthEmailDeliveryResult> SendPasswordResetAsync(
            string email,
            string displayName,
            string resetToken,
            CancellationToken ct)
        {
            PasswordResetToken = resetToken;
            return Task.FromResult(Delivered("PasswordReset"));
        }

        public Task<AuthEmailDeliveryResult> SendTwoFactorCodeAsync(
            string email,
            string displayName,
            string verificationCode,
            CancellationToken ct)
        {
            TwoFactorCode = verificationCode;
            return Task.FromResult(Delivered("TwoFactorCode"));
        }

        private static AuthEmailDeliveryResult Delivered(string purpose)
        {
            return new AuthEmailDeliveryResult
            {
                Provider = "Test",
                Purpose = purpose,
                FallbackUsed = false
            };
        }
    }
}
