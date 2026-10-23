# NineStar mobile app: Codex handover

Prepared 23 October 2026. This is a starting snapshot; inspect the current checkout before making changes.

## Goal and architecture

Build an iPhone and Android app using React Native, Expo and TypeScript, consuming the existing NineStar JSON API. The server remains the single owner of calculations, descriptions, membership rules and database data. Do not reproduce calculation or entitlement logic in the app.

The existing MVC website and mobile API are parallel clients of shared services. Do not reroute existing website pages through the mobile API. Do not extend the plugin ApiController for mobile features. An ASP.NET Core migration is outside this task.

## Owner and environment

Simon (Si) is an experienced C# developer, working alone with Codex.
- Windows 10 Pro version 1909, 64-bit; Intel i5-3320M, 16 GB RAM.
- Node v22.22.0; npm 10.9.4.
- VS Code installed on C:, Codex extension being installed/signed in.
- Existing website remains in Visual Studio 2017.
- Proposed mobile project folder: D:\Workspace\ninestar-mobile.
- Existing website checkout: D:\Workspace\ninestar.
- C: had about 11.8 GB free; D: about 98 GB free at setup.
- Physical test phone: iPhone 14 Pro, reported iOS 26.5. No Android phone.
- An old MacBook Pro is available; prefer daily development on Windows.
- The mobile project and its GitHub repository have not yet been created in this conversation.

Verify installed tools and current Expo/Node compatibility before scaffolding. Avoid a heavy emulator on this PC. Use EAS cloud builds for native iOS/Android builds. Check Apple signing/account requirements before promising an iPhone installation route. Windows 1909 may constrain current tooling; diagnose actual errors rather than assuming compatibility.

## Existing backend

Repository: https://github.com/glantzsimon/ninestar
Branch: feature/mobile-json-api
Draft PR: https://github.com/glantzsimon/ninestar/pull/12
Last implementation/master-merge snapshot before this document: e70251fc4f55e66fb88b04e3cbba59bf8c287147.

Stack: .NET Framework 4.7.2, MVC 5, C# 7.3, EF6, SQL Server, Autofac, Newtonsoft.Json, Stripe, K9 libraries and astronomy services. Preserve VS2017 compatibility and aligned OpenIddict 6.4.0 packages.

Read these files in the actual website checkout before backend work:
1. AGENTS.md
2. docs/mobile-api.md
3. docs/mobile-api-routes.md
4. docs/mobile-api-validation.md
5. docs/services-and-controllers.md

There are 124 declared mobile routes in the source audit. This is not evidence that all routes passed runtime validation. Existing view, plugin and test-only/debug workflows are deliberately separate; read the documented exclusions.

## Validation status

The owner confirmed that updated DEV builds, launches and the ordinary website UI works after the master merge. The cloud implementation environment performed source checks but could not build the legacy .NET Framework solution.

Full mobile HTTP integration, access/ownership matrices, SQL concurrency, Stripe test-mode fulfilment and image processing remain to be validated. Do not describe the API as production-validated.

The earlier DEV startup failure was a branch/model mismatch: DEV had already applied 202610170149068_AddOpenIddictOAuth. Master was merged into this feature branch to restore model/migration/dependency compatibility. No new corrective migration or database rollback was required. A database does not switch when Git branches switch.

Do not enable automatic migrations, alter migration history or generate migrations from an older model to hide this problem. All existing-database schema ownership remains in webapp/DataAccess.

## API and authentication contract

Prefix: /api/mobile/v1, appended to the actual site base URL, including its virtual directory where applicable.

Current mobile authentication is the existing Forms Authentication cookie/session contract:
- Retain cookies across requests.
- Obtain account/csrf and send X-CSRF-Token for every POST, including public POSTs.
- Refresh CSRF after login/logout.
- Read metadata/options, metadata/timezones and account/me.
- Inspect actual DTOs in webapp/WebApplication/ViewModels/MobileApi and route documentation before implementing requests.
- Treat HTTP/protocol errors separately from user-facing localised messages.
- Check actual response envelopes, entitlement fields and rich-text content; do not invent schemas.

Native OAuth/PKCE is not a completed app integration. If selected, integrate with the existing OAuth owner rather than creating another identity database or token store.

Complementary calculation POSTs can consume credits: do not automatically replay them. Use saved retrieval routes when reopening a result. Preserve ownership, membership expiry and revocation behaviour.

No server API keys, database credentials, Stripe secrets or other private credentials belong in the app bundle. Client environment variables are not a secret store.

An iPhone's localhost refers to the phone. Establish a reachable HTTPS test API address and trusted certificate before phone integration. Do not disable certificate validation. Integration deployment is a separate owner-authorised action.

## Development and delivery workflow

Use the HTTPS Integration site as the shared development API target once the feature branch is deployed and smoke-tested there. Confirm its actual URL and database/migration state before deployment; do not assume the development API is already hosted. Check website regression, API routing, cookies/CSRF and basic chart responses after deployment.

TeamCity remains responsible for the website/API's existing Integration workflow. It is not needed to scaffold or run the mobile app locally. EAS can build native binaries in the cloud; automated mobile CI can be added later.

GitHub remains the source-control home. A separate mobile repository is the proposed default, but has not yet been created or named beyond ninestar-mobile. Keep server source in its existing repository.

The owner's Git Bash function prepends feature/:
- loadbranch mobile-json-api selects feature/mobile-json-api.
- Never suggest loadbranch feature/mobile-json-api.
- Verify with git branch --show-current.

Ordinary server commits must not contain #teamcity. That marker triggers Integration deployment. Do not merge into master, deploy, publish an app or run live payments without an instruction authorising that action.

Read applicable .gitattributes before editing backend files. Preserve git-crypt-protected sources/configuration and never replace ciphertext with plaintext.

## First implementation sequence

Proceed step by step, reporting concrete progress and blockers:
1. Inspect local folders, Git state and installed tools. Read the existing repository guidance and API contracts.
2. Scaffold the separate Expo/TypeScript mobile project on D:. Create concise mobile AGENTS.md guidance and a README recording setup and verified versions.
3. Establish configuration for the API base URL, request/response types, cookie/session handling and CSRF. Validate the authentication transport on the actual iPhone early, before broad UI development.
4. Complete one vertical slice: sign in, account state, own personal chart, selected date and predictions. Compare results with the website using identical calculation settings and timezones.
5. Add loading, accessible error states, logout/session expiry and entitlement handling. Keep labels/messages localisable. Use native controls; MVC HTML-helper rules apply to the website, not React Native.
6. Add planner/calendar, compatibility and further member features incrementally after the first slice passes.
7. Consider payments only after server checkout tests and current App Store/Google Play billing requirements have been reviewed. Existing website Stripe routes alone do not establish a compliant native purchase flow.
8. Validate Android independently before claiming Android support; an iPhone test does not cover it.

Use existing energy artwork and branding where appropriate. Safely render server-authored rich text. Preserve personal/global prediction distinctions, local dates, birth times/timezones and explicit calculation options.

Start with the first slice rather than building every screen at once. Do not change the website architecture to make scaffolding easier.

## Suggested initial instruction to local Codex

Read this handover and the referenced backend documentation in D:\Workspace\ninestar. Inspect the actual environment, then help me create the Expo/TypeScript app in D:\Workspace\ninestar-mobile. Begin with API connectivity and a working sign-in, own-chart and predictions flow. Keep existing web pages unchanged and report progress as you complete each step.
