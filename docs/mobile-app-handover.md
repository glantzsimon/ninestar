# NineStar mobile app: Codex handover

Prepared 23 October 2026. This is a starting snapshot; inspect the current checkout before making changes.

## Artwork follow-up: 27 October 2026

The owner approved the antique gold artwork standard in `docs/visual-design.md`. The implementation is on `feature/ui-artwork-refresh`, draft PR #16, pending merge/deployment. It refreshes all UI artwork and every PNG under `Images/ninestar`, preserving filenames, dimensions and formats except the approved UI naming/PNG exceptions. Read `docs/ui-artwork-report.md` and `docs/ninestar-artwork-report.md` for exact asset/ID mappings and validation limits. Technical trigram bars and relationship arrows are rendered from precise vector geometry; the dark cycle-of-five sibling-image error is corrected.

The mobile artwork catalogue must use its request origin/application path plus Images. It now does so instead of selecting the website's shared media/CDN host. Integration URLs must remain on Integration. Deploy image files to the application Images folder, then refresh the environment-specific catalogue/ETag. Do not rewrite hosts or hardcode image filenames in the app. The website's separate CDN configuration and embedded HTML media selection are unchanged. Windows build/xUnit and live release checks remain outstanding here.

## Current handover: 26 October 2026

The backend feature is merged into master. Continue from current master, not the old feature/mobile-json-api branch or an old TeamCity feature-branch build. The latest implementation commit at this handover is 82fd5748. Native app code remains in the separate ninestar-mobile project; its current UI state must be checked there.

Read docs/mobile-api.md for exact fields/resource mappings, docs/personal-calendar.md and docs/combined-predictions.md for calculation/content contracts, docs/website-calculators.md for trials/input memory, docs/website-theme.md for colours, and docs/quick-deployment.md for release tooling.

### Final API contracts

- Personal chart full/free responses expose roleLabels for all six energies. Character section titles, short role labels and explanatory help titles are distinct. Adult / Child Relationship details keep the plain heading; its helper uses About Adult / Child Relationship.
- Full/free Predictions include labels.periods, cycleTitles, predictionTitles, scopes and groups (primary, additional, global, extendedGlobal). Each available cycle exposes houseName. Free globalYear selects global prose. Labels do not grant access to further readings.
- Predictions help.overview uses PredictionsIntroduction; help.globalKi uses GlobalKiIntroduction, explaining collective/planetary Ki versus individual personal houses without interface wording such as toggle. The membership-page GlobalKiDescription remains unchanged. Keep moreOnCycles and the Epoch-specific globalKiWarning separate.
- Calendar month responses add Dictionary control/status labels, Monday-first weekdays, formatted dates/house labels and registered energy artwork IDs. Use labels.googleCalendarLink for the link-panel heading, help.subscription.htmlBody for instructions and labels.copyCalendarLink for the copy button. About Your Personal Calendar is explanatory help, not that panel heading.
- metadata/artwork includes assets["numerology.karmic-blueprint"].variants.default.url (222 by 222). Resolve IDs through the catalogue rather than constructing paths.

### Final website/content decisions

- One successful anonymous result per calculator section. Public pages and knowledge base remain available. Same-browser input memory shares last-submitted birth details and retains page-specific options; memberships remain unchanged.
- The introductory trial notice is removed. The approved post-trial sign-in explanation sits below Calculate/Generate controls in all six forms. Inputs must remain visually flush at the top. Check with the owner before adding unrequested explanatory/promotional copy. Display text belongs in Dictionary.
- All 729 personal full daily readings and 729 separate concise subscription readings lead with the day, with shorter year/month context. Full readings have two paragraphs; subscriptions retain IDEAL FOR / BE MINDFUL OF, three bullets each, and one paragraph. Day-specific list items come first where applicable. They share calculations/combinations, not one text resource. Full resources also serve personal combined daily predictions and mobile calendar descriptions. Monthly/global reading families were unchanged by this revision.
- Dark neutral borders use #b8b8b8 centrally. Light values, widths, decorative white image framing and blue focus/selection accents are preserved. LESS and all 41 affected CSS copies are synchronised.

### Verification and release boundaries

All 1,458 revised calendar resources passed checks for coverage, preserved daily guidance/headings and subscription formatting; changed C# passed syntax checks. Theme work compiled all 69 configured LESS inputs and checked affected CSS for unchanged light values, widths and blue accents. Windows build/xUnit, browser visual checks and authenticated Integration verification remain pending for these final revisions. Do not describe them as executed here.

Earlier anonymous Integration probes returned JSON: metadata/options 200 and own Predictions routes 401. This does not prove signed-in calculations/timeouts are resolved. The website error filter now leaves mobile paths to existing JSON handlers; capture server exceptions if failures persist.

Pushes do not establish deployment. The owner builds/releases on Windows. Quick-deploy full transforms config and uploads a fixed already-built binary list; it does not compile or publish every dependency. Servers require deployment protocol v2 before the new uploader can use them. Verify master/build revision in TeamCity. Integration noindex belongs only in its encrypted transform, never Production. No final Integration rollout or Production deployment has been verified here. Do not run deployments, migrations or restarts merely to test changes.

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
