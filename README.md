# MovieShop

MovieShop is a full-stack movie catalog and shopping application migrated to the RS1 2025/26 architecture using Clean Architecture, CQRS, JWT authentication, FluentValidation, Angular modular architecture, and Reactive Forms.

## Project Structure

```text
MovieShop.Backend/
MovieShop.Frontend/
db-backups/
dokumenti/
README.md
```

## Backend

- ASP.NET Core
- Clean Architecture
- CQRS
- FluentValidation
- Entity Framework Core
- JWT authentication with refresh tokens
- Role-based authorization
- SQL Server

## Frontend

- Angular
- Non-standalone modular architecture
- Lazy-loaded modules
- Reactive Forms with validators

## Main Features

- Registration with username
- Login by email or username
- Email confirmation
- Forgot/reset password
- Two-factor authentication
- User profile
- Movies, categories, actors and directors
- Reviews and rating
- Favourites
- Shopping cart
- Checkout
- PayPal Sandbox
- Notifications
- Admin dashboard
- Admin settings

## Local Development Accounts

```text
Admin:
username: admin
email: admin@market.local
password: Admin123!

User:
username: user
email: user@market.local
password: User123!
```

Login accepts either username or email.

## Local Running

Backend:

```powershell
cd MovieShop.Backend
dotnet restore
dotnet build
dotnet run --project Market.API
```

Final verified development backend URL:

```text
http://localhost:5177
```

Frontend:

```powershell
cd MovieShop.Frontend
npm install
npm start
```

Frontend URL:

```text
http://localhost:4200
```

## Database

Database name:

```text
MovieShopDb
```

Backup:

```text
db-backups/MovieShopDb_final.bak
```

Restore the backup before testing if the local database is missing or outdated. Development startup does not automatically apply migrations or seed data. EF Core migrations are included, including the latest username migration.

## Integrations

PayPal Sandbox is used for checkout. The PayPal ClientSecret is not committed, and local testing requires valid sandbox configuration through User Secrets or environment variables. Buyer approval requires a PayPal Personal sandbox test account.

Email confirmation, password reset and 2FA require a configured development email provider. No email credentials or external provider secrets are committed.

## Tests

The final verified state:

- Backend build passes
- Automated backend test suite passes 9/9
- Frontend build passes

Commands:

```powershell
cd MovieShop.Backend
dotnet build
dotnet test --no-build

cd ..\MovieShop.Frontend
npm run build
```

## Documentation

`dokumenti/` contains architecture documentation, database schema notes, business flow documentation, and Mermaid diagrams.

## Limitations and Notes

- No persisted order history/module is implemented.
- PayPal checkout session ownership is in-memory.
- Notifications are refresh/reload based, not SignalR/live.
- External provider secrets are not stored in source control.
