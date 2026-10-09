using Market.Domain.Common;

namespace Market.Domain.Entities.Catalog;

public sealed class MovieEntity : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public int Duration { get; set; }
    public int DirectorId { get; set; }
    public DirectorEntity? Director { get; set; }
    public int CountryId { get; set; }
    public string? TrailerLink { get; set; }
    public byte[]? Image { get; set; }
    public string StoryLine { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? AverageScore { get; set; }

    public ICollection<MovieCategoryEntity> MovieCategories { get; set; } = new List<MovieCategoryEntity>();
    public ICollection<MovieActorEntity> MovieActors { get; set; } = new List<MovieActorEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();

    public static class Constraints
    {
        public const int TitleMaxLength = 200;
        public const int TrailerLinkMaxLength = 500;
        public const int StoryLineMaxLength = 4000;
    }
}
