using Market.Domain.Common;

namespace Market.Domain.Entities.Catalog;

public sealed class ActorEntity : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public byte[]? Photo { get; set; }
    public DateTime BirthDate { get; set; }
    public int CountryId { get; set; }
    public string? ImdbLink { get; set; }
    public string? Biography { get; set; }

    public static class Constraints
    {
        public const int FirstNameMaxLength = 100;
        public const int LastNameMaxLength = 100;
        public const int ImdbLinkMaxLength = 500;
        public const int BiographyMaxLength = 1000;
    }
}