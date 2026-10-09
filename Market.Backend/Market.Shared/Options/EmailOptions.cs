using System.ComponentModel.DataAnnotations;

namespace Market.Shared.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    [Required] public string ApiKey { get; init; } = default!;
    [Required, EmailAddress] public string FromEmail { get; init; } = default!;
    [Required] public string FromName { get; init; } = "MovieShop";
    [Required, Url] public string FrontendBaseUrl { get; init; } = default!;
}
