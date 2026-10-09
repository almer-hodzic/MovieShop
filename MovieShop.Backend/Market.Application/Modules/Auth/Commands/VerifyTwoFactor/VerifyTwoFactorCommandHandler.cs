namespace Market.Application.Modules.Auth.Commands.VerifyTwoFactor;

using Market.Application.Modules.Auth.Commands.Login;

public sealed class VerifyTwoFactorCommandHandler(
    IAppDbContext ctx,
    IJwtTokenService jwt,
    IPasswordHasher<MarketUserEntity> hasher,
    TimeProvider timeProvider)
    : IRequestHandler<VerifyTwoFactorCommand, LoginCommandDto>
{
    private const int MaximumFailedAttempts = 5;

    public async Task<LoginCommandDto> Handle(VerifyTwoFactorCommand request, CancellationToken ct)
    {
        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId && x.IsEnabled && !x.IsDeleted, ct)
            ?? throw new MarketNotFoundException("User was not found or is disabled.");

        if (!user.IsTwoFactorEnabled || string.IsNullOrWhiteSpace(user.TwoFactorCodeHash))
            throw new MarketConflictException("No active two-factor challenge exists.");

        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        if (user.TwoFactorCodeExpiresAtUtc is null || user.TwoFactorCodeExpiresAtUtc <= nowUtc)
        {
            ClearChallenge(user);
            await ctx.SaveChangesAsync(ct);
            throw new MarketConflictException("Two-factor verification code has expired.");
        }

        var passwordVerification = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
            throw new MarketConflictException("Invalid two-factor credentials.");

        var codeVerification = hasher.VerifyHashedPassword(user, user.TwoFactorCodeHash, request.Code);
        if (codeVerification == PasswordVerificationResult.Failed)
        {
            user.TwoFactorFailedAttempts++;
            user.ModifiedAtUtc = nowUtc;

            if (user.TwoFactorFailedAttempts >= MaximumFailedAttempts)
                ClearChallenge(user);

            await ctx.SaveChangesAsync(ct);
            throw new MarketConflictException("Invalid or expired two-factor verification code.");
        }

        ClearChallenge(user);
        user.ModifiedAtUtc = nowUtc;

        var tokens = jwt.IssueTokens(user);
        ctx.RefreshTokens.Add(new RefreshTokenEntity
        {
            TokenHash = tokens.RefreshTokenHash,
            ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc,
            UserId = user.Id,
            Fingerprint = request.Fingerprint
        });

        await ctx.SaveChangesAsync(ct);

        return new LoginCommandDto
        {
            UserId = user.Id,
            RequiresTwoFactor = false,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshTokenRaw,
            ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc
        };
    }

    private static void ClearChallenge(MarketUserEntity user)
    {
        user.TwoFactorCodeHash = null;
        user.TwoFactorCodeExpiresAtUtc = null;
        user.TwoFactorFailedAttempts = 0;
    }
}
