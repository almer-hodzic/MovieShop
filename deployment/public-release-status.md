# MovieShop public release checkpoint

**PUBLICLY DEPLOYED; FINAL EMAIL AND SANDBOX VERIFICATION PENDING.** No paid service enabled.

| # | Item | Result |
| --- | --- | --- |
| 1 | Branch / deployed commit | deployment/vercel-demo; application commit 8c66668b0d8f600be7bd4fae3babd0aa0fdc0949. Documentation-only follow-up: see Git log. |
| 2 | Backend configuration | Required database/JWT/email/Sandbox values configured encrypted in Production; issuer/audience/callback match actual URLs; access 20 minutes, refresh 14 days. |
| 3 | Database startup | ApplyMigrationsOnStartup=false; SeedOnStartup=false; PublicDemo:Seed=false. Deliberate migrations/seed only; existing Neon data retained. |
| 4 | Email setup | Existing free SendGrid account and verified sender validated. Real confirmation delivered; owner found it in Spam. No replacement account/key required. |
| 5 | PayPal setup | Sandbox OAuth and public SDK returned 200; USD, Sandbox endpoint and Live-rejection guards retained. |
| 6 | Backend deployment | READY; dpl_7v1gtuajtCvZRQzJ3AwaXT4unoVZ after the single requested restart. |
| 7 | Backend URL | https://movieshop-api.vercel.app |
| 8 | Frontend build | Clean npm ci and production build passed, including actual public Sandbox client ID; no private API/JWT/database/email key in frontend configuration. |
| 9 | Frontend deployment | READY; Hobby project movieshop; dpl_Cu4vwtCvybtso71sVGxsu4upRyz8. |
| 10 | Frontend URL | https://movieshop-two.vercel.app |
| 11 | CORS | Exact frontend origin allowed; other origin received no allow-origin header in live preflight checks. |
| 12 | Demo account | Discoverable Use demo account helper; confirmed non-staff identity; credential changes, catalog deletion and admin access denied. |
| 13 | Auth smoke | Live login/refresh/logout/revoked refresh and authorization passed. Separate live probe confirmed foreign cart reads hidden, edits/deletes denied and profile remains own; temporary fixture cleaned up. |
| 14 | Email flows | Confirmation delivered; reset request returned 200. Actual confirmation/reset/emailed 2FA completion remains pending owner actions; no auth bypass performed. |
| 15 | PayPal flow | Real 12.00 USD Sandbox order CREATED. Buyer approval/capture remains pending; creation alone is not payment verification. |
| 16 | Database persistence | Neon catalog/user/cart persisted; TLS VerifyFull direct/pooled connections verified; zero admin users. |
| 17 | Checkout persistence | Real Sandbox checkout session persisted across API redeployment with status/owner unchanged. Capture/expiry behavior also covered by automated tests. |
| 18 | Restart | Single requested redeployment passed: READY, health 200, existing JWT/profile/cart/session preserved, frontend 200. No repeat restart needed. |
| 19 | Security | 28 tests, 24 local Neon HTTP checks, 23 live HTTP checks and separate live isolation probe passed. Errors sanitized; recent logs scanned for configured secrets/bearer tokens with no matches; request-body logging off. Local 429 tested; production saturation deferred during owner auth QA. |
| 20 | Desktop rendered QA | Unverified; no browser automation surface available. Seven frontend routes and eight JS/CSS assets returned 200; this does not establish rendered layout. |
| 21 | Mobile rendered QA | Unverified. |
| 22 | Files changed | Inventory below; follow-up after deployment changes documentation only. |
| 23 | Commit SHA | Deployed application: 8c66668b0d8f600be7bd4fae3babd0aa0fdc0949; latest documentation commit in deployment branch Git log. |
| 24 | Git status | Deployment branch only; main not merged; Portfolio/HoopsLab unchanged. Check git status for current cleanliness. |
| 25 | Remaining blockers | Owner must complete confirmation, password reset and real emailed 2FA; then representative Sandbox buyer approval/capture. Rendered QA unverified. Do not claim full public deployment verified yet. |

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

Manual verification: [runbook](README.md#required-manual-verification). Never paste secrets or email codes/links into chat.
