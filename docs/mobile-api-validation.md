# Mobile API validation and app handoff

## Status

The JSON transport source and controller-family coverage audit are complete, with the exclusions in [mobile-api.md](mobile-api.md). The branch is `feature/mobile-json-api`, reviewed through draft PR 12. Website routes/views and the plugin API remain separate. No deployment, merge, live payment, message send or administrative operation was executed to validate this implementation.

Source checks cover C# syntax, project XML/compile registrations, route inventory uniqueness, unchanged view/route/plugin files and whitespace. This environment does not provide .NET Framework/MSBuild or decrypted astronomy sources. **Compilation, xUnit execution and HTTP/SQL/Stripe/image integration remain unverified.**

## Windows build and tests

For the website-membership entitlement phase, first build/run the updated Windows solution and existing tests, including `MobileEntitlementsControllerTests`, `MobileEntitlementsMapperTests` and the new read-only lookup tests in `MembershipModelserviceTests`. Full solution/xUnit execution has not been performed in the cloud environment. The previously passing standalone verifier probe is separate evidence and does not validate these endpoint changes.

After an explicitly requested Integration deployment, use the app's authenticated cookie session to compare `GET /api/mobile/v1/membership/entitlements` with `GET /api/mobile/v1/account/me` -> `data.entitlements` (asOfUtc naturally differs between requests). Exercise existing free, paid and lifetime accounts; confirm paid expiry/deactivation causes fresh flags/chart access to match current server rules, and that switching account/logout cannot retain paid UI state. Check anonymous 401, missing-account 404, JSON/no-store behaviour and empty sources without grant creation. Existing profile fields and complementary counters must be preserved. No new schema migration is required. Keep unsaved My Account edits separate from refreshed app entitlements.

In the owner's normal decrypted checkout, select `loadbranch mobile-json-api` in Git Bash, then verify `git rev-parse --abbrev-ref HEAD` (the owner's older Git does not support `branch --show-current`). Use the Visual Studio developer environment with the .NET Framework 4.7.2 targeting pack and normal package access.

From the repository root:

```powershell
nuget restore webapp\NineStar.sln
msbuild webapp\NineStar.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU" /p:LangVersion=7.3
$runner = Get-ChildItem webapp\packages\xunit.runner.console.2.3.1 -Recurse -Filter xunit.console.exe | Select-Object -First 1
& $runner.FullName webapp\WebApplication.Tests\bin\Debug\K9.WebApplication.Tests.dll
```

Resolve build failures against the actual restored K9 2.0.0.212 assemblies before proceeding. Source inspection of a shared-library repository is not a substitute for compiling against those binaries. New tests cover mobile profile access/options/projections, request date/time validation, I Ching isolation, reserved preferences, session isolation/revocation, image boundaries, protected actions, error scoping, donor deduplication and moderation. Run the existing tests as well because shared owners changed.

## Existing DEV database compatibility

DEV was confirmed to have `202610170149068_AddOpenIddictOAuth` applied under `K9.DataAccessLayer.Database.DatabaseInitialiserLocal`. An earlier version of this feature branch lacked that migration and its OpenIddict model mappings, causing `AutomaticMigrationsDisabledException` at startup. Current master has now been merged into the feature branch, retaining the existing migration, unconditional LocalDb OpenIddict mappings and package/configuration updates. No new migration, rollback or migration-history edit is part of this fix.

After pulling the updated branch, stop the local site, restore NuGet packages and clean/rebuild the solution before rerunning DEV. The owner uses git-crypt, so protected configuration/package files must be read in the normal decrypted checkout. Do not enable automatic migrations to make an older branch fit an already migrated database. If startup still reports a mismatch, inspect the actual connection target, loaded assemblies and migration history before changing schema.

## IIS and data gates

Use a test instance and disposable test data. Record the actual result for each gate; this document is not evidence they passed.

| Area | Required check |
| --- | --- |
| Routing | Exercise declared routes, missing routes, wrong HTTP verbs, invalid JSON/enums and oversized bodies. Managed failures must be JSON with no login redirect or exception details. Check IIS rejection/error-page behaviour separately. |
| Cookie/CSRF | Fetch `account/csrf`, retain its cookie, send `X-CSRF-Token` on every POST, log in, and refresh the token. Missing/mismatched tokens must fail, including on public POSTs. Use HTTPS. |
| Access | Test anonymous, free, paid, expired and administrator accounts. Check ownership for account updates, messages, bookings, images and saved readings. Remove access between requests and test again. |
| Sections | Compare own/free and complementary data with website grants. Full natal sections must not leak through complementary compatibility/predictions. Check minors and hidden chemistry. |
| Dates and calculations | Compare birth time/timezone, selected local date, calculation method, calculator type, house display, both hemisphere flags, morning/afternoon, moon phases and planner navigation with website results. |
| Saved readings | Save different inputs for two identities and impersonation targets. Confirm isolation, logout clearing, paid revocation and complementary retrieval without another debit. New complementary POSTs each consume a credit; do not automatically replay them to reopen a reading. |
| SQL transactions | Confirm repositories and transaction coordinators share the scoped DbContext. Test concurrent last-credit reads, slot booking conflicts, rollback, deadlock handling and multiple IIS workers. |
| Checkout | Use Stripe test mode. Check repeated start/completion with the same GUID, mismatched body/GUID, wrong owner/amount/currency, unsuccessful intents and failure during SQL commit. Verify one fulfilment and reconciliation of legacy nontransactional email effects. Retain stable MachineKey material. |
| Media | Decode real/corrupt JPEG/PNG, oversize/dimension failures, app virtual directories, write permissions, avatar ownership and deletion traversal. Verify output is a valid re-encoded PNG. |
| Administration | Check each schema/field allowlist, target IDs and empty bulk selections. Verify role changes take effect immediately; ordinary users cannot moderate, send, import, export or run maintenance. Test moderation cleanup of comment likes. |
| Regression | Open the existing website calculators, account, booking and purchase flows. Their URLs and view rendering must behave as before. Check the existing plugin API independently. |

## First app integration

1. Use the validated test-instance base URL plus `/api/mobile/v1`. Keep a cookie jar and treat protocol error codes as localisation keys. Refresh CSRF after login/logout.
2. Load `metadata/options`, `metadata/timezones` and `account/me`. Build one vertical slice: login, own chart, date selection and predictions. Use the typed calculation request settings rather than reimplementing calculations in the app.
3. Add planner, compatibility, numerology, bookings and account editing. Use paid routes or explicit complementary operations according to the account's returned entitlements. Render resource-authored rich text safely in the chosen native framework.
4. Add payments only after the Stripe/SQL gates pass. Keep a stable request GUID per checkout and fulfil only through server confirmation.
5. If native OAuth/PKCE or external-provider sign-in is selected, integrate with the existing authentication implementation brought in from master; do not create another token store. Cookie login already has a transport contract on this branch.
6. Review and merge after validation. Request Integration deployment separately through the established TeamCity workflow. Normal branch commits do not contain the deployment trigger marker.

The endpoint inventory is [mobile-api-routes.md](mobile-api-routes.md); request DTOs are in `webapp/WebApplication/ViewModels/MobileApi`. All website and app calculations continue to come from the existing shared services and repositories.
