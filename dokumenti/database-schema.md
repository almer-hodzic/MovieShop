# MovieShop Database Schema

The current EF Core `DatabaseContext` exposes the active MovieShop tables below.

## Identity

- `Users`: application users with email, password hash, roles/flags, email confirmation, password reset, and two-factor fields.
- `RefreshTokens`: refresh token records connected to users.

## Catalog

- `Movies`: movie title, director, duration, release date, price, country, trailer, storyline, and soft-delete metadata.
- `Categories`: movie category names.
- `Actors`: actor profile data.
- `Directors`: director profile data.
- `MovieActors`: many-to-many link between movies and actors, including character name.
- `MovieCategories`: many-to-many link between movies and categories.
- `Reviews`: user reviews for movies.
- `FavouriteMovies`: user favourite movie links.

## Sales

- `ShoppingCarts`: user cart aggregate.
- `CartItems`: movie line items in a cart.

## Notifications

- `Notifications`: notifications created by administrators.
- `UserNotifications`: per-user notification delivery/read/delete state.

## Migrations

Migrations live in `Market.Infrastructure/Migrations`. The active model snapshot is MovieShop-oriented, though historical migration files may include old template table names because they record earlier schema history.
