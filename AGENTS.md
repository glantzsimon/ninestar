# NineStar project conventions

Preserve existing structure, naming, formatting, services, repository patterns and dependency injection. Keep changes focused; do not refactor unrelated code. Use C# 7.3 and .NET Framework 4.7.2-compatible code.

## UI authoring

The owner requires UI controls to use existing K9 HTML helpers, rather than hand-written control markup or client-generated control HTML. Inspect comparable views and actual helper signatures before implementing UI.

- Use `Html.BootstrapEditorFor` for editable model properties and `Html.BootstrapDisplayFor` for display properties, with existing editor/display templates and `EditorOptions`.
- Use `Html.BeginBootstrapForm`, `Html.BootstrapButton` and `Html.BootstrapActionLinkButton` for forms and buttons. Reuse existing alert, panel, collapsible-panel and validation helpers.
- Model field labels through existing resource-backed metadata or `EditorOptions`; avoid separately hand-writing labels and inputs.
- For dynamic UI, clone helper-rendered controls instead of creating raw button/input HTML in JavaScript.
- Follow calculator, account and personal-cycle views for spacing, styling and help-icon patterns. Structural layout containers should match existing views.
- Expanded inline help uses the existing `.well` container for rounded corners, even padding and the theme border (white in dark mode). Place it inside the collapse wrapper so the collapsed state and height animation remain correct; reuse shared styles rather than duplicating them.

## Performance diagnosis

Read [docs/cache-safety.md](docs/cache-safety.md) for calculation ownership and access-cache safeguards. Profile/planner service results are request-owned copies; do not assume generic cache/repository results are isolated. Cache calculated data beneath identity/access checks rather than caching responses that bypass them.

Read [docs/performance-review.md](docs/performance-review.md) for the service/controller audit, applied query reductions and prioritised follow-ups. Selector metadata must not expand recipients. Batch distinct related IDs within one operation; keep eligibility/authorisation checks fresh and do not parallelise repositories sharing an EF context.

Browser Waiting for server timing measures time before the response, including possible server queuing; it does not identify a particular slow method. Measure controller access checks, preference loading, planner batches and description mapping separately before attributing a delay to astronomy, session locks or caching. DEBUG builds emit correlated `GetPlannerData` stage timings (cache, profile, boundaries, astronomy batches and row construction) and `Personal calendar Month` overall timing through `Debug.WriteLine` to Visual Studio's Debug Output. Keep diagnostic timers and traces inside `#if DEBUG`; avoid duplicate outer-service traces and private inputs. Do not present an unmeasured optimisation as a confirmed root-cause fix.

## Panel loading

Use the existing `$.fn.displaySpinner($container)` and `$.fn.hideSpinner($container)` helpers in `Views/Shared/Scripts/Default.cshtml`, as demonstrated in `Views/Predictions/_CyclesJs.cshtml`. They render the existing `partialSpinner` / optional `partialOverlay` styled by `Content/less/controls/pageSpinner.less`. Do not substitute a plain Loading label or introduce a separate spinner design. Keep the container height stable while loading, stop a pending spinner fade before restarting it, and clear loading on success, failure, timeout and rendering exceptions. Panel requests should own their loading/error handlers without triggering unrelated global AJAX handlers.

## Combined predictions

Read [docs/combined-predictions.md](docs/combined-predictions.md) before changing prediction descriptions. Personal `Predictions/Combined` and collective `GlobalPredictions/Combined` are separate interpretation families: never alias global readings to personal-house text. Preserve generic resources/endpoints and knowledge-base/Additional Cycles consumers; opt the main month/day pills and legacy planner into new combined endpoints and rendering only. Reuse `CombinedPredictionHelper` for lookup and period context. Two-number `_Y_M` keys belong to numerology; combined monthly keys require their own prefix. Decorate request-owned UI planner copies, keep context immutable and resolve HTML at display time; never cache culture-specific combined text inside the numerical planner. Enclosing year/month must come from the selected astronomy periods, not a reused profile snapshot. Keep morning/afternoon distinctions and lunar phases separate. Combined monthly and daily readings should retain the richness of the original source descriptions in two substantial paragraphs, blending the nested influences rather than reducing them to a one-sentence summary or simply listing generic meanings. Keep useful emotional, relational and practical detail; use the matching personal/global source family.

## AJAX calculation calls

Forward the rendered model's `CalculatorType` explicitly in planner and alchemy AJAX payloads; an optional server default can silently change Advanced results to Simple. Shared partials must specify the owning controller in `Url.Action` rather than depend on the page's current controller. Profile and predictions scripts coexist on My Personal Chart: keep their initializer names distinct and scope alchemy panel selectors to `#personal-profile` and `.cycles-tabs-container` respectively.

## Date-range calculations

