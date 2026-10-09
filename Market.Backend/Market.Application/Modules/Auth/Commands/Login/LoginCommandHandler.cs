using Market.Application.Modules.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IAppDbContext ctx,
    IJwtTokenService jwt,
    IPasswordHasher<MarketUserEntity> hasher,
    IAuthEmailService emailService,
    TimeProvider timeProvider)
    : IRequestHandler<LoginCommand, LoginCommandDto>
{
    public async Task<LoginCommandDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.IsEnabled && !x.IsDeleted, ct)
            ?? throw new MarketNotFoundException("Korisnik nije pronađen ili je onemogućen.");

        var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
            throw new MarketConflictException("Pogrešni kredencijali.");

        if (!user.IsEmailConfirmed)
            throw new MarketConflictException("Email address is not confirmed.");

        if (user.IsTwoFactorEnabled)
        {
            var previousCodeHash = user.TwoFactorCodeHash;
            var previousCodeExpiry = user.TwoFactorCodeExpiresAtUtc;
            var previousFailedAttempts = user.TwoFactorFailedAttempts;
            var previousModifiedAt = user.ModifiedAtUtc;
            var verificationCode = System.Security.Cryptography.RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString("D6");
            var nowUtc = timeProvider.GetUtcNow();

            user.TwoFactorCodeHash = hasher.HashPassword(user, verificationCode);
            user.TwoFactorCodeExpiresAtUtc = nowUtc.AddMinutes(5).UtcDateTime;
            user.TwoFactorFailedAttempts = 0;
            user.ModifiedAtUtc = nowUtc.UtcDateTime;

            await ctx.SaveChangesAsync(ct);

            try
            {
                var emailDelivery = await emailService.SendTwoFactorCodeAsync(
                    user.Email,
                    $"{user.Firstname} {user.Lastname}".Trim(),
                    verificationCode,
                    ct);

                return new LoginCommandDto
                {
                    UserId = user.Id,
                    RequiresTwoFactor = true,
                    EmailDeliveryFallbackUsed = emailDelivery.FallbackUsed,
                    EmailDeliveryMessage = emailDelivery.Message
                };
            }
            catch
            {
                user.TwoFactorCodeHash = previousCodeHash;
                user.TwoFactorCodeExpiresAtUtc = previousCodeExpiry;
                user.TwoFactorFailedAttempts = previousFailedAttempts;
                user.ModifiedAtUtc = previousModifiedAt;
                await ctx.SaveChangesAsync(CancellationToken.None);
                throw;
            }
        }

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
}
