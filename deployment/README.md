# MovieShop public-demo deployment

Status: preparation only. No public deployment is verified. Do not add a Portfolio link yet.
Branch: `deployment/vercel-demo`; base: `32ac4fa217ada07a029df2b828d7e8f99861d983`.

## Release gates

- Resolve checkout persistence before deploying on Vercel. The existing in-memory store expires after three hours, does not survive restarts, and is not shared across instances. A small SQL-backed store is proposed; no store architecture change has been made pending the user's answer. Do not publish an unreliable checkout.
- Azure authentication and an eligible existing subscription are required. Use the Azure SQL free offer only, with **Auto-pause the database until next month**; never select additional charges. Stop before subscription creation, billing, or payment authorization.
- SendGrid verification and a usable account are required. Its current free offer is a 60-day trial, not ongoing free delivery. Do not enable a paid plan automatically, publish verification codes, bypass confirmation/2FA, or switch the deployed application to Development. A durable no-cost email solution still requires a decision if the trial is unsuitable.
- Obtain PayPal **Sandbox** app credentials and a Sandbox buyer account through the owner's Developer Dashboard. Never use Live credentials. Do not paste credentials into chat.

## Runtime and project roots

The frontend Angular runtime/compiler packages are pinned to 21.2.25, and its CLI/build tools to 21.2.28, resolving seven high-severity production dependency findings without moving off Angular 21. Material/CDK remain 21.0.1 and every unrelated direct dependency stays at its previous locked version. Production npm audit reports zero vulnerabilities.

The six backend projects target .NET 10 LTS, with EF Core and authentication packages 10.0.14. The container uses official SDK/runtime images pinned by digest, Release publish, an unprivileged runtime user, and HTTP on `$PORT` behind Vercel TLS. A direct Pkcs 10.0.14 reference overrides a vulnerable SQL-client transitive dependency found during restore.

Backend Vercel project root: `MovieShop.Backend`, framework `container`, region `fra1`, `Dockerfile.vercel`.
Frontend Vercel project root: `MovieShop.Frontend`, framework `angular`, output `dist/movieshop-frontend/browser`.
Do not create either public deployment until the gates above are resolved. Keep Hobby; no paid resources are authorized.

## Backend provider configuration

Store sensitive values as encrypted Vercel environment values; never in Git, Docker arguments recorded in documentation, frontend assets, or logs.

| Environment key | Purpose |
| --- | --- |
| `ConnectionStrings__Main` | Azure SQL connection string; encrypted, validate the server certificate |
| `Jwt__Issuer`, `Jwt__Audience` | Exact deployed API/frontend identifiers |
| `Jwt__Key` | Cryptographically random signing secret, at least 32 characters; encrypted |
| `Jwt__AccessTokenMinutes` | Suggested 20 |
| `Jwt__RefreshTokenDays` | Suggested 14 |
| `PayPal__Mode` | `sandbox` only |
| `PayPal__BaseUrl` | `https://api-m.sandbox.paypal.com` only |
| `PayPal__ClientId` | Sandbox app client ID (public identifier) |
| `PayPal__ClientSecret` | Sandbox app secret; encrypted |
| `PayPal__CurrencyCode` | `USD` |
| `Email__ApiKey` | SendGrid Mail Send credential; encrypted |
| `Email__FromEmail`, `Email__FromName` | Verified sender and MovieShop name |
| `Email__FrontendBaseUrl` | Exact HTTPS frontend origin for confirmation/reset callbacks |
| `Cors__AllowedOrigins__0` | Exact HTTPS frontend origin, no wildcard/path/trailing slash |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `PORT` | Supplied by platform; local container default 8080 |

No production secret defaults are supplied. Exact CORS is required at startup. The email and payment integrations retain their real providers; local fixtures are not deployable credentials.

## Database initialization

Normal production startup does **not** apply migrations or seed. Run a single controlled initialization against the intended empty demo database before starting application instances:

```text
dotnet Market.API.dll --initialize-database
```

For this one-off operation only, set `PublicDemo__Seed=true` and `PublicDemo__Password` to a newly generated, nonpersonal passphrase of at least 16 characters in secret storage. The command applies pending migrations and seeds, then exits. Remove seed/password configuration from the running API afterward. Use a migration identity with DDL permissions for initialization and a restricted application identity for runtime where practical.

