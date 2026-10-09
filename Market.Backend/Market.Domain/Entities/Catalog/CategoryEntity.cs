using Market.Domain.Common;

namespace Market.Domain.Entities.Catalog;

public sealed class CategoryEntity : BaseEntity
{
    public string CategoryName { get; set; } = string.Empty;

    public static class Constraints
    {
        public const int CategoryNameMaxLength = 100;
    }
}