using Market.Application.Abstractions;

namespace Market.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    public DbSet<ActorEntity> Actors => Set<ActorEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<DirectorEntity> Directors => Set<DirectorEntity>();
    public DbSet<FavouriteMovieEntity> FavouriteMovies => Set<FavouriteMovieEntity>();
    public DbSet<MovieEntity> Movies => Set<MovieEntity>();
    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();
    public DbSet<UserNotificationEntity> UserNotifications => Set<UserNotificationEntity>();
    public DbSet<ShoppingCartEntity> ShoppingCarts => Set<ShoppingCartEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();
    public DbSet<MovieActorEntity> MovieActors => Set<MovieActorEntity>();
    public DbSet<MovieCategoryEntity> MovieCategories => Set<MovieCategoryEntity>();
    public DbSet<MarketUserEntity> Users => Set<MarketUserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    private readonly TimeProvider _clock;
    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock) : base(options)
    {
        _clock = clock;
    }
}
