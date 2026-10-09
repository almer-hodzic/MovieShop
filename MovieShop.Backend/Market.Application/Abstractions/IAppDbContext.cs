namespace Market.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    DbSet<ActorEntity> Actors { get; }
    DbSet<CategoryEntity> Categories { get; }
    DbSet<DirectorEntity> Directors { get; }
    DbSet<FavouriteMovieEntity> FavouriteMovies { get; }
    DbSet<MovieEntity> Movies { get; }
    DbSet<ReviewEntity> Reviews { get; }
    DbSet<NotificationEntity> Notifications { get; }
    DbSet<UserNotificationEntity> UserNotifications { get; }
    DbSet<ShoppingCartEntity> ShoppingCarts { get; }
    DbSet<CartItemEntity> CartItems { get; }
    DbSet<MovieActorEntity> MovieActors { get; }
    DbSet<MovieCategoryEntity> MovieCategories { get; }
    DbSet<MarketUserEntity> Users { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
