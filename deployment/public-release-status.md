# MovieShop public release checkpoint

**NOT PUBLICLY DEPLOYED OR VERIFIED.** Waiting at the owner account/credential steps explicitly required by the release instructions. No paid services enabled.

| # | Item | Result |
| --- | --- | --- |
| 1 | Branch / commit | deployment/vercel-demo; base 04b72847c2c384947c8939261295a438c264302d; see branch Git log for final preparation commit. |
| 2 | Backend configuration | Neon Main retained; JWT generated/stored encrypted; issuer/audience/lifetimes, exact CORS, Sandbox-only mode/USD, email name/callback and Production configured. All required provider keys are configured encrypted; existing free SendGrid account/sender and Sandbox OAuth verified. |
| 3 | Database startup | ApplyMigrationsOnStartup=false; SeedOnStartup=false; PublicDemo:Seed=false. Existing Neon schema/data retained; deliberate one-off migrations/seed only. |
| 4 | Email setup | Existing configured sender verified, account reports free with 100 available sends before QA; API key configured encrypted. Real delivery/confirmation/reset/2FA verification pending. |
| 5 | PayPal setup | Mode sandbox, Sandbox API endpoint/USD and rejection guards retained; existing app credentials passed Sandbox OAuth and are configured; buyer approval pending. |
| 6 | Backend deployment | Not started at this checkpoint; all provider configuration is now available for deployment. Final local production container built successfully. |
| 7 | Backend URL | Reserved https://movieshop-api.vercel.app ; not a verified live deployment. |
| 8 | Frontend build | npm ci and production compilation passed for code validation and then passed again with the actual public Sandbox client ID. Only the restricted shared-demo login values are intentionally public. |
| 9 | Frontend deployment | Hobby project movieshop prepared, no deployment created. |
| 10 | Frontend URL | Reserved https://movieshop-two.vercel.app ; not a verified live deployment. |
| 11 | CORS | Exact assigned frontend origin configured, allowed/disallowed preflights passed in the Neon-backed local API. |
| 12 | Demo account | Existing confirmed non-staff portfolio-demo retained; email/password mutation and catalog deletion rejected. Discoverable login helper prepared with intentionally public shared-demo build values. |
| 13 | Auth smoke | Real-PostgreSQL automated flows passed; Neon-backed local Production login/refresh/logout/admin restrictions passed. Public HTTPS auth not tested. |
| 14 | Email flows | Existing mocked tests pass; real confirmation/reset/2FA delivery unverified until real email QA. |
| 15 | PayPal flow | Existing mocked Sandbox flow passes; real creation/approval/capture unverified until representative buyer approval/capture. |
| 16 | Database persistence | Neon schema/catalog/user/cart queries and writes passed in local Production checks; no startup reseeding performed. |
| 17 | Checkout persistence | Database-backed checkout previously verified locally and on Neon; owner/captured/expiry behavior covered by tests. Real payment/redeploy checks pending. |
| 18 | Restart | Prior independent application/PostgreSQL restart checkpoint passed; post-public-deployment restart not run because no deployment exists. |
| 19 | Security | 28 tests and 24 Neon-backed local HTTP checks passed. Production body logging remains off; errors sanitized; rate-limit 429 and untrusted CORS rejection checked; no private credential values committed; privileged build identity rejected. |
| 20 | Desktop rendered QA | Unverified; no browser automation performed. |
| 21 | Mobile rendered QA | Unverified; no browser automation performed. |
| 22 | Files changed | See inventory below. |
| 23 | Commit SHA | See current deployment branch Git log; commit message Finalize MovieShop public deployment. This title does not claim production verification. |
| 24 | Git status | Changes isolated to deployment/vercel-demo; no merge or Portfolio/HoopsLab changes. |
| 25 | Remaining blockers | Actual backend/frontend deployment, real HTTPS/email verification and representative Sandbox buyer approval/capture, followed by restart checks. Existing provider accounts/credentials are valid; do not request duplicate setup. Do not claim public deployment verified. |

## Changed-file inventory

- `MovieShop.Backend/Market.Application/Modules/Profile/Commands/ChangeEmail/ChangeMyEmailCommandHandler.cs`
- `MovieShop.Backend/Market.Application/Modules/Profile/Commands/ChangePassword/ChangeMyPasswordCommandHandler.cs`
- `MovieShop.Backend/Market.Tests/PublicDemoSafetyTests.cs`
- `MovieShop.Frontend/scripts/configure-production.mjs`
- `MovieShop.Frontend/src/app/modules/auth/login/login.component.html`
- `MovieShop.Frontend/src/app/modules/auth/login/login.component.ts`
- `deployment/README.md`
- `deployment/preparation-status.md`
- `deployment/public-release-status.md`

Exact manual setup steps: [runbook](README.md#required-manual-provider-steps). Never paste service secrets into chat.
