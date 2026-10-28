# Historical MovieShop PostgreSQL preparation checkpoint

This table records the earlier PostgreSQL preparation checkpoint, not the current deployment state. For current live URLs and verification results, see [public-release-status.md](public-release-status.md). Portfolio, HoopsLab, main, and unrelated repositories remain unchanged; no paid services were enabled.

| # | Item | Result |
| --- | --- | --- |
| 1 | Branch / base | deployment/vercel-demo; base dee98045e9650b8e6a3b479b0b444c0b80077523. Main remains 32ac4fa217ada07a029df2b828d7e8f99861d983. |
| 2 | EF provider | Npgsql.EntityFrameworkCore.PostgreSQL 10.0.3; EF/Relational 10.0.14; explicit existing JWT dependency 8.19.2. |
| 3 | Compatibility | bytea binary data; date calendar fields; timestamptz UTC instants; numeric(18,2); native identity/default/index DDL; citext account uniqueness; lowercased text searches. See README. |
| 4 | Migrations | Fresh PostgreSQL baseline and snapshot in PostgreSqlMigrations; seven original SQL Server migrations/snapshot preserved byte-for-byte and excluded from compilation. Exactly one PG baseline selected; no model drift. |
| 5 | Checkout | Scoped async EF store with owner, amount/currency, captured status, creation/expiry; three-hour TTL; bounded concurrency-safe expiry cleanup; unchanged HTTP API. Captured checkpoint persisted across independent application and PostgreSQL restarts. |
| 6 | Demo data | Existing three-movie catalog and related records; idempotent seed; fictional non-staff portfolio-demo; no production admin/development credentials. |
| 7 | Backend build | Restore/Release build/publish passed; production Docker image built successfully. |
| 8 | Frontend build | Production build passed with non-deployable public fixtures; frontend source/UI unchanged. |
| 9 | Automated tests | 26 passed, zero failed/skipped with real PostgreSQL API/schema test databases; separate restart-checkpoint rerun passed 1/1. |
| 10 | Real PostgreSQL | PostgreSQL 17 Docker: migration, seed, registration/confirmation/reset/2FA/login/refresh/logout, catalog/search, cart, ownership/admin, notifications, mocked PayPal, expiry, uniqueness and restart persistence passed. Production container passed 21 local HTTP checks. Hosted Neon PostgreSQL 18 also passed schema/seed/checkout verification and all 21 HTTP checks through a temporary local Production API; temporary resources were cleaned up. |
| 11 | Vercel readiness | Existing pinned .NET 10 non-root container, dynamic PORT, Production, exact CORS, JWT, health, safe logging, rate limits and Sandbox-only PayPal retained. No public deployment yet. |
| 12 | Neon provisioning | Provisioned movieshop-demo on Free free_v3 in fra1 after owner accepted terms. Neon Auth/realtime disabled. Connected to Hobby movieshop-api Production; encrypted pooled ConnectionStrings__Main configured. Direct hosted migration/seed and pooled validation passed. |
| 13 | Manual step | No manual action remains for database provisioning. Owner confirmed terms/account setup completed with no paid plan/card. Later email and PayPal Sandbox account work remains separate. |
| 14 | Files changed | See exact inventory below. |
| 15 | Commit | See deployment branch Git log after validation; requested message Prepare MovieShop PostgreSQL deployment. |
| 16 | Remaining blockers | Database provisioning, hosted initialization, TLS and persistence verification are complete. Hosted application deployment, real email/PayPal Sandbox setup, final frontend/CORS configuration and rendered QA remain. No public deployment exists yet. |

## PostgreSQL implementation changed-file inventory (commit 8f9e5a0)

- `MovieShop.Backend/Market.API/Program.cs`
- `MovieShop.Backend/Market.Application/Abstractions/IPayPalCheckoutSessionStore.cs`
- `MovieShop.Backend/Market.Application/Modules/Catalog/Actors/Queries/List/ListActorsQueryHandler.cs`
- `MovieShop.Backend/Market.Application/Modules/Catalog/Categories/Queries/List/ListCategoriesQueryHandler.cs`
- `MovieShop.Backend/Market.Application/Modules/Catalog/Directors/Queries/List/ListDirectorsQueryHandler.cs`
- `MovieShop.Backend/Market.Application/Modules/Catalog/Movies/Queries/List/ListMoviesQueryHandler.cs`
- `MovieShop.Backend/Market.Application/Modules/Catalog/Reviews/Queries/GetByMovieId/GetReviewsByMovieIdQueryHandler.cs`
- `MovieShop.Backend/Market.Application/Modules/Payments/PayPal/Commands/CaptureOrder/CapturePayPalOrderCommandHandler.cs`
- `MovieShop.Backend/Market.Application/Modules/Payments/PayPal/Commands/CreateOrder/CreatePayPalOrderCommandHandler.cs`
- `MovieShop.Backend/Market.Application/Modules/Payments/PayPal/Queries/GetOrderStatus/GetPayPalOrderStatusQueryHandler.cs`
- `MovieShop.Backend/Market.Domain/Entities/Sales/CheckoutSessionEntity.cs`
- `MovieShop.Backend/Market.Infrastructure/Common/DatabasePayPalCheckoutSessionStore.cs`
- `MovieShop.Backend/Market.Infrastructure/Common/MemoryPayPalCheckoutSessionStore.cs`
- `MovieShop.Backend/Market.Infrastructure/Common/PostgresConnection.cs`
- `MovieShop.Backend/Market.Infrastructure/Database/Configurations/Identity/UserEntityConfiguration.cs`
- `MovieShop.Backend/Market.Infrastructure/Database/Configurations/Sales/CheckoutSessionConfiguration.cs`
- `MovieShop.Backend/Market.Infrastructure/Database/DatabaseConfiguration.cs`
- `MovieShop.Backend/Market.Infrastructure/Database/DatabaseContext.cs`
- `MovieShop.Backend/Market.Infrastructure/Database/DeploymentContextFactory.cs`
- `MovieShop.Backend/Market.Infrastructure/DatabaseInitializer.cs`
- `MovieShop.Backend/Market.Infrastructure/DependencyInjection.cs`
- `MovieShop.Backend/Market.Infrastructure/Market.Infrastructure.csproj`
- `MovieShop.Backend/Market.Infrastructure/PostgreSqlMigrations/20261028102940_PostgreSqlDeploymentBaseline.Designer.cs`
- `MovieShop.Backend/Market.Infrastructure/PostgreSqlMigrations/20261028102940_PostgreSqlDeploymentBaseline.cs`
- `MovieShop.Backend/Market.Infrastructure/PostgreSqlMigrations/DatabaseContextModelSnapshot.cs`
- `MovieShop.Backend/Market.Tests/DeploymentSafetyTests.cs`
- `MovieShop.Backend/Market.Tests/RelationalDeploymentTests.cs`
- `deployment/README.md`
- `deployment/preparation-status.md`

See [runbook](README.md) for configuration, compatibility changes and the manual Marketplace gate.

## Provisioning follow-up

Only this report and README were updated after provisioning; application source remains at the validated PostgreSQL implementation. No secrets, generated skills, or local environment files are included. Vercel API project is prepared without a public deployment; Neon remains on Free.

Latest runtime/demo preparation and outstanding manual provider steps are recorded in [public-release-status.md](public-release-status.md). No public application deployment is verified.
