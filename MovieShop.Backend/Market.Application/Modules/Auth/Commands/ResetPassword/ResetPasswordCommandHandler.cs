namespace Market.Application.Modules.Auth.Commands.ResetPassword;

using System.Security.Cryptography;
using System.Text;

public sealed class ResetPasswordCommandHandler(
    IAppDbContext ctx,
    IPasswordHasher<MarketUserEntity> hasher)
    : IRequestHandler<ResetPasswordCommand, ResetPasswordCommandDto>
{
    public async Task<ResetPasswordCommandDto> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && !x.IsDeleted, ct)
            ?? throw new MarketNotFoundException("User with this email does not exist.");

        if (string.IsNullOrWhiteSpace(user.PasswordResetTokenHash) ||
            user.PasswordResetTokenHash != HashToken(request.Token.Trim()))
            throw new MarketConflictException("Invalid password reset token.");

        if (user.PasswordResetTokenExpiresAtUtc is null ||
            user.PasswordResetTokenExpiresAtUtc < DateTime.UtcNow)
            throw new MarketConflictException("Password reset token has expired.");

        var existingPassword = hasher.VerifyHashedPassword(user, user.PasswordHash, request.NewPassword);
        if (existingPassword != PasswordVerificationResult.Failed)
            throw new MarketConflictException("New password must be different from the current password.");

        var changedAtUtc = DateTime.UtcNow;
        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiresAtUtc = null;
        user.TwoFactorCodeHash = null;
        user.TwoFactorCodeExpiresAtUtc = null;
        user.TwoFactorFailedAttempts = 0;
        user.TokenVersion++;
        user.ModifiedAtUtc = changedAtUtc;

        var activeRefreshTokens = await ctx.RefreshTokens
            .Where(x => x.UserId == user.Id && !x.IsRevoked && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAtUtc = changedAtUtc;
        }

        await ctx.SaveChangesAsync(ct);

        return new ResetPasswordCommandDto
        {
            Email = user.Email,
            Message = "Password has been reset successfully."
        };
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