All seven original migration IDs and SQL operations are retained. The seventh migration (`20261010010000_AddSaveForLaterToCartItems`) lacked discovery attributes, so it was never recognized by EF; those attributes are restored. The existing snapshot matches EF Core 10 without a new schema migration.

Public seeding is explicit and idempotent: three existing catalog movies, genres, directors, actors, relationships, existing frontend image mappings, and one fictional account `portfolio-demo` / `demo@movieshop.invalid`. It never calls the development-user seed, creates staff/admin accounts, resets an existing user's password, or clears their security settings. It rejects a conflicting or elevated demo identity. Existing login UI is unchanged; document the public demo passphrase after verification, without reusing development credentials.

## Frontend build

```text
npm ci
npm run build -- --configuration production
```

Set these **public** Vercel build values first:

- `MOVIESHOP_API_URL`: final HTTPS API origin, without a path, query, credentials, or fragment.
- `MOVIESHOP_PAYPAL_SANDBOX_CLIENT_ID`: public client ID of the same Sandbox app configured on the API.

The prebuild generates ignored `src/environments/environment.production.generated.ts`. No client secret is read by the generator. Missing configuration fails the build. The local validation build used `https://api.movieshop.invalid` and an explicitly marked public Sandbox fixture; those artifacts must never be deployed.

## Security and checks

Production uses console logs without broad request/response bodies or provider error bodies. Unexpected error responses omit exception details. Request failure logs retain a trace ID and exception type without personal identifiers or exception messages. `/health` is anonymous, checks database connectivity with a dedicated non-pooled connection (three-second connection/command timeouts, no connect retries, five-second cancellation budget), and returns only healthy/503. It is a connectivity check, not a migration substitute.

Production per-instance limits: auth routes 30 requests/minute per observed remote IP; payment POST routes 10/minute per authenticated user. No request queue. Retry-After is 60 seconds. Vercel may present a shared proxy IP and multiple instances have separate limiter state; these are lightweight demo protections, not a distributed abuse-control service. Do not trust arbitrary forwarded IP headers.

Payment capture and status require session ownership. Sandbox/live endpoint restrictions are validated at startup. CORS permits only configured exact frontend origins. No auth bypass or public admin is introduced.

Backend checks (SDK 10 required, or the pinned SDK container):

```text
dotnet restore MovieShop.Backend/Market.Backend.sln
dotnet build MovieShop.Backend/Market.Backend.sln -c Release
dotnet test MovieShop.Backend/Market.Backend.sln -c Release
```

For the opt-in relational test, supply `MOVIESHOP_TEST_SQL` using a fresh private SQL Server database whose name starts with `MovieShopDeploymentTest`. The test applies all migrations, seeds twice, verifies non-staff roles and catalog relationships, and checks for pending migrations. Without this variable, that test is reported skipped. Never point it at a production database.

Sources:
- https://dotnet.microsoft.com/en-us/platform/support/policy
- https://vercel.com/changelog/bring-your-dockerfile-to-vercel-functions
- https://vercel.com/kb/guide/docker-on-vercel-vs-render
- https://learn.microsoft.com/en-us/azure/azure-sql/database/free-offer
- https://www.twilio.com/docs/sendgrid/ui/account-and-settings/upgrading-your-plan
- https://developer.paypal.com/api/get-started/

## Validated local checkpoint

- Angular locked install and production build passed (fixture API/client ID only).
- Final backend suite: 24 passed, zero failed/skipped, including all seven fresh-SQL migrations and repeated public seeding.
- Final production container: Release build/publish passed; runs as UID 1654.
- Local production HTTP suite: 21 checks passed. Database outage readiness returned 503 in 5.062 seconds and recovered to 200 after the test database restarted.
- Generated SQL password, JWT key, demo password, email address, bearer tokens, and password-body fields were absent from the sampled production runtime logs.
- No real provider secrets, email deliveries, payment approvals, public hosting resources, or paid resources were used in these checks.

Final NuGet vulnerability audit reports no known vulnerable packages across all six backend projects.
