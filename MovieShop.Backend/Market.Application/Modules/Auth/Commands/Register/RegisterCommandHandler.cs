namespace Market.Application.Modules.Auth.Commands.Register;

using System.Security.Cryptography;
using System.Text;

public sealed class RegisterCommandHandler(
    IAppDbContext ctx,
    IPasswordHasher<MarketUserEntity> hasher,
    IAuthEmailService emailService)
    : IRequestHandler<RegisterCommand, RegisterCommandDto>
{
    public async Task<RegisterCommandDto> Handle(RegisterCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();
        var normalizedUsername = username.ToLowerInvariant();
        var firstname = request.Firstname.Trim();
        var lastname = request.Lastname.Trim();

        var emailExists = await ctx.Users
            .AnyAsync(x => !x.IsDeleted && x.Email.ToLower() == email, ct);

        if (emailExists)
            throw new MarketConflictException("Email already exists.");

        var usernameExists = await ctx.Users
            .AnyAsync(x => !x.IsDeleted && x.Username.ToLower() == normalizedUsername, ct);

        if (usernameExists)
            throw new MarketConflictException("Username already exists.");

        var confirmationToken = GenerateConfirmationToken();

        var user = new MarketUserEntity
        {
            Firstname = firstname,
            Lastname = lastname,
            Username = username,
            Email = email,
            IsAdmin = false,
            IsManager = false,
            IsEmployee = true,
            IsEnabled = true,
            IsEmailConfirmed = false,
            IsTwoFactorEnabled = true,
            EmailConfirmationTokenHash = HashToken(confirmationToken),
            EmailConfirmationTokenExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            CreatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = hasher.HashPassword(user, request.Password);

        ctx.Users.Add(user);

        var emailDelivery = await emailService.SendEmailConfirmationAsync(
            user.Email,
            $"{user.Firstname} {user.Lastname}".Trim(),
            confirmationToken,
            ct);

        await ctx.SaveChangesAsync(ct);

        return new RegisterCommandDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            IsEnabled = user.IsEnabled,
            IsEmployee = user.IsEmployee,
            IsEmailConfirmed = user.IsEmailConfirmed,
            EmailDeliveryFallbackUsed = emailDelivery.FallbackUsed,
            EmailDeliveryMessage = emailDelivery.Message
        };
    }

    private static string GenerateConfirmationToken()
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
