using Market.Domain.Common;

namespace Market.Domain.Entities.Catalog;

public sealed class MovieActorEntity : BaseEntity
{
    public int MovieId { get; set; }
    public MovieEntity? Movie { get; set; }

    public int ActorId { get; set; }
    public ActorEntity? Actor { get; set; }

    public string CharacterName { get; set; } = string.Empty;

    public static class Constraints
    {
        public const int CharacterNameMaxLength = 150;
    }
}