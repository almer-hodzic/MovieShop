using Market.Domain.Common;

namespace Market.Domain.Entities.Catalog;

public sealed class DirectorEntity : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }

    public static class Constraints
    {
        public const int FirstNameMaxLength = 100;
        public const int LastNameMaxLength = 100;
    }
}