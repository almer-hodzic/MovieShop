namespace Market.Application.Modules.Auth.Commands.Login;

/// <summary>
/// Represents a pair of tokens (access + refresh) that the client receives upon login or token refresh.
/// </summary>
public sealed class LoginCommandDto
{
    public int UserId { get; set; }

    public bool RequiresTwoFactor { get; set; }
    public bool EmailDeliveryFallbackUsed { get; set; }
    public string? EmailDeliveryMessage { get; set; }

    /// <summary>
    /// JWT access token – used for authorized API calls.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token that the client stores locally and uses to obtain a new access token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Expiration time of the access token in UTC format.
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }
}
