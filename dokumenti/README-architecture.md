# MovieShop Architecture

MovieShop uses a layered Clean Architecture backend and a modular Angular frontend.

## Backend Layers

- `Market.Domain`: domain entities for movies, categories, actors, directors, reviews, favourites, notifications, users, refresh tokens, shopping carts, and cart items.
- `Market.Application`: CQRS commands/queries, DTOs, validators, and abstractions such as current user, email, PayPal, and database context contracts.
- `Market.Infrastructure`: EF Core SQL Server context, entity configurations, migrations, seeders, JWT token generation, PayPal service, email implementations, and exception/current-user infrastructure.
- `Market.API`: HTTP controllers, Swagger, JWT authentication, authorization policies, CORS, request/response logging, exception handling, and application startup.

## CQRS Flow

Controllers receive HTTP requests and send command/query objects to application handlers. Handlers use `IAppDbContext` and injected abstractions, then return DTOs. Validation is performed with FluentValidation through the application validation behavior before handlers mutate state.

## Authentication

Authentication uses JWT access tokens and refresh tokens. The API validates issuer, audience, signing key, and token lifetime. Admin-only endpoints use the `AdminOnly` policy, which requires the `is_admin=true` claim.

## Frontend

The Angular app is non-standalone and module-based. `AppRoutingModule` lazy-loads `admin`, `auth`, `client`, and `public` modules. Feature modules use API service classes, Reactive Forms, route guards, and shared UI components for MovieShop workflows.
