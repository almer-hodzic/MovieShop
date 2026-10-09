# MovieShop

MovieShop is an RS1 full-stack web application for browsing movies, managing favourites and a shopping cart, and completing sandbox checkout through PayPal. The administrative area manages the movie catalog, people, reviews, notifications, dashboard statistics, and account settings.

## Architecture

The backend is organized as a Clean Architecture solution:

- `Market.Domain` contains entities for identity, movies, categories, actors, directors, reviews, notifications, favourites, shopping carts, cart items, and refresh tokens.
- `Market.Application` contains CQRS commands and queries, validators, DTOs, and service abstractions.
- `Market.Infrastructure` contains EF Core, SQL Server configuration, migrations, database initialization, seeding, JWT, PayPal, email delivery, and current-user infrastructure.
- `Market.API` contains controllers, authentication/authorization, Swagger, CORS, exception handling, and request logging.
- `Market.Tests` contains integration and flow tests for active MovieShop features.

The frontend is an Angular non-standalone application. It uses modules, lazy-loaded routes, API service classes, Reactive Forms with validators, route guards, and HTTP interceptors/services for authenticated workflows.

## Technologies

- ASP.NET Core Web API
- Entity Framework Core with SQL Server
- MediatR-style CQRS
- FluentValidation
- JWT access tokens and refresh tokens
- Angular 21
- Angular Material
- Reactive Forms
- PayPal sandbox integration
- SendGrid or development email fallback
- xUnit integration tests

## Repository Structure

```text
MovieShop/
  MovieShop.Backend/
    Market.Backend.sln
    Market.API/
    Market.Application/
    Market.Domain/
    Market.Infrastructure/
    Market.Shared/
    Market.Tests/
  MovieShop.Frontend/
    angular.json
    package.json
    src/
  db-backups/
  dokumenti/
  README.md
```

## Backend Startup

```powershell
cd MovieShop.Backend
dotnet restore
dotnet build
dotnet run --project Market.API
```

The API reads `appsettings.json`, `appsettings.Development.json`, environment variables, and development User Secrets. Do not store real PayPal, SendGrid, SMTP, JWT, or database passwords in tracked files.

## Frontend Startup

```powershell
cd MovieShop.Frontend
npm install
npm start
```

The Angular app runs on `http://localhost:4200` and expects the backend API URL configured in `src/environments/environment*.ts`.

## SQL Server and Database

The default connection string targets:

```text
Server=localhost;Database=MovieShopDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

Create or restore `MovieShopDb` in SQL Server before running the API. In non-development environments, startup applies EF Core migrations by default. In Development, `appsettings.Development.json` currently sets:

```json
{
  "DatabaseStartup": {
    "ApplyMigrationsOnStartup": false,
    "SeedOnStartup": false
  }
}
```

That means development startup does not automatically modify the database unless those flags are changed.

## Seeded Accounts

When development/test seeding is enabled, the application seeds:

```text
Admin:
admin@market.local
Admin123!

User:
user@market.local
User123!
```

## Build and Test Commands

```powershell
cd MovieShop.Backend
dotnet build
dotnet test

cd ..\MovieShop.Frontend
npm run build
```

## Feature Overview

- Authentication: register, email confirmation, login, JWT, refresh token, forgot password, reset password, and two-factor verification.
- Public MovieShop: welcome page, movie browse, movie details, favourites, shopping cart, checkout, profile, and video player.
- Payments: PayPal sandbox order creation and capture.
- Notifications: admin creates notifications, users receive them, mark read/unread, and delete.
- Admin: dashboard, settings, movies, categories, actors, directors, reviews, notifications, favourites, and shopping-cart administration.

## Email Development Behavior

Development email can use a complete local SMTP configuration, SendGrid when a real API key is provided, or a console/log fallback. Tracked configuration contains placeholders only.

## PayPal Sandbox Note

PayPal is configured for sandbox mode by default. Set `PayPal:ClientId` and `PayPal:ClientSecret` through User Secrets or environment variables, never in tracked source.

## Known Limitations

- Development startup currently skips migrations and seeding unless explicitly enabled.
- PayPal uses sandbox endpoints only.
- Email delivery in development may fall back to logs when SMTP or SendGrid is not configured.
- Old EF migrations may still contain historical table names from the template, but active controllers/modules/entities are MovieShop-focused.
