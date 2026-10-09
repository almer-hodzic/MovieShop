namespace Market.Application.Modules.Auth.Commands.ForgotPassword;

using System.Security.Cryptography;
using System.Text;

public sealed class ForgotPasswordCommandHandler(
    IAppDbContext ctx,
    IAuthEmailService emailService)
    : IRequestHandler<ForgotPasswordCommand, ForgotPasswordCommandDto>
{
    public async Task<ForgotPasswordCommandDto> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && !x.IsDeleted, ct)
            ?? throw new MarketNotFoundException("User with this email does not exist.");

        var previousTokenHash = user.PasswordResetTokenHash;
        var previousTokenExpiry = user.PasswordResetTokenExpiresAtUtc;
        var previousModifiedAt = user.ModifiedAtUtc;
        var token = GenerateToken();
        var expiresAtUtc = DateTime.UtcNow.AddDays(1);

        user.PasswordResetTokenHash = HashToken(token);
        user.PasswordResetTokenExpiresAtUtc = expiresAtUtc;
        user.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);

        try
        {
            var emailDelivery = await emailService.SendPasswordResetAsync(
                user.Email,
                $"{user.Firstname} {user.Lastname}".Trim(),
                token,
                ct);

            return new ForgotPasswordCommandDto
            {
                Email = user.Email,
                Message = emailDelivery.FallbackUsed
                    ? "Password reset instructions were written to the development logs."
                    : "Password reset instructions have been sent to your email.",
                EmailDeliveryFallbackUsed = emailDelivery.FallbackUsed,
                EmailDeliveryMessage = emailDelivery.Message
            };
        }
        catch
        {
            user.PasswordResetTokenHash = previousTokenHash;
            user.PasswordResetTokenExpiresAtUtc = previousTokenExpiry;
            user.ModifiedAtUtc = previousModifiedAt;
            await ctx.SaveChangesAsync(CancellationToken.None);
            throw;
        }
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
