namespace Market.Application.Modules.Auth.Commands.ConfirmEmail;

using System.Security.Cryptography;
using System.Text;

public sealed class ConfirmEmailCommandHandler(IAppDbContext ctx)
    : IRequestHandler<ConfirmEmailCommand, ConfirmEmailCommandDto>
{
    public async Task<ConfirmEmailCommandDto> Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var tokenHash = HashToken(request.Token.Trim());

        var user = await ctx.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email && !x.IsDeleted, ct)
            ?? throw new MarketNotFoundException("User with this email does not exist.");

        if (user.IsEmailConfirmed)
        {
            return new ConfirmEmailCommandDto
            {
                Id = user.Id,
                Email = user.Email,
                IsEmailConfirmed = true,
                EmailConfirmedAtUtc = user.EmailConfirmedAtUtc ?? DateTime.UtcNow
            };
        }

        if (string.IsNullOrWhiteSpace(user.EmailConfirmationTokenHash) ||
            user.EmailConfirmationTokenHash != tokenHash)
            throw new MarketConflictException("Invalid email confirmation token.");

        if (user.EmailConfirmationTokenExpiresAtUtc is null ||
            user.EmailConfirmationTokenExpiresAtUtc < DateTime.UtcNow)
            throw new MarketConflictException("Email confirmation token has expired.");

        var confirmedAt = DateTime.UtcNow;

        user.IsEmailConfirmed = true;
        user.EmailConfirmedAtUtc = confirmedAt;
        user.EmailConfirmationTokenHash = null;
        user.EmailConfirmationTokenExpiresAtUtc = null;
        user.ModifiedAtUtc = confirmedAt;

        await ctx.SaveChangesAsync(ct);

        return new ConfirmEmailCommandDto
        {
            Id = user.Id,
            Email = user.Email,
            IsEmailConfirmed = user.IsEmailConfirmed,
            EmailConfirmedAtUtc = confirmedAt
        };
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