For calendars, exports and other multi-date features, reuse `NineStarKiService.GetPlannerData` and its existing period-energy batches. Do not loop over dates calling `CalculateNineStarKiProfile`: a complete profile per day is expensive and duplicates planner calculation logic. Choose the largest appropriate `EPlannerView`: EightyOneYear returns nine-year rows, NineYear returns year rows, Year returns month rows, Month returns daily/afternoon rows, and Day returns hourly rows. For daily exports, fetch Year once per covered solar year, use its year energy and month rows, and expand only overlapping months through Month view with the existing `nineStarKiModel` argument. This avoids recalculating the profile per month as well as per day. Calendar/export callers that do not use lunar presentation should pass `includeMoonPhases: false`; the planner forwards this to profile creation as well as parent/daily moon phases. The default preserves existing profile/planner UI, and both cache keys include the option. Include this option in the planner cache key so lightweight and full results stay distinct. Use the parent row's energy for its period; a reused profile's selected-date year/month snapshot is not the child period's house. Trim results to the requested Gregorian range. Preserve saved calculator/house/hemisphere preferences, check cancellation between batches and rows, and refresh the profile when its date-dependent cycle-switch state changes.

Pass local selected dates as `DateTimeKind.Unspecified` together with the saved user timezone. Supply the date-only birth date, birth timezone and birth time separately: `GetPlannerData` adds the birth time itself, so pre-combining it would apply it twice. Keep period boundaries in the existing planner/astronomy code rather than introducing another calculation path. Exact-instant solar-year selection at midnight on a transition date can return a year whose all-day rows have already ended. If the returned Year period ends before the calendar cursor, select safely inside the following period using its returned end plus three days (the planner's forward-navigation convention); retain the original cursor and contiguous-coverage checks. Never hardcode Lichun's date or skip an uncovered day.

## Helpers and services

Place static utility and presentation helpers in `webapp/WebApplication/Helpers` with the `K9.WebApplication.Helpers` namespace, following existing helper naming conventions (for example, `CombinedPredictionHelper`). Static classes without an injected service lifecycle should not be placed in `Services` merely because controllers or services call them. Keep injected application services and their interfaces in `Services`. Register moved/new files in the project and update namespaces, callers and documentation together.

## File organisation

Keep Nine Star Ki domain models in `Models/NineStarKi`, retaining the existing `K9.WebApplication.Models` namespace used by that folder. Copying infrastructure such as `CalculationCopyContext` belongs in Helpers. Application-specific JavaScript belongs in `Scripts/k9`, using existing camelCase filenames. The main `~/Scripts/js` bundle already includes `Scripts/k9/*.js`; remove standalone script tags when moving a script into that wildcard to prevent duplicate initialisation. Keep controller tests in `Unit/Controllers` and service tests in `Unit/Services`; test support helpers remain separate.

## Service and controller reuse

Read [docs/services-and-controllers.md](docs/services-and-controllers.md) before implementing service/controller work. It maps the existing application services, controller families and tested astronomy batch call sites. Inspect the relevant interface, implementation and current callers before adding a method; extend the existing owner of the behaviour instead of creating parallel business logic.

- Inject existing service interfaces through the established packages/constructor pattern. `BaseService` exposes `INineStarKiBasePackage` and inherits `CachableBase`. Controllers use `INineStarKiPackage` or `IControllerPackage<T>`; generic `BaseNineStarKiController<T>` already supplies the CRUD event/markup pattern.
- Reuse `NineStarKiService` for profiles, compatibility, summary energies and planner data; `NumerologyService.Calculate*PlannerCodes` for numerology batches; `BiorhythmsService.Calculate` for biorhythms; `ReportsService.GetYearlyReport` for report assembly; and `AstrologyService.GetMoonPhase` for resource-backed lunar descriptions. Keep calendar/ICS concerns in `GoogleCalendarService`.
- Resolve identity with the existing `Current` helpers, preserving impersonation. Read explicit-member/background/feed preferences via `IUserService.GetUserPreference<T>`; it queries the database, so resolve values once outside date/item loops. `UpdateUserPreference` also writes session state and reloads preferences. Do not call it from read-only calendar GETs. Session-safe preference getters do not make every `SessionHelper` method safe without HTTP context.
- Reuse membership access predicates and existing controller checks. `GetActiveUserMembership` can create a free membership; `GetOrCreateUserInfo` and `ContactService.GetOrCreateContact` can write records. Do not treat them as pure/cacheable lookups. Use existing controller-specific authorisation, anti-forgery, API-key/account validation and complementary-reading accounting.
- Use `EmailTemplateService` for templates and unsubscribe links, `MailerService.SendEmailTemplateToUsers` for bulk sends, `EmailQueueService` for queue rules, and `PromotionService` for promotions/reminders. Reuse consultation booking, contact matching, Stripe, PDF and media services rather than rebuilding their workflows.
- Inspect the actual cost of a reused method. Avoid database queries, full profiles, AI/HTTP calls or process launches inside per-row loops where a batch exists. `MailingListService.ListAll` expands users/system lists; `GetAllSlotsAndBookings` loads a weekly collection and hydrates bookings. Use lighter existing methods/options when their semantics fit, and do not label all existing code inherently efficient.
- `CachableBase` uses process-wide `MemoryCache.Default` and returns shared object references. Keep all result-changing inputs in keys, including dates, timezones, calculation/calculator/house/hemisphere choices and supplied-profile state. It coalesces concurrent misses per key across service instances, removes completed/failed in-flight work, and does not clone models. Keep nested cache dependencies acyclic. Do not mutate a shared profile to change its selected date; preserve culture/user-dependent output-cache variation and avoid caching anonymous private-feed results by URL/token.

## Globalisation

Use Title Case for English panel headings, matching existing headings such as `Houses You Currently Occupy` and `Your Personal Calendar`.

All user-visible labels, help, tooltips and messages belong in Globalisation. Check existing resource keys and text first; reuse entries without duplicates.

Use strongly typed `Dictionary.ResourceName` properties for fixed UI text, including weekday headings. Keep generated accessors in sync. Do not use string-key helpers or `ResourceManager.GetString("FixedLabel", ...)` for fixed text. Calculated resource keys, such as combined year/month/day predictions, may use dynamic lookups.

## Branch and deployment workflow

The owner's Git Bash `loadbranch` function automatically prepends `feature/` to its argument, fetches origin, then checks out the branch (creating it from the remote if needed) and fast-forwards an existing local branch. Pass only the suffix: `loadbranch mobile-json-api` selects `feature/mobile-json-api`. Never suggest `loadbranch feature/mobile-json-api`, which doubles the prefix. Use `loadmaster` to return to master. Verify the selected branch with `git branch --show-current`.

Work on the existing feature branch and prepare a draft pull request for review. Do not merge or deploy without the user's instruction. Normal commits must not contain `#teamcity`; that marker deliberately triggers Integration deployment and should only be added when deployment is requested.

## git-crypt

The owner uses git-crypt locally: protected files are stored encrypted in Git and decrypted in the local working project. Before editing any file, check the applicable .gitattributes rules (or `git check-attr filter diff -- <path>`) for git-crypt protection. Leave protected files and their encryption attributes untouched. Do not replace ciphertext with plaintext, bypass the filters, or change keys. If a requested change needs a protected file, explain the need to the owner before proceeding. Work on ordinary, unprotected files normally.

Do not assume an unexpected modified-file status is caused by git-crypt. Inspect the diff and applicable attributes before recommending that local changes be discarded.

## LESS and CSS

Treat `webapp/WebApplication/Content/less/` as the styling source. Preserve the existing organisation: `sections/` for page/feature sections (such as predictions and navbar), `controls/` for reusable controls (such as panels and the personal calendar), `main/` for general and responsive styles, and `config/` for shared colours and paths. Edit the relevant existing LESS file rather than adding unrelated overrides.

Keep the corresponding CSS under `Content/` in sync. Check `webapp/WebApplication/compilerconfig.json` for the LESS-to-CSS mapping; register new stylesheets there and in the project as required. Compile with the project's tooling when available. If compilation is unavailable, update both files consistently and clearly state the verification limit.

Reuse existing theme variables instead of hardcoded colours. Panel backgrounds use `background: var(--well-color)` (a gradient in light mode and teal in dark mode). Calendar full-screen mode uses that same variable; embedded mode is transparent. Use the existing energy artwork under `ninestar/energies/` through `MediaService.BaseImagesPath`, matching the cycle views.

For hover and selected states, inspect comparable existing controls before styling. The predictions calendar in `Content/less/sections/predictions.less` provides the calendar visual hierarchy: 2px hover lift and shadow, blue selected background and stronger shadow. Preserve theme colours and account for the general important button shadow in `main/elements.less` when adapting these effects.

Inspect actual stacking contexts and existing z-index values before adding overlays. The calendar is moved to the document body while expanded and restored on close; its z-index must cover the floating navbar/account header. Preserve keyboard focus and Escape handling.

## Maintaining project continuity

As work proceeds, update this file for verified, lasting project conventions and owner preferences. Update relevant `docs/` files for feature behaviour, calculation contracts, decisions and validation limits. Keep entries concise and current; replace superseded guidance rather than accumulating contradictions. Do not record secrets, private subscription links, encryption keys or transient build status.

At the start of a new thread, fetch the current branch and read `AGENTS.md` and the relevant feature documentation before editing. Repository files are the authoritative technical record; project memory is supplementary. Do not rely on old chat summaries or scratch copies when current repository contents are available. For calendar work, read `docs/personal-calendar.md`; its calculation reuses the existing planner batches, preserving local calendar date, birth time and their respective timezones.
