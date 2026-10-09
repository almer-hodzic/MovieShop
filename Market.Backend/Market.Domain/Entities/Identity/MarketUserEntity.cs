// MarketUserEntity.cs
using Market.Domain.Common;

namespace Market.Domain.Entities.Identity;

public sealed class MarketUserEntity : BaseEntity
{
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public byte[]? ProfileImage { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsManager { get; set; }
    public bool IsEmployee { get; set; }
    public int TokenVersion { get; set; } = 0; // For global revocation
    public bool IsEnabled { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? EmailConfirmationTokenHash { get; set; }
    public DateTime? EmailConfirmationTokenExpiresAtUtc { get; set; }
    public DateTime? EmailConfirmedAtUtc { get; set; }
    public string? PasswordResetTokenHash { get; set; }
    public DateTime? PasswordResetTokenExpiresAtUtc { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public string? TwoFactorCodeHash { get; set; }
    public DateTime? TwoFactorCodeExpiresAtUtc { get; set; }
    public int TwoFactorFailedAttempts { get; set; }
    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
}
