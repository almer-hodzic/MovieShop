namespace Market.Shared.Options;

public sealed class DevelopmentEmailOptions
{
    public const string SectionName = "DevelopmentEmail";

    public bool Enabled { get; init; }
    public string? Host { get; init; }
    public int Port { get; init; } = 587;
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? FromEmail { get; init; }
    public string FromName { get; init; } = "MovieShop";
    public bool UseSsl { get; init; } = true;

    public bool HasRequiredSmtpSettings()
    {
        return Enabled &&
               !string.IsNullOrWhiteSpace(Host) &&
               Port > 0 &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(FromEmail);
    }
}
