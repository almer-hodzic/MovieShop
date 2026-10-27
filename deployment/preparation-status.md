# MovieShop deployment preparation status

No public deployment has been verified. Portfolio, HoopsLab, main, and unrelated repositories are untouched.

| # | Item | Result |
| --- | --- | --- |
| 1 | Branch / base | deployment/vercel-demo / 32ac4fa217ada07a029df2b828d7e8f99861d983; main unchanged. |
| 2 | Runtime | .NET 10 LTS, EF/auth packages 10.0.14; existing architecture retained. |
| 3 | Frontend strategy | Angular 21.2.25 runtime/compiler, CLI/build 21.2.28; public Vercel build environment generates ignored production config. Material/CDK and unrelated direct dependencies retain their old locked versions. |
| 4 | Backend strategy | Pinned official .NET container, Release publish, non-root, dynamic PORT, Production; not deployed. |
| 5 | Database strategy | Azure SQL free offer, existing SQL Server provider; no paid resources. |
| 6 | Azure SQL | Pending owner sign-in and eligible existing subscription; no resources created. |
| 7 | Migrations | All seven applied successfully to fresh private SQL Server. Missing discovery attributes on migration seven restored; IDs/operations retained; no pending model changes. |
| 8 | Demo data | Idempotent catalog-only seed: three movies plus genres/directors/actors/relationships; existing frontend assets reused. |
| 9 | Demo account | Dedicated fictional portfolio-demo account, confirmed, enabled, non-admin/non-manager/non-employee; unique secret supplied only for explicit initialization; not publicly provisioned. |
| 10 | JWT/auth | Environment-only production settings; local login/refresh/logout/admin restrictions passed; registration/confirmation/reset/2FA covered by existing automated tests with a test email provider. Public verification pending. |
| 11 | Email | SendGrid preserved; no code disclosure or authentication bypass. Account/verified sender pending. Current no-card free offer is a 60-day trial; ongoing no-cost delivery unresolved. |
| 12 | PayPal | Sandbox-only startup validation and existing service guard; mocked automated checkout passed; real Sandbox credentials/buyer approval pending. |
| 13 | Checkout compatibility | Existing memory store unchanged pending owner decision. Vercel multi-instance/restart behavior is a public-release blocker; SQL persistence proposed. |
| 14 | CORS | Exact HTTPS origins required in Production; local production allowed/disallowed preflights passed; final origin not assigned. |
| 15 | Health | Anonymous /health with dedicated short-timeout SQL connection and SELECT 1; healthy locally; outage returned a sanitized 503 in 5.062 seconds, then recovered to 200. |
| 16 | Logging/security | Production stdout only; broad bodies/provider bodies removed; sanitized error logs/responses; no generated secrets or request bodies found in sampled production logs. Payment status now enforces ownership. |
| 17 | Rate limits | Production auth 30/minute per observed IP; payment POST 10/minute per user, per instance. Auth 429 verified locally; proxy/shared-IP and multi-instance limits documented. |
| 18 | Secrets scan | Tracked configuration uses placeholders; high-confidence private-key/GitHub/SendGrid token scan across 61 reachable commits found no matches. Known dev/test fixtures are not public-demo credentials. Generated local secrets and build config ignored/excluded. Historical secret-setting review also found only placeholders. Final NuGet audit: no known vulnerable packages in six projects; production npm audit: zero findings after fixing seven high-severity Angular package findings. |
| 19 | Frontend build | npm ci and npm run build -- --configuration production passed with non-deployable fixture origin/client ID. Existing Sass deprecation/browser-mapping notices remain. |
| 20 | Backend build | Restore/Release build/publish passed in .NET 10 SDK and production Docker build; existing nullability warnings remain. |
| 21 | Automated tests | 24 passed, zero failures/skips with the SQL test connection supplied; 11 existing tests included. Final rerun after readiness change also passed. |
| 22 | Files changed | See inventory below; no UI/layout/style files changed. |
| 23 | Commit | See deployment/vercel-demo Git log after validation; no main merge authorized. |
| 24 | Deployment | Not started; Vercel authenticated on Hobby. No new Vercel project or paid service created. |
| 25 | Backend HTTPS URL | Not assigned. Local-only production review: http://localhost:5188. |
| 26 | Frontend HTTPS URL | Not assigned; build artifacts are fixtures and must not be deployed. |
| 27 | Production smoke tests | Public HTTPS not run. Local production container passed 21 HTTP checks; external email/PayPal not called. |
| 28 | Rendered desktop QA | Unverified; no browser automation required for this preparation checkpoint. |
| 29 | Rendered mobile QA | Unverified; UI/layout unchanged. |
| 30 | Git status | Preparation kept on deployment/vercel-demo; final status/commit reported in chat. Ignored local verification files contain generated test secrets. |
| 31 | Remaining gates | Azure account/free offer, checkout persistence decision, email account/durable free-delivery decision, PayPal Sandbox setup, hosted smoke tests and rendered QA. |

## Changed-file inventory

- `.gitignore`
- `MovieShop.Backend/.dockerignore`
- `MovieShop.Backend/.vercelignore`
- `MovieShop.Backend/Dockerfile.vercel`
- `MovieShop.Backend/Market.API/DeploymentProtection.cs`
- `MovieShop.Backend/Market.API/Market.API.csproj`
- `MovieShop.Backend/Market.API/Program.cs`
- `MovieShop.Backend/Market.API/appsettings.Production.json`
- `MovieShop.Backend/Market.Application/Market.Application.csproj`
- `MovieShop.Backend/Market.Application/Modules/Payments/PayPal/Queries/GetOrderStatus/GetPayPalOrderStatusQueryHandler.cs`
- `MovieShop.Backend/Market.Domain/Market.Domain.csproj`
- `MovieShop.Backend/Market.Infrastructure/Common/MarketExceptionHandler.cs`
- `MovieShop.Backend/Market.Infrastructure/Common/PayPalService.cs`
- `MovieShop.Backend/Market.Infrastructure/Common/SendGridAuthEmailService.cs`
- `MovieShop.Backend/Market.Infrastructure/Database/Seeders/DynamicDataSeeder.cs`
- `MovieShop.Backend/Market.Infrastructure/Database/Seeders/PublicDemoSeeder.cs`
- `MovieShop.Backend/Market.Infrastructure/DatabaseInitializer.cs`
- `MovieShop.Backend/Market.Infrastructure/DependencyInjection.cs`
- `MovieShop.Backend/Market.Infrastructure/Market.Infrastructure.csproj`
- `MovieShop.Backend/Market.Infrastructure/Migrations/20261010010000_AddSaveForLaterToCartItems.cs`
- `MovieShop.Backend/Market.Shared/Market.Shared.csproj`
- `MovieShop.Backend/Market.Tests/DeploymentSafetyTests.cs`
- `MovieShop.Backend/Market.Tests/Market.Tests.csproj`
- `MovieShop.Backend/Market.Tests/PayPalFlowTests.cs`
- `MovieShop.Backend/Market.Tests/RelationalDeploymentTests.cs`
- `MovieShop.Backend/vercel.json`
- `MovieShop.Frontend/package-lock.json`
- `MovieShop.Frontend/package.json`
- `MovieShop.Frontend/scripts/configure-production.mjs`
- `MovieShop.Frontend/src/environments/environment.prod.ts`
- `MovieShop.Frontend/vercel.json`
- `deployment/README.md`
- `deployment/preparation-status.md`

See [deployment runbook](README.md) for configuration, account steps, and release gates.
