# Mobile JSON API

The proposed shared website/app Apple subscription contract is in [Shared website/app membership and Apple subscriptions](#shared-websiteapp-membership-and-apple-subscriptions-proposed-contract). It is a draft for agreement, not an implemented billing feature.


## Public artwork catalogue

`GET /api/mobile/v1/metadata/artwork` supplies the shared, public artwork needed by native views. Existing website views/helpers and all existing image fields remain unchanged. This is an additive API feature with no database/migration or purchasing changes.

The response uses the normal success envelope:

```json
{
  "success": true,
  "data": {
    "version": "<64-character SHA-256>",
    "assets": {
      "ninestar.energies.water": {
        "group": "ninestar.energies",
        "variants": {
          "default": {
            "url": "https://<configured-media-host>/Images/ninestar/energies/water.png?v=<content-sha256>",
            "revision": "<content-sha256>",
            "contentType": "image/png",
            "width": 600,
            "height": 600
          }
        }
      }
    }
  },
  "error": null
}
```

Dimensions above are illustrative; use the actual returned dimensions. `assets` is a dictionary keyed by stable protocol ID, not localised display text. Each entry contains `group` and a dictionary of actual `variants` (`default`, optionally `dark`). Select `dark` when present for dark mode, otherwise `default`. No synthetic/resized variants are advertised. Separately authored logo sizes retain their own IDs. GIF IDs end in `-animated`. IDs are case-sensitive; original filename case is preserved in URLs. Use the exact supplied URL rather than constructing a path from an ID, label or number.

The initial explicit catalogue contains **431 file registrations, 424 logical IDs and 424 distinct image contents**. It covers Nine Star energies, elements, trigrams, modalities, seasons, organs and transformations; yin/yang; all 32 supplied moon-phase images; numerology codes and planner artwork; UI artwork; company branding; I Ching; membership artwork; backgrounds, home/about and shared artwork; and supplied country flags. Seven light/dark pairs share their logical IDs. Identical image contents share a canonical URL and revision even when used under different semantic IDs (for example, earth and stable modality). This avoids duplicate image downloads while preserving useful domain names. The catalogue contains metadata/URLs, never inline image bytes.

Public media is served from the existing `MediaService.BaseImagesPath`, captured once per response. There are no media health requests, directory scans, file reads, database queries, user lookups or calculations in the action. Metadata and content hashes are generated ahead of time and compiled into the application. An unavailable or invalid/non-absolute media base produces `503 artwork_unavailable` with `no-store`; it must not silently return an empty successful catalogue. Keep configured media bases absolute HTTP(S) URLs with no credentials, query or fragment. The existing scheduled health check remains the sole owner of hosted/local fallback selection.

### Artwork references in calculation responses

Additive properties, retaining existing fields:

| Response projection | Added artwork references |
| --- | --- |
| Every `MobileChartEnergy` (including profiles, cycles, planners and knowledge base) | `artworkId`, `elementArtworkId`, `trigramArtworkId`, `modalityArtworkId`, `seasonArtworkId`, `yinYangArtworkId` |
| Full personal chart | `strongYinOrganArtworkId`, `strongYangOrganArtworkId`, `weakYinOrganArtworkIds`, `weakYangOrganArtworkIds` (arrays retain corresponding organ order) |
| Moon projection | `artworkId`, alongside the unchanged `image` filename |
| Numerology code and planner-period projections | `artworkId`, based on the domain enum rather than a translated name |

An absent/unregistered symbol returns a null reference; clients use their existing placeholder or legacy field. Unknown future IDs must not break screen rendering. Responses that only expose numbers retain their existing shape; a client must not derive filenames from those numbers. Hexagram lines remain structured data rather than invented image files. This endpoint does not render HTML or rewrite images embedded in resource-authored HTML.

Article-specific images, user avatars/uploads and video URLs remain on their existing owning endpoints, with existing access rules. Article IDs and user IDs are never catalogue keys. Email-only artwork, website favicons/install icons and administrative/private uploads are outside the shared runtime catalogue. A source audit found 92 literal image references across views, LESS and Globalisation content: all content-image references are included; the remaining 15 are website install/favicon images. Dynamic energy/trigram/element/modality/season/organ, numerology and lunar filename families were checked separately against their existing model/enum mappings.

### Cache contract and native client behaviour

The global BeginRequest cache default explicitly excludes only this exact artwork GET route; it must not pre-set DEBUG NoCache or Release ServerAndPrivate, because the action cannot relax an already restrictive ASP.NET policy. All other requests retain their existing defaults. Success caching remains the action's responsibility; errors remain no-store. See the startup-free regression checks and deferred IIS gate in `docs/mobile-artwork-validation.md`.

This one public, identity-free action uses `Cache-Control: public, max-age=300` and a weak `ETag: W/"<version>"`. The action explicitly sets a five-minute max age and ASP.NET sliding expiration for a constant per-response client lifetime, and disables origin/server caching so each request selects the current media base and ETag. This does not create a sliding server cache or renew a client's freshness without a network response. Image-file caching is separate and is controlled by the image host. Private API responses still use `no-store`. Version reflects media base/fallback, registered IDs, variants, URLs, content hashes, MIME types and dimensions. Image URLs carry their content revision as `?v=...` so changed artwork changes its cache key. The media host must serve the same file when that query is supplied; validate that in Integration.

Cache the successful envelope and its ETag per API environment (not per NineStar account). Revalidate with `If-None-Match` after freshness expires; `304` has no JSON body and retains the cached catalogue. Whole strong/weak tags, lists and wildcard are supported. A changed host/fallback returns `200` and a new version. Do not send conditional headers without a cached body; recover from an unexpected `304` by retrying once without the header. Fetch only images needed for the current/nearby screen; share image cache entries by URL or revision. Do not preload the entire catalogue's artwork. Keep the mobile client's existing 15-second timeout, retain the last successful catalogue on refresh failure, and preserve existing image rendering until this backend is available. Catalogue refresh must not replace account state or unsaved edits.

Maintain `tools/mobile-artwork-paths.txt` as the explicit public allowlist. When adding/changing files, run `python tools/update-mobile-artwork.py` (Python with Pillow), then commit the generated `Helpers/MobileArtworkRegistry.Data.cs`. `--check` verifies source files, image metadata and hashes against the committed catalogue; this is a development check, never a server-startup operation. Preserve existing IDs when filenames move by adjusting the generator/registration deliberately rather than deriving a new client contract accidentally. New private directories must never be allowed.

Validation here: the registry/DTO and isolated controller compiled with C# 7.3 against .NET Framework 4.7.2 reference assemblies with warnings treated as errors (controller compilation used actual MVC/Newtonsoft references and a stub for the existing media-base property); 27 isolated catalogue/ETag/URL/fallback/isolation checks ran under the available Linux .NET runtime. Metadata regeneration, C# syntax, project registrations and literal source coverage are also checked. Added Windows xUnit tests cover the controller, conditional GET, safe failures, complete domain references, variants and public boundaries. Full VS2017 build/xUnit/IIS tests have **not** been executed here; use the handoff in `docs/mobile-artwork-validation.md` before release. No deployment is performed.

## Contextual explanatory help

Existing feature responses now include additive `data.help` dictionaries containing the website's resource-authored introductions and help text. This includes the Constitution introduction shown above the core-energy reading. These introductions are different from a calculated energy's `description` or a personal `sections.*.body`; those existing readings and fields remain unchanged. No website views are rerouted, and no new endpoint, migration, billing operation or global help download is introduced.

Each entry has this shape (text shortened for illustration):

```json
{
  "help": {
    "core": {
      "title": "Constitution",
      "htmlBody": "<h4>Constitution...</h4><p>...</p>",
      "culture": "en-GB"
    }
  }
}
```

Keys are stable, case-sensitive protocol identifiers. Titles and HTML come from existing strongly typed Globalisation resources, resolved at request/projection time using the existing resource culture; `culture` reports that culture. Do not hardcode the English examples in the app. Missing keys mean unavailable/not applicable, and must be handled gracefully. Do not use absence as an entitlement signal; retain the shared entitlements/access-error contract.

| Response | Help keys |
| --- | --- |
| Full personal chart (`me`, `calculate`, `last`, and full complementary chart results) | `overview`, `core`, `character`, `socialExpression`, `dayStar`, `generation`, `epoch`, `adultChildRelationship`; `relationships` only when the chart person is sixteen or over |
| Own/free chart and free complementary chart projection | Same public introductions, excluding `adultChildRelationship`; `relationships` retains the sixteen-or-over gate |
| Predictions (`me`, `calculate`, `last`) and planner | `day`, `month`, `year`, `generation`, `epoch`, `globalKiWarning`, `moreOnCycles` |
| Calendar `month` | `overview`, `subscription` |
| Compatibility calculation/last/complementary result | `overview` |
| Numerology calculation/last | `overview` |
| Biorhythms calculation/last | `overview` |
| I Ching `cast` | `overview` |
| Knowledge base, all callers | `origins`, `lightAndDark`, `yinYang`, `modalities`, `mainEnergies`, `characterEnergies`, `year`, `month`, `personalChart`, `relationships` |
| Knowledge base, authenticated account eligible for the website's Free-level content, or authenticated admin | Additionally `elements`, `fire`, `earth`, `metal`, `water`, `tree`, `transformations`, `sexuality` |

The six chart introductions map to `MainEnergyDescription`, `CharacterEnergyDescription`, `SurfaceEnergyDescription`, `DayStarDescription`, `GenerationalEnergyDescription` and `EpochEnergyDescription`. Relationship help uses `relationships_description`; adult-child help uses `AdultChildRelationship`. Cycle introductions use `day_description`, `month_description`, `year_description`, `generation_description`, `epoch_description`; the Global Ki caveat is separate `globalki_display_warning`, and More on Cycles uses `holograhpic_spirals`. Calendar help uses `PersonalCalendarHelp` and `CalendarSubscriptionHelp`. Other feature overviews use `compatibility_subtitle`, `karmic_numerology`, `biorhythms` and `i_ching_info` respectively.

Knowledge-base articles retain the website's Free-membership access check for the newly exposed element/transformation/sexuality articles. Anonymous callers receive only the new public introductions. Existing reference arrays and their existing access behaviour are unchanged. Template placeholders in five-elements/transformations/sexuality HTML are resolved through the existing TemplateParser with the configured media base, existing transformation resources and the absolute website knowledge-base URL. No Razor controls or layouts are returned.

Help appears once at its owning response level, never on each energy or planner row. Full profiles embedded in predictions or compatibility have `help: null`, avoiding duplicate chart introductions, especially across two compatibility profiles; use the standalone chart response for chart help. Other standalone detail/number-only/anonymous preview responses, existing content pages, alchemy, subscription/revoke responses and artwork catalogue retain their current shape. There is no invented hourly introduction because the website does not supply one in these panels. The website's "Click on a tab below" instruction, collapsed/expanded state, panel wrappers, loading messages and navigation controls are not API content.

Render `htmlBody` as the same resource-authored rich content used elsewhere in the app. Never append it into a calculated reading or substitute it for paid prose. Show `globalKiWarning` when presenting Global Ki. Embedded media remains ordinary resource-authored HTML; it does not create duplicate artwork-catalogue registrations. Existing response caching remains private/no-store, and the mobile timeout remains 15 seconds.

### Source audit and validation

The audit covers all `Html.CollapsiblePanel` calls in `Views/PersonalChart`, `Views/Predictions` and `Views/IChing`, the explicit calendar help collapse, and related feature/knowledge-base introduction panels. Energy/child/trigram interpretations, individual cycle descriptions and I Ching detailed interpretations already have existing chart/energy/cycle/hexagram fields and are not duplicated in help. This is not an automatic dump of every resource, tooltip or view.

Added tests cover website-resource parity, free/member and age filtering, the help transport shape, template replacement, per-response isolation and unchanged reading projections when embedded help is omitted. Existing free-energy field allowlists and existing authentication/ownership checks remain in place. Cloud validation is limited to C# syntax, resource/accessor/template checks, project registration and source/diff checks; a Windows VS2017 build, xUnit execution and signed-in Integration verification are required before release. No website startup, schema execution or deployment was performed here.

## Current implementation

### Shared entitlements: website-membership phase

Following the app review of contract revision 2, the first phase is implemented in source: authenticated read-only `GET /api/mobile/v1/membership/entitlements` and the identical additive `account/me.entitlements` projection. The snapshot contains only existing website membership grants and uses the revision 2 response shape (all Apple-only fields null). Existing account fields remain present. Both reads use `MembershipService.GetExistingActiveMembership`, without creating a free grant when no eligible record exists; in that case sources is empty, access flags are false and account/me.membership is null. A missing account returns 404; an anonymous/unresolved session returns 401. Admin role alone does not grant subscription benefits.

The shared mapper uses existing UserMembership paid/unlimited/activity rules, retaining inclusive local legacy end dates, lifetime and deactivation behaviour. It does not cache entitlements, query Apple, change schema, consume/reset complementary credits or alter existing chart access checks. The OAuth read-only entry point delegates the same lookup without changing its identity contract. No views, plugin API or website purchase flows are rerouted.

Tests were added for free/weekly/monthly/annual/lifetime projection, missing/expired/future/deactivated grants, ownership/allowlists, legacy dates, snapshot parity, identity/logout/revocation changes and no free-grant creation. Source syntax, project registrations, route inventory and whitespace are checked here; full Windows solution build, xUnit and IIS/device validation remain outstanding for this change. No Integration deployment is part of implementation. App consumption must update account-scoped entitlement state without overwriting unsaved profile edits, retain the 15-second timeout and tolerate the property being absent until the backend is deployed.

Apple contract sections below remain proposed and unimplemented. No Apple bindings, provider grants, product setup, verification or purchasing is introduced by this first phase.

The mobile JSON transport implementation and controller-family coverage audit are complete on `feature/mobile-json-api` for the scope below. This is **source completion**, not a passing build, executed integration tests or release approval. The separate API covers calculator/readings, account/session, predictions/planners, calendar/report data, membership/payment, booking, blog/support, metadata/content and reviewed administration operations. Existing website routes and views continue to use their original entry points and are not switched to HTTP calls through the mobile API.

See [mobile-api-routes.md](mobile-api-routes.md) for declared routes. Every transport projection is an explicit object or DTO. Do not serialize the EF entity, calculated view-model graph, payment provider object or service result wholesale. The existing plugin ApiController is unchanged.

## Authentication and requests

The native client can use the existing Forms Authentication provider: retain the server cookies, call `GET account/csrf`, send the returned request token in `X-CSRF-Token` on **every POST**, and fetch a fresh token after login/logout. Login, reset/change-password and deletion require HTTPS outside localhost. Cookie-authenticated mobile operations do not use an embedded API key, a supplied user ID, or a second identity database. The branch now includes current master's OAuth implementation, EF model mappings, existing migration and dependency updates through an upstream merge; the mobile transport does not replace or duplicate that implementation. OAuth/PKCE support for native apps requires integration with that implementation, not a parallel token system.

`MobileApiControllerBase` implements no-store JSON responses, redirects/error-page suppression, request-size limits (64 KiB), and CSRF validation before action execution. Actions explicitly check anonymous/member/admin and ownership requirements. `RequireUser` resolves existing Current identity/impersonation; checkout, account deletion and password changes refuse impersonation where the underlying provider operates on the actual login. Codes are protocol identifiers, not hardcoded UI messages. Native clients localise them.

JSON uses UTF-8, camelCase, ISO timestamps and no type metadata. Success is `{ success: true, data, error: null }`; failure is `{ success: false, data: null, error: { code } }` with an appropriate HTTP error status. Managed framework/routing failures in the mobile URL namespace are also converted to the JSON envelope by a path-scoped Application_Error handler. Normal website errors retain their existing handling. IIS-level failures before ASP.NET executes still require deployment checks.

Calculation requests explicitly include person, selected local date/time, birth/user timezone, calculation method, calculator type, house display and both hemisphere flags. Birth dates are date-only, birth time is separate, and selected timestamps have no UTC suffix. Validate real Windows/IANA timezones rather than silently falling back to GMT. Profile/compatibility calls combine birth time once. Planner calls pass birth date and time separately because the existing planner adds the time. Date-range limits and enum validation bound requests. The public chart preview returns numbers/overview only; full profile, compatibility, predictions and numerology use paid/admin routes. Separate `readings/*` routes apply the complementary section policy and counters.

## Shared services and projections

Calculations use NineStarKiService, NumerologyService, BiorhythmsService, IChingService, AstrologyService, ReportsService and existing planner batches. CombinedPredictionHelper populates request-owned planner results; personal and global descriptions remain separate. JSON projections preserve full source descriptions, morning/afternoon and inverted energies, lunar information, period boundaries, directions, numerology codes and series values. No Razor rendering is used by the new JSON endpoints. PDF and ICS remain existing file/download endpoints, with URLs or JSON metadata supplied as needed.

Profile projection preserves the website relationship age gate. Compatibility forwards the selected calculator and birth-time information and omits chemistry text/score when hidden or either person is below sixteen. I Ching templates are now copied before adding changing-line interpretations; otherwise one request could mutate the shared template used by another reading. A regression test covers that isolation.

The request culture is returned with profiles, but existing resource dictionaries and summary caches have inherited culture-isolation limitations. No claim of complete multilingual parity is made. Future work must fix those in their owning models/resources, rather than duplicating texts in the API.

## Account and ownership rules

Account updates load the authenticated user's entities and set an allow-list of profile fields. No client-supplied user ID, role, membership or password field is trusted. UserInfo is updated through the existing typed repository rather than passing untrusted strings to the legacy raw-SQL update helper. Preference updates accept only named calculation/timezone flags, never arbitrary preference keys. Reset-request responses never return the legacy service's password-reset token or indicate whether an email exists. Activation requires a real token or an OTP handle plus code/captcha. Deletion requires explicit confirmation and password reauthentication. All such endpoints only execute these effects when called; authoring them does not send messages or change accounts.

Booking list/calendar responses expose availability without other customers' names or bookings. Selecting a slot verifies the owning user-consultation association, matching duration, future time and availability, then delegates scheduling under the existing shared EF context's serializable transaction. Administrative slot edits are separately guarded.

## Existing Stripe payment contract

Mobile checkout uses the existing Stripe, membership, consultation and donation owners. Prices and eligibility come from server-side membership/consultation data; a client never posts a trusted PurchaseModel or successful-payment assertion. A caller supplies a stable request GUID. Stripe creation uses an idempotency key bound to that GUID and user. Confirmation reloads the server-created intent and verifies succeeded status, quoted amount and currency before fulfilling it.

Private checkout state is persisted as a purpose-bound MachineKey-protected JSON record under an internal `MobileCheckout:` UserPreference key, using the **existing database/context**, not a new table, migration or persistence project. These keys are not exposed through preference APIs. The existing public preference writer now accepts only ASCII identifier keys without colons, preventing users from overwriting/replaying internal checkout records, including through SQL-collation Unicode lookalikes. Protection is bound to both user ID and request GUID. Stable application MachineKey material must be retained for durable records to remain readable. A serializable SQL transaction protects creation/completion across workers; completed requests return the existing result rather than granting again. User ID, intent, amount and product association come from persisted server state. The existing Stripe interface has an additive idempotency overload; normal website callers retain their original method.

This requires Windows/SQL Server/Stripe integration tests for transaction sharing, concurrent completion, failed commit/retry and real provider responses. Legacy service email sends are not transactional with SQL and may require reconciliation if a surrounding transaction fails. No claim of exactly-once email delivery is made. Checkout is **not release-ready** before those checks. No payment, schema migration, merge or deployment has been executed during this work.

## Audited implementation coverage

| Family | Implementation status |
| --- | --- |
| PersonalChart | Full own chart, arbitrary-person full calculation, public numeric preview, free own-chart sections, alchemy and paid retrieve-last implemented. Complementary section projection and own/complementary alchemy implemented through the readings routes. |
| Predictions | Calculation, own snapshot, planner hierarchy, generic cycles, combined descriptions, moon phase and alchemy implemented. Paid retrieve-last, own/free yearly sections and complementary yearly sections/alchemy implemented. |
| Compatibility | Paid calculation with explicit privacy/age checks and retrieve-last implemented; complementary calculation omits the two full personal-chart payloads. |
| Numerology/Biorhythms | Paid calculations, numerology forecast and structured planners/series implemented. Paid retrieve-last implemented. Complementary numerology implemented. The website paywalls biorhythm gauges, planners and analysis; these remain paid. |
| IChing/KnowledgeBase | Public cast and structured knowledge-base data implemented. |
| PersonalCalendar | Month JSON and protected subscription/revoke implemented; existing ICS feed retained. |
| Reports | Structured yearly report, existing PDF URL and admin-only prompt implemented. |
| Account | Session login/logout, CSRF token, own account, profile/preferences updates, registration/activation, password operations and deletion implemented. Captcha-protected registration availability, opaque-link unsubscribe and admin promotion email operations are implemented. First-party cookie login is supported; native OAuth/PKCE and external provider browser callbacks are a separate authentication integration, not reimplemented here. |
| Membership/Payments | Options and quotes, promo redemption and server-verified durable checkout implemented. Provider/SQL tests required. |
| Consultation | Owned bookings, slots/calendar, transaction-protected selection and admin slot operations implemented. |
| Blog/Support | Public articles/tags, comments/likes/moderation, contact/newsletter submission and donation totals implemented. Article editing, publishing and deletion delegate existing owners. Per-user messages mirror the existing owned CRUD without adding delivery semantics. |
| Generic administration and operations | Twenty explicit entity contracts provide admin-only schema/read/page operations and selected repository-backed writes. Users and processed email queue are read-only here; user lifecycle and sending use their owner services. Custom user lifecycle and assignments, impersonation, article publishing, template test/single/list sending, logs, free-reading maintenance and MailChimp sync are implemented. Six explicit association pairs support atomic replacement with target validation. Chosen-list recipient data, deduplicated donor import and bounded resource HTML/text export are implemented. SQL authoring/execution is intentionally excluded; stored association selection uses explicit IDs. No arbitrary type/table or internal-preference endpoint exists. |

## Chart overview table metadata

Full energy DTOs now include the six missing website header-table values. The free own-chart projection additionally returns `data.energies`, keyed by the same roles as the full chart: `epoch`, `generation`, `core`, `character`, `socialExpression`, `dayStar` and `hour`. Each free entry is an explicit `MobileChartEnergyOverview`, containing only `number` and the nine table values below. Existing numeric properties at `data.core`, `data.character`, etc., `summary`, `overview` (a string) and the three already granted `sections` remain unchanged. There is no new endpoint or entitlement requirement.

| JSON field on each energy | Type | Existing website value |
| --- | --- | --- |
| `colour` | string | `MetaData.GetColour()` |
| `direction` | string | `MetaData.GetDirection()` |
| `familyMember` | string | `MetaData.GetFamilyMember()` |
| `season` | string | `SeasonDescription`, exactly as in `_ProfileHeaderInfo.cshtml` |
| `internalRepresentation` | string | `MetaData.GetInternalRepresentation()` |
| `physiognomy` | string | `MetaData.GetPhysiognomy()` |
| `primaryAttributes` | string | `MetaData.GetPrimaryAttributes()` |
| `mostHarmoniousCombinations` | string | `MetaData.GetMostHarmoniousCombinationsString()` |
| `mostChallengingCombinations` | string | `MetaData.GetMostChallengingCombinationsString()` |

These are resource-backed display strings, not new paid interpretations. Combination fields preserve the website's comma-separated number-and-element labels; do not interpret them as JSON arrays or recalculate compatibility in the app. `number` is an integer. The full `MobileChartEnergy` inherits this allowlisted metadata and retains every existing paid field/artwork reference; JSON stays flat, for example `data.energies.core.season`. Other existing full-energy projections (cycles, planners, knowledge base) inherit the additive fields too.

Free entries have exactly ten properties. They do **not** include `description`, `elementDescription`, `modalityDescription`, `trigramDescription`, `yinYangExpansionDescription`, or additional paid sections. Use these entries to connect the overview table; they are not a substitute for the full paid energy DTO. A missing energy produces a null entry. Older deployed backends can lack `energies` or the new properties; omit unavailable rows gracefully until the new revision is deployed.

The own free endpoint `GET /api/mobile/v1/personal-chart/me/free` and already authorised complementary profile responses share this projection. Public anonymous `personal-chart/preview` and the `Numbers` projection remain unchanged. Existing authentication, ownership, membership, complementary-credit checks and no-store policies are unchanged. No website views/helpers, resources, billing or schema are changed.

Validation handoff: build with VS2017 and run `MobilePersonalChartControllerTests`, including the nine-energy website-row parity cases and the exact free-DTO allowlist, plus the existing mobile access and artwork tests. After an owner-authorised Integration deployment, compare full and free `energies.core` metadata for the same account/settings and the website header, verify all seven roles, the existing three free sections and absence of paid descriptions. The mobile app continues to use Integration directly and retains its 15-second timeout. Source syntax/registration/whitespace checks are performed here; Windows build/tests and live Integration verification remain outstanding for this revision. No deployment or migration execution is part of this source change.

## Additional transport contracts

`GET personal-chart/me/free` returns numbers, the allowlisted overview table metadata described above, and the core, character and intellectual sections granted by the website for one's own chart. It never calls the full mapper or returns energy descriptions for paid sections. Full `/me` remains paid. The five calculator `/last` endpoints save only validated request inputs under a user-bound session key and recalculate after fresh entitlement checks. Saved settings include birth time/timezone and calculation flags; switching identity cannot retrieve another user's saved request. Logging out clears this session state. Anonymous previews are not saved.

`content/{page}` returns resource-backed title and authored content for privacy, terms, FAQ, data removal, calculation methods, southern hemisphere and AI information. Resource-authored HTML is content, not a rendered view. `metadata/options` supplies protocol values with resource-backed enum labels; `metadata/timezones` mirrors the existing extended selector. Administrative schemas include resource-backed field labels and enum choices. No complete multilingual guarantee is implied.

Bulk association pairs are `user-roles`, `role-permissions`, `user-mailing-lists`, `contact-mailing-lists`, `mailing-list-users` and `mailing-list-contacts`. Replacement takes `{ ownerId, selectedIds }`, rejects missing/deleted targets, and applies repository changes in one serializable transaction. An empty array clears the selection; automatic mailing lists cannot be edited as stored rows. General records and association reads exclude soft-deleted rows.

Administrative mail and operational endpoints execute effects only when explicitly called. Test/single/list sends reuse MailerService; free-reading maintenance is shared with the existing web controller through MembershipService. The all-account maintenance and MailChimp sync routes require `confirm: true` as part of their request contract. Updating another user's saved preferences no longer replaces the administrator's current session preferences.

## Complementary readings and images

`POST readings/personal-chart`, `readings/predictions`, `readings/compatibility` and `readings/numerology` are explicit credit-consuming operations for free memberships. They validate inputs, read available credits and delegate the relevant MembershipService decrement under the shared scoped DbContext's serializable transaction. Projection/serialization precedes commit so a failed calculation does not consume the credit. These transaction semantics still require SQL Server concurrency tests. Paid accounts use the normal calculator routes; they are not debited here.

Complementary profiles expose the same restricted sections as the own/free chart. Complementary predictions expose the yearly personal, lunar and global interpretations; they do not return all paid periods or the full profile graph. Complementary compatibility exposes compatibility detail and numeric personal charts, rather than returning unrelated paid natal sections. Numerology follows its complementary grant for the calculation and outlooks. `GET readings/{section}/last` recalculates the session's successfully granted reading without another debit. The grant is server-held, bound to the current user, expires with the session and cannot be created by the paid retrieve-last routes. New POSTs each consume a credit; clients should use the corresponding GET to reopen a successful reading rather than replaying POST. Own and successfully granted complementary profile/prediction alchemy use existing services and the enabled setting.

`POST media/avatar` writes only the current account's avatar. `POST media/article-image` and deletion require administration. JPEG/PNG payloads are limited to 2 MiB, decoded, checked for dimensions, resized and re-encoded as PNG with server-generated filenames. The media transport alone allows multipart framing above the normal 64 KiB JSON cap. Storage uses MediaManagementService and fixed mobile image folders. No caller-supplied filesystem folder, executable file extension, arbitrary CLR image-option type or arbitrary deletion path is accepted. Avatar removal clears the current account's reference and removes only an owned mobile image; it does not delete external/legacy media. Account responses include the avatar URL. Image decoding and storage still need Windows/IIS tests.

## Scope boundaries and validation gates

- The app can use the implemented first-party Forms Authentication cookie flow. Native OAuth/PKCE, Facebook/Google redirects and provider callbacks must use the existing authentication owner when selected for the app; this branch includes that owner through the upstream merge and does not duplicate its infrastructure.
- Promotion emails, opaque-link unsubscribe, donor import, chosen-list recipient data and resource exports have JSON contracts. Recipient SQL authoring, arbitrary SQL execution and edits to processed email queue rows are excluded administrative tools. Sending uses MailerService and its queue rules. Existing ICS/PDF endpoints continue to serve their file formats.
- Media management deliberately supports owned avatars and admin article images; arbitrary file upload, arbitrary type-based dynamic image controls and arbitrary-path deletion are excluded from the JSON transport.
- Pure view layouts, success/failure redirect pages, debug/test controllers, raw SQL maintenance utilities and controller infrastructure helpers are not mobile data endpoints. Any newly required operational capability must use a reviewed explicit contract.
- Build the complete branch on Windows with restored K9 packages and decrypted astronomy sources, run xUnit, then validate routing/error JSON, cookie/CSRF handling, ownership and section grants under IIS.
- Exercise SQL transaction sharing and concurrent complementary/booking/checkout writes; test failed commit/retry and Stripe confirmation before release. Source checks cannot prove these behaviours.

## Validation and next gates

Added input/timezone/birth-time, I Ching isolation and session identity/access-revocation, free-section projection, image-boundary, protected-action access, transport-error, donor deduplication and moderation tests alongside the existing mobile profile tests. C# syntax, project XML, explicit compile registrations, unchanged plugin-controller sources and whitespace are checked in this environment. The environment has no .NET Framework/MSBuild/Mono runtime or restored K9 binaries, and astronomy source is git-crypt protected. Compilation, xUnit execution and MVC/IIS/SQL/Stripe integration tests are **not executed**. A draft PR is not a deployment approval.

The source coverage audit is closed with the explicit exclusions above. Follow [mobile-api-validation.md](mobile-api-validation.md) for the Windows build, xUnit, IIS and provider gates and the first app integration steps. The mobile layer must not be described as release-ready before those checks pass.


## Website compatibility and final workflow notes

No views, existing URL registrations or plugin ApiController source were changed by this mobile expansion. Existing maintenance now delegates its original operation to MembershipService. Shared backend additions include Stripe idempotency support, internal preference protection/session isolation, I Ching template isolation, a missing-list null guard, constrained image storage and moderation authorisation. ArticlesService now permits authors/admins to edit/delete comments and requires an administrator for approval/rejection; the mobile transport delegates those operations to that same owner, preserving like cleanup. The application-level error hook is restricted to `~/api/mobile/v1` and does not intercept ordinary website errors.

Registration availability requires captcha and returns correctly named availability booleans. Unsubscribe uses the existing opaque external link identifier and owner services, accepts only `user` or `contact`, and requires the mobile CSRF pair. Promotion email acceptance does not assert delivery, because the existing owner does not report all delivery failures. Donor import does not alter existing contacts or resubscribe them and deduplicates email addresses within the import. Admin resource export accepts catalogued folder names, pages at most 100 files and limits source bytes per page; it returns authored HTML and extracted text, never a rendered view or arbitrary file path.

## Concrete controller action inventory

The list below records declared MVC-returning actions from the existing controller files, with overload names deduplicated. It deliberately includes utilities for review, not automatic exposure. `GetObjectName` and other infrastructure methods are not app endpoints. See the coverage table for intended treatment.

| Source file | Declared actions |
| --- | --- |
| `AccountController.cs` | `Login`, `LoginUser`, `Facebook`, `FacebookCallback`, `GoogleVerify`, `ExternalAuthCallback`, `ExternalAuthPostRegsiter`, `AccountLocked`, `LogOff`, `Register`, `RegisterDetails`, `UpdatePassword`, `UpdatePasswordSuccess`, `MyAccount`, `ViewAccount`, `UpdateAccount`, `DeleteAccount`, `EmailPromoCode`, `EmailPromoCodeToUser`, `PromoCodeEmailSent`, `RemoveMyData`, `ConfirmDeleteAccount`, `DeleteAccountSuccess`, `DeleteAccountFailed`, `PasswordResetEmailSent`, `PasswordResetRequest`, `ResetPassword`, `ResetPasswordFailed`, `ResetPasswordSuccess`, `AccountCreated`, `VerifySixDigitCode`, `AccountActivated`, `AccountVerified`, `AccountActivationFailed`, `AccountAlreadyActivated`, `ActivateAccount`, `ActivateUserAccount`, `UnsubscribeContact`, `UnsubscribeUser` |
| `AccountControllerAjax.cs` | `IsUserNameAvailable`, `IsEmailAddressAvailable`, `SetCurrentUserTimeZone`, `ResendActivationCode` |
| `AdminController.cs` | `DisplayCompleteGlobalisationContents`, `DownloadTextFile`, `RunMaintenance`, `MaintenanceComplete` |
| `ApiController.cs` | `GetPersonalChart`, `GetPredictions`, `GetCompatibility`, `GetKnowledgeBase`, `GetIChing`, `GetCompatibilityTest`, `UploadFile`, `RunMaintenanceScript` |
| `ApiPureAlchemyController.cs` | `CalculateNineStarKiAjax` |
| `ArticlesController.cs` | `Preview`, `Publish`, `Unpublish`, `PublishSuccess`, `UnpublishSuccess`, `GetAllTags` |
| `BaseNineStarKiController.cs` | `SetLanguage`, `RedirectToLastSaved`, `UpdateUserPreferenceInt`, `UpdateUserPreferenceString`, `UpdateUserPreferenceBool`, `UploadFile`, `DeleteImages`, `GetImageOptions` |
| `BiorhythmsController.cs` | `Index`, `IndexPost`, `RetrieveLastBiorhythms` |
| `BlogController.cs` | `Index`, `Redirection`, `View`, `Details`, `PostComment`, `ToggleCommentLike`, `DeleteComment`, `EditComment`, `Dashboard`, `ApproveComment`, `RejectComment`, `SaveComment`, `ModerateDeleteComment` |
| `CompatibilityController.cs` | `Index`, `IndexPost`, `RetrieveLastCompatibility` |
| `ConsultationController.cs` | `BookConsultationStart`, `BookConsultation`, `ProcessConsultation`, `BookConsultationSuccess`, `ScheduleConsultation`, `ConfirmSlot`, `SelectSlot`, `ScheduleConsultationSuccess`, `CreateFreeSlots`, `ViewAvailableSlots`, `ViewCalendar`, `Calendar`, `UpdateSlot`, `CreateSlot`, `DeleteSlot` |
| `ConsultationsController.cs` | Inherited CRUD; inspect base controller. |
| `ContactsController.cs` | `ImportContactsFromDonations`, `SignUpToNewsLetter`, `SignUpSuccess`, `AddAllContactsToMailChimp`, `MailChimpImportSuccess` |
| `CountriesController.cs` | Inherited CRUD; inspect base controller. |
| `DonationsController.cs` | Inherited CRUD; inspect base controller. |
| `EmailQueueController.cs` | Inherited CRUD; inspect base controller. |
| `EmailTemplatesController.cs` | `SendToUser`, `SendToMailingList`, `TestEmailTemplate` |
| `ErrorController.cs` | `Index`, `NotFound`, `Unauthorized` |
| `HomeController.cs` | `Index`, `GptInfo`, `PrivacyPolicy`, `TermsOfService`, `FAQ`, `HowToRemoveYourData`, `CalculationMethods`, `SouthernHemisphere` |
| `IChingController.cs` | `Index`, `Toss` |
| `IbogaController.cs` | `Index` |
| `KnowledgeBaseController.cs` | `Index`, `RetrieveLastKnowledgeBaseSection` |
| `LogController.cs` | `Index` |
| `MailingListContactsController.cs` | `EditMailingListsForContact` |
| `MailingListUsersController.cs` | `EditMailingListsForUser` |
| `MailingListsController.cs` | `SelectUsers`, `SelectUsersTable`, `SelectUsersTableJson` |
| `MembershipController.cs` | `Index`, `PurchaseStart`, `UpgradeNotPossible`, `Purchase`, `ProcessPurchase`, `PurchaseSuccess`, `PurchaseCancelSuccess`, `SwitchStart` |
| `MembershipOptionsController.cs` | Inherited CRUD; inspect base controller. |
| `MessagesController.cs` | Inherited CRUD; inspect base controller. |
| `NumerologyController.cs` | `Index`, `Link`, `CalculateForecast` |
| `PaymentController.cs` | `GetPaymentIntent`, `ProcessPayment` |
| `PermissionsController.cs` | Inherited CRUD; inspect base controller. |
| `PersonalCalendarController.cs` | `Month`, `Subscription`, `Revoke`, `Feed` |
| `PersonalChartController.cs` | `Index`, `IndexPost`, `GetAlchemy`, `MyProfile`, `RetrieveLast`, `RetrieveLastProfile`, `GetAllEnergies` |
| `PredictionsController.cs` | `Index`, `IndexPost`, `GetAlchemy`, `GetPlanner`, `GetHourlyPredictions`, `GetDailyPredictions`, `GetMonthlyPredictions`, `GetYearlyPredictions`, `GetNineYearlyPredictions`, `GetEightyOneYearlyPredictions`, `MyCycles`, `RetrieveLastPredictions`, `GetCombinedMonthlyPredictions`, `GetCombinedDailyPredictions` |
| `PromotionsController.cs` | `Get` |
| `ReportsController.cs` | `YearlyReport`, `YearlyReportPdf`, `YearlyReportPrompt` |
| `RolePermissionsController.cs` | `EditPermissionsForRole` |
| `RolesController.cs` | Inherited CRUD; inspect base controller. |
| `SupportController.cs` | `Index`, `_ContactUs`, `ContactUs`, `ContactUsSuccess`, `DonateStart`, `Donate`, `ProcessDonation`, `DonationSuccess`, `DonationCancelSuccess` |
| `SystemSettingsController.cs` | Inherited CRUD; inspect base controller. |
| `TestController.cs` | `Index`, `SendMail` |
| `UserConsultationsController.cs` | Inherited CRUD; inspect base controller. |
| `UserMembershipsController.cs` | Inherited CRUD; inspect base controller. |
| `UserPromotionsController.cs` | Inherited CRUD; inspect base controller. |
| `UserRolesController.cs` | `EditRolesForUser` |
| `UsersController.cs` | `AssignMembershipStart`, `AssignMembership`, `AssignMembershipSuccess`, `AssignFreeConsultationStart`, `AssignFreeConsultation`, `AssignFreeConsultationSuccess`, `AssignPromoCodeStart`, `AssignPromoCode`, `AssignPromoCodeSuccess`, `ViewAccount` |

## Shared website/app membership and Apple subscriptions: proposed contract

**Status: draft for agreement, 23 October 2026. No Apple endpoints, persistence, migration, purchase verification or deployment are implemented by this proposal.** This section supersedes any app-only subscription proposal. App notes belong in the separate `ninestar-mobile/docs/` directory; this file is the shared backend contract.

**Contract revision 2:** resolves the app review of `efe0d438`: token-error precedence, complete schemas/examples, unfinished-purchase recovery, plan changes/retired products, and the current 15-second client timeout. This revision remains proposed until agreed. The review findings were supplied in chat; the separate local `ninestar-mobile/docs/apple-contract-review.md` file is not accessible from this backend workspace.

The owner reports that Expo login succeeds on iPhone and My Account/own-chart screens exist, with further device validation outstanding. Those observations do not validate Apple billing.

### Product decision and existing architecture

One existing NineStar account is used on website and app. An eligible website membership grants corresponding mobile access; a verified Apple subscription grants corresponding website and API access. Eligibility follows the existing active paid/unlimited membership rules, rather than a separate mobile paywall. Keep free own-chart/complementary rules, ownership, age gates, administration permissions and existing membership benefits.

Inspection findings and extension points:
- `DataAccess/Models/UserMembership.cs` owns membership dates, deactivation, complementary counters and paid/unlimited predicates. Legacy activity uses inclusive calendar dates; Apple expiry must use an exact UTC instant.
- `MembershipOption` defines Free, WeeklyPlatinum, MonthlyPlatinum, AnnualPlatinum and LifeTimePlatinum. Existing paid periods are unlimited; duration is not a new benefit tier.
- `MembershipService.GetActiveUserMembership(s)` selects the highest active option. Its fallback may create a free membership. The OAuth lookup is deliberately read-only. Website paywalls, mobile checks and calendar access already use these paths.
- `MembershipService.CreateMembership` and `AssignMembershipToUser` call `TerminateExistingMemberships`. The purchase method uses `Current.UserId`, sets locally calculated dates and may send mail/create a lifetime consultation. **Do not call these methods for Apple verification, restore or renewal.**
- `MobileCheckoutService` coordinates existing Stripe fulfilment, with private checkout records in UserPreference. This is not an Apple subscription ledger.
- `Current.UserId` can represent impersonation. Apple account linking must use the actual authenticated WebSecurity account and reject impersonation, as existing sensitive account/payment operations do.
- `MailingListService` contains direct date-based paid-member queries; `PromotionService`, `EmailQueueService` and generic membership administration also read membership records. Audit/update these narrow consumers for provider-managed UTC grants and protection, rather than assuming every check passes through one service.
- Existing Hangfire infrastructure can run notification processing/reconciliation. Do not introduce a second scheduler or database owner.

**Recommended initial mapping for agreement:** Apple monthly -> existing MonthlyPlatinum; Apple annual -> existing AnnualPlatinum, with the same benefits across both platforms. Exact App Store product IDs, bundle ID, numeric app Apple ID and subscription-group ID must be supplied/confirmed before implementation. Keep one subscription group, Family Sharing disabled and no Apple lifetime product for this first version. Existing website weekly, monthly, annual, lifetime, promotions and consultation benefits remain intact. Do not create/reset complementary credits or award lifetime consultations on monthly/annual renewals or restores.

### Smallest reliable backend extension

See [apple-verifier-compatibility.md](apple-verifier-compatibility.md) for the 23 October compatibility spike: actual framework-targeted package assets were inspected, the C# 7.3 probe compiled against Framework 4.7.2 references, and 15 primitive checks passed on an isolated Linux .NET 8 runtime. Windows/IIS execution and full Apple trust/transaction verification remain gates; the probe is not a production verifier. There is no identified fundamental framework blocker, and no requirement to contact Apple on each chart/page request. An authenticated app assertion alone is never payment evidence.

Retain the existing account/authentication system and `MembershipService`. Add one narrow Apple verification/synchronisation service behind an `IApplePurchaseVerifier` adapter, with a separate mobile subscription controller and a server-notification controller. The verifier must fit .NET Framework 4.7.2/C# 7.3/VS2017 and be independently tested before implementation is approved. Apple provides maintained server libraries for Node, Python, Java and Swift; do not assume an official legacy .NET package exists. Prefer a reviewed compatible JOSE/X.509 implementation; if compatibility cannot be established, bring back that concrete blocker before introducing a separate verification host. Do not hand-roll signature validation or assume decoding a JWT verifies it.

Proposed additive storage, all under `webapp/DataAccess` and the existing `LocalDb`:
1. `AppleAccountBinding`: existing UserId, server-generated random appAccountToken UUID, timestamps; unique UserId and unique token. Issue once and reuse across reinstall/restore; the token is an association identifier, not a credential.
2. `AppleSubscription`: immutable owning UserId/binding, bundle/environment/originalTransactionId, mapped MembershipOptionId, provider status, latest transaction/product, auto-renew flag, paid/grace/revocation timestamps, last verification/reconciliation time and concurrency version. Unique (bundle ID, environment, originalTransactionId); exactly one linked provider-managed UserMembership grant per chain.
3. `AppleTransaction`: unique (bundle ID, environment, transactionId), subscription association, signed product/group/token/ownership data, purchase/expiry/revocation information, verification timestamps and bounded evidence/hash. Transaction IDs are opaque strings throughout JSON and SQL, never JavaScript numbers. Retain refund/state revisions through the notification audit rather than discarding contrary evidence.
4. `AppleNotification`: durable inbox/audit keyed uniquely by environment/app/notificationUUID, verified signed payload or protected evidence, processing state/attempts/timestamps/error category. Unknown-owner but valid events are retained for reconciliation without granting access.

These records provide binding, subscription state, transaction idempotency and durable event recovery; a single current-membership row or user preference cannot safely replace them. No general billing platform or migration of historical Stripe records is needed.

Add nullable `Provider`, `AccessStartsAtUtc` and `AccessEndsAtUtc` fields to UserMembership for the Apple-managed grant (legacy Provider null; Apple Provider `apple`). Maintain its existing StartsOn/EndsOn for legacy presentation only. For provider grants, `IsActive` must require a recognised provider, non-deactivation and `AccessStartsAtUtc <= UtcNow < AccessEndsAtUtc`; missing bounds fail closed. Keep all legacy/null-provider date and lifetime semantics unchanged. The approved shared predicate/query must be used consistently by direct membership consumers as well as service lookups.

Apple synchronisation upserts only its chain's grant, using an explicit verified UserId, and updates provider evidence plus the grant atomically in the existing shared EF context. Membership lookup includes those grants, so existing website/API consumers receive the same access. Use fresh request-owned membership state; do not cache paid access in session, claims or profile/calculation caches. Provider selection changes must never transfer, reset or destroy counters on a separate legacy membership. Apple-grant complementary counters start at zero; preserve the existing free/legacy counters for fallback after expiry. Exclude provider-managed grants from free-credit maintenance and generic edits. Keep account-number identity compatibility for current plugin/OAuth consumers; do not rotate a grant's membership identity on renewal.

Change `TerminateExistingMemberships` narrowly to exclude Apple-managed rows; normal website-versus-website purchase behaviour remains unchanged. Apple updates never terminate legacy memberships. Generic admin APIs/forms must not manufacture/edit provider-managed grants, evidence or binding fields; provider ledger rows are not general CRUD resources. Explicit account suspension/deletion still takes precedence over any paid grant. Persist enough non-reassignable ownership history under the existing retention policy to prevent automatic relinking after deletion; do not recreate an account or reactivate a deleted/suspended user from a webhook.

### Entitlement and overlap rules

Effective access is the union of currently eligible legacy and Apple grants. Select existing membership options according to established precedence; lifetime retains its benefits. A refund/expiry/revocation changes only the relevant Apple grant and cannot remove an independent website grant. Do not add purchased durations together, convert billing periods, credit/prorate website purchases or cancel one provider on behalf of the other.

Recommended duplicate-billing policy: block initiating a new Apple purchase when an eligible website grant or an already-entitled Apple subscription exists. Return a machine-readable reason so the app offers existing access or subscription management. Add the equivalent check at website membership checkout initiation for an active Apple grant; website-only purchase behaviour remains unchanged. This avoids known duplicate starts but cannot guarantee that independent provider renewals or purchases already in flight never overlap. An already-paid, verified purchase must still be reconciled and linked to its rightful account even if another valid grant appeared meanwhile; never reject restore/fulfilment merely because access already exists. Preserve both provider records and show the billing sources. Changing plans/cancelling Apple billing remains an Apple operation.

### Proposed mobile endpoints

All relative paths below are beneath `/api/mobile/v1`. They retain the existing JSON envelope, no-store headers and cookie authentication. POSTs require the existing CSRF pair and HTTPS. New subscription endpoints reject impersonation and derive UserId from the actual authenticated account; no posted UserId, membership ID, email or account number can select an owner.

| Method and path | Request | Proposed successful data |
| --- | --- | --- |
| GET `membership/entitlements` | None | The entitlement snapshot defined below, freshly read from persisted verified state. No Apple network call or new grant on this GET. |
| POST `subscriptions/apple/context` | `{}` | `{ appAccountToken, environment, purchaseAllowed, purchaseBlockedReason, canManageSubscription, canChangePlan, products, entitlements }`. Idempotently get/create this account's binding. Products include recognised retired mappings as well as saleable ones, distinguished by `availableForPurchase`. Token is returned even when a new purchase is blocked, to support same-account restore. |
| POST `subscriptions/apple/verify` | `{ "transactionId": "opaque-storekit-id" }` | `{ processed: true, transactionId, state, entitlements }` only after server verification and durable transaction/grant commit. Echo the requested transaction ID; repeated same-owner submissions succeed without duplicate benefits. A valid inactive purchase can be recorded with `state: "expired"` or `"revoked"` and no Apple access. |
| POST `subscriptions/apple/refresh` | `{}` | `{ processed: true, complete, verifiedSubscriptionCount, pendingSubscriptionCount, retryAfterSeconds, entitlements }`. Reconcile this account's already-linked chains within the request budget. `complete: false` explicitly reports a partial refresh; see timing rules below. No arbitrary transaction/account selector. |

`purchaseAllowed` governs starting a **new** Apple subscription, not restoring or managing an owned chain. `purchaseBlockedReason` is null when allowed; otherwise choose `website_membership_active` first if any eligible legacy grant exists, then `apple_subscription_active` for an active/grace Apple grant, then `apple_subscription_requires_management` for billing retry. No second subscription should be offered blindly. Same-group plan changes remain available through Apple management even when a new purchase is blocked. `products` is a product mapping, not a price quote: display actual StoreKit-localised pricing/periods and disclosures.

Example entitlement snapshot (illustrative timestamps and enum values, not a live response):

```json
{
  "asOfUtc": "2026-10-23T20:00:00Z",
  "paid": true,
  "unlimited": true,
  "websiteAccess": true,
  "mobileAccess": true,
  "sources": [
    {
      "provider": "apple",
      "subscriptionType": "MonthlyPlatinum",
      "state": "active",
      "endsOn": null,
      "expiresAtUtc": "2026-11-23T18:00:00Z",
      "autoRenewEnabled": true,
      "productId": "example.platinum.monthly",
      "nextProductId": null,
      "nextSubscriptionType": null,
      "changeEffectiveAtUtc": null,
      "verifiedAtUtc": "2026-10-23T19:59:00Z"
    }
  ]
}
```

`paid` and `unlimited` represent membership benefits, not an admin-role bypass. `websiteAccess` and `mobileAccess` mean membership-derived paid access; free features still work when false. `sources` includes the current eligible legacy grant (if any) plus owned Apple chains, including inactive chains needed for management; it never exposes provider evidence, other accounts, tokens, secrets or transaction history. Provider `website` represents existing website/admin/promotion grants; its `endsOn` is the legacy inclusive date (`yyyy-MM-dd`, null for free/unbounded or lifetime grants) and `expiresAtUtc` is null. Apple uses the exclusive UTC access boundary, including verified grace when applicable, and null `endsOn`. A null legacy auto-renew flag is returned when actual provider renewal state is unknown; legacy IsAutoRenew alone is not proof of a Stripe recurring subscription.

Add the **same** snapshot as an additive `entitlements` property on `GET account/me`; preserve its existing `membership` fields and meanings for compatibility. Do not replace or rename current fields. The dedicated GET and account snapshot use one shared projection. Existing paid routes continue server-side membership enforcement; the app must not calculate access from its own receipt or dates. Fetch account/entitlements after purchase, restore and foreground, and handle fresh membership-required responses.

Restore is a client StoreKit operation followed by server verification: enumerate restorable/current transactions for the configured products, call `verify` per transaction, then `refresh` and read account/entitlements. No separate receipt-trusting restore endpoint is needed. If no device transaction exists, `refresh` can still recover this account's known chains. Finish a StoreKit transaction only after the server returns committed processing success; retain/retry on transport/provider failures. StoreKit pending/Ask-to-Buy/cancelled/unverified client states never grant access. An expired/refunded processed transaction can finish without unlocking.

Proposed error contract uses the existing failure envelope:
- 400: `invalid_apple_request`, `invalid_apple_transaction`, `apple_product_not_supported`, `apple_environment_not_allowed`.
- 401: `authentication_required`; existing 400 `invalid_csrf_token` remains unchanged.
- 403: `impersonation_not_allowed`, `apple_account_mismatch` (wrong/malformed/unrecognised token, unsupported ownership, another account's chain or a missing token on an already-linked chain without approved legacy status; exact precedence below). Never reveal the other owner.
- 409: `apple_account_link_required` only for a verified, supported, **unbound and tokenless** purchase needing reviewed support recovery. Never silently claim it or overwrite its token.
- 429: `apple_rate_limited`.
- 503: `apple_verification_unavailable` for provider/network/verifier/reconciliation failure. No new/unverified access on failure; retry with the same transaction ID. Do not return a durable-success assertion before commit.

The existing request-size limits apply. Bound and validate transaction ID strings (maximum 128 characters); no client JWS, expiry, price, plan or success flag is required/trusted.

### Revision 2: response schemas and state definitions

These are the complete proposed `data` shapes inside the existing success envelope. All listed fields are required, including nullable ones; clients tolerate additive unknown fields. A failure retains `{ success: false, data: null, error: { code } }`, not a success-shaped payload. Examples use fictional product IDs.

| Shape/field | Type and meaning |
| --- | --- |
| Entitlements `asOfUtc` | ISO 8601 UTC timestamp with Z, evaluated for this response. It does not make older Apple evidence newly verified. |
| Entitlements `paid`, `unlimited`, `websiteAccess`, `mobileAccess` | Four booleans. Access is the union of currently active paid sources; Free never grants paid access. Existing endpoint-specific benefits/ownership still apply. |
| Entitlements `sources` | Array of Source objects, empty when no existing legacy membership or Apple chain is present. It is not an account's entire purchase history. |
| Source `provider` | `website` or `apple`. |
| Source `subscriptionType` | Exact existing enum name: `Free`, `WeeklyPlatinum`, `MonthlyPlatinum`, `AnnualPlatinum`, `LifeTimePlatinum`. |
| Source `state` | `active`, `grace`, `billing_retry`, `expired` or `revoked`, as defined below. Website sources are current eligible grants and use `active`, including Free. |
| Source `endsOn` | Nullable date-only string, inclusive legacy website end date; null for Apple, free/unbounded or lifetime. |
| Source `expiresAtUtc` | Nullable exclusive Apple access boundary, including verified grace when applicable; null for website. Retain the past boundary for inactive Apple chains; null only if no trusted interval is available. |
| Source `autoRenewEnabled` | Boolean or null when not verified/not applicable. It is independent of current access. |
| Source `productId` | Current Apple product ID string, including retired recognised IDs; null for website. |
| Source `nextProductId`, `nextSubscriptionType` | Nullable verified scheduled renewal product and mapped enum. Both null if no verified change, auto-renew is off or source is website. Do not set them from an unconfirmed client selection. |
| Source `changeEffectiveAtUtc` | Nullable verified effective instant for the scheduled change. Null if unavailable; do not invent one from a guessed month length. |
| Source `verifiedAtUtc` | Last successful Apple server verification timestamp, or null for website/no verified evidence. |

Apple `state` is the latest verified subscription state, bounded by current time: `active` has a currently valid paid interval; `grace` has a current Apple-granted grace interval; `billing_retry` has provider-confirmed billing recovery but no current paid/grace interval; `expired` has no current entitlement and no applicable retry/revocation; `revoked` means the relevant current entitlement is invalidated by verified refund/revocation. A cancelled-but-still-paid chain is `active` with `autoRenewEnabled: false`. Once a known active/grace interval ends, it cannot remain entitled merely because no notification/job arrived; project `expired` unless verified evidence establishes retry or revocation. No pending client purchase state belongs in this server enum.

All example snapshots below are evaluated at `2026-10-23T20:00:00Z`. Free, website-only, lifetime, expired-Apple and overlapping-source examples are complete Entitlements objects; outer success envelopes are omitted.

**Free**

```json
{
  "asOfUtc": "2026-10-23T20:00:00Z",
  "paid": false,
  "unlimited": false,
  "websiteAccess": false,
  "mobileAccess": false,
  "sources": [
    {
      "provider": "website",
      "subscriptionType": "Free",
      "state": "active",
      "endsOn": null,
      "expiresAtUtc": null,
      "autoRenewEnabled": null,
      "productId": null,
      "nextProductId": null,
      "nextSubscriptionType": null,
      "changeEffectiveAtUtc": null,
      "verifiedAtUtc": null
    }
  ]
}
```


**Website-only annual**

```json
{
  "asOfUtc": "2026-10-23T20:00:00Z",
  "paid": true,
  "unlimited": true,
  "websiteAccess": true,
  "mobileAccess": true,
  "sources": [
    {
      "provider": "website",
      "subscriptionType": "AnnualPlatinum",
      "state": "active",
      "endsOn": "2027-02-28",
      "expiresAtUtc": null,
      "autoRenewEnabled": null,
      "productId": null,
      "nextProductId": null,
      "nextSubscriptionType": null,
      "changeEffectiveAtUtc": null,
      "verifiedAtUtc": null
    }
  ]
}
```


**Website lifetime**

```json
{
  "asOfUtc": "2026-10-23T20:00:00Z",
  "paid": true,
  "unlimited": true,
  "websiteAccess": true,
  "mobileAccess": true,
  "sources": [
    {
      "provider": "website",
      "subscriptionType": "LifeTimePlatinum",
      "state": "active",
      "endsOn": null,
      "expiresAtUtc": null,
      "autoRenewEnabled": null,
      "productId": null,
      "nextProductId": null,
      "nextSubscriptionType": null,
      "changeEffectiveAtUtc": null,
      "verifiedAtUtc": null
    }
  ]
}
```


**Expired Apple, free fallback**

```json
{
  "asOfUtc": "2026-10-23T20:00:00Z",
  "paid": false,
  "unlimited": false,
  "websiteAccess": false,
  "mobileAccess": false,
  "sources": [
    {
      "provider": "website",
      "subscriptionType": "Free",
      "state": "active",
      "endsOn": null,
      "expiresAtUtc": null,
      "autoRenewEnabled": null,
      "productId": null,
      "nextProductId": null,
      "nextSubscriptionType": null,
      "changeEffectiveAtUtc": null,
      "verifiedAtUtc": null
    },
    {
      "provider": "apple",
      "subscriptionType": "MonthlyPlatinum",
      "state": "expired",
      "endsOn": null,
      "expiresAtUtc": "2026-10-20T18:00:00Z",
      "autoRenewEnabled": false,
      "productId": "example.platinum.monthly",
      "nextProductId": null,
      "nextSubscriptionType": null,
      "changeEffectiveAtUtc": null,
      "verifiedAtUtc": "2026-10-23T19:59:00Z"
    }
  ]
}
```


**Overlapping website and Apple**

```json
{
  "asOfUtc": "2026-10-23T20:00:00Z",
  "paid": true,
  "unlimited": true,
  "websiteAccess": true,
  "mobileAccess": true,
  "sources": [
    {
      "provider": "website",
      "subscriptionType": "AnnualPlatinum",
      "state": "active",
      "endsOn": "2027-02-28",
      "expiresAtUtc": null,
      "autoRenewEnabled": null,
      "productId": null,
      "nextProductId": null,
      "nextSubscriptionType": null,
      "changeEffectiveAtUtc": null,
      "verifiedAtUtc": null
    },
    {
      "provider": "apple",
      "subscriptionType": "MonthlyPlatinum",
      "state": "active",
      "endsOn": null,
      "expiresAtUtc": "2026-11-23T18:00:00Z",
      "autoRenewEnabled": true,
      "productId": "example.platinum.monthly",
      "nextProductId": null,
      "nextSubscriptionType": null,
      "changeEffectiveAtUtc": null,
      "verifiedAtUtc": "2026-10-23T19:59:00Z"
    }
  ]
}
```


### Revision 2: operation response schemas

Complete operation responses:

| Operation data | Required fields and constraints |
| --- | --- |
| Context | `appAccountToken`: UUID string; `environment`: `Sandbox` or `Production` from server configuration; `purchaseAllowed`: boolean; `purchaseBlockedReason`: null or one of the three documented reason codes; `canManageSubscription`, `canChangePlan`: booleans; `products`: Product[]; `entitlements`: Entitlements. |
| Product | `productId`: nonempty string; `subscriptionType`: mapped membership enum; `availableForPurchase`: boolean. Only recognised server mappings are returned; retired mappings remain with false. Sort by product ID for deterministic output. |
| Verify | `processed`: literal true; `transactionId`: the exact requested opaque string; `state`: Apple state enum above; `entitlements`: Entitlements. Processing success is not synonymous with paid access. |
| Refresh | `processed`: literal true; `complete`: boolean; `verifiedSubscriptionCount`, `pendingSubscriptionCount`: nonnegative integers for distinct chains in this invocation; `retryAfterSeconds`: null if complete, otherwise positive integer; `entitlements`: Entitlements after committed updates. A partial success requires at least one committed chain reconciliation. |

`canManageSubscription` is true when an owned Apple chain exists, including an expired/revoked chain, and directs the app to native Apple subscription management, never a caller-supplied URL. `canChangePlan` is true when an owned active/grace/retry chain has another saleable mapped product in the same group; Apple still determines actual eligibility and timing. These flags do not establish which App Store account is signed into the device, authorise account transfers or guarantee StoreKit will offer a change.

### Revision 2: deterministic account-link errors

Validate authentication, CSRF, HTTPS, syntax and supported verified app/product/environment first. Failure to obtain/verify provider evidence is 503 for a transient dependency failure or 400 for invalid signed evidence, never a token-recovery response. For valid supported evidence apply these rules in order:

| Verified evidence / stored association | Result |
| --- | --- |
| Chain is already linked to a different NineStar user, regardless of token presence | 403 `apple_account_mismatch`; never identify that user. |
| Unsupported ownership, including Family Sharing in this version | 403 `apple_account_mismatch`. |
| Token is present but malformed, unrecognised or differs from this user's server-issued token | 403 `apple_account_mismatch`, whether chain is linked or unbound. |
| Token is absent and chain is unbound | 409 `apple_account_link_required`; do not auto-link. |
| Token is absent and chain is linked to this user | Allow only previously approved, explicitly recorded tokenless-legacy recovery status; otherwise 403 `apple_account_mismatch`. No automatic legacy exception is created. |
| Token matches this user and chain is unbound or already linked to this user | Atomically link/reconcile, with database uniqueness enforcing the same result under races. |

The first release has no tokenless adoption endpoint. Support recovery is a separately reviewed process, not an app-side retry or token rewrite.

### Revision 2: unfinished purchase and account recovery

1. Obtain context while signed into the intended NineStar account before starting a new purchase. Pass its appAccountToken to StoreKit and associate pending local work with that token and configured environment; never store a password, auth cookie or reusable credentials inside a purchase queue.
2. Observe StoreKit transaction updates from app launch and enumerate unfinished transactions after login/restart. StoreKit's queue is the recovery source; local queue storage supplements it and must not be the only evidence that work remains.
3. Verify each supported transaction using the same transactionId. If a response is lost after server commit, the next same-owner call returns committed processing success without another benefit/grant.
4. On 401, stop automatic retries, retain unfinished work and request sign-in. After login obtain fresh CSRF and context; proceed only when the current account binding matches the pending purchase. A 400 `invalid_csrf_token` permits one token refresh and one same-ID retry for the same authenticated session/account; do not loop.
5. On logout/account switch, advance an auth-session generation, abort old client requests where possible and discard late responses from the old generation. Re-authenticate before resuming; a request already accepted by the server can still complete for its original account. Never merge that response into the new account, silently retry it as the new user, change the appAccountToken or transfer the chain.
6. On 403 mismatch or 409 recovery-required, stop automatic retries and show a localised sign-in/support action. Do not finish the transaction as delivered to the wrong account. Return to the original NineStar account or complete separately reviewed recovery. No other user's identity is returned.
7. On timeout/disconnect/429/503, keep the transaction unfinished and follow the retry budget below. A timeout means the commit result is unknown, not that the purchase failed. Do not start another StoreKit purchase to retry server verification.
8. Finish the matching StoreKit transaction only after a same-account `verify` response has `processed: true` and the echoed ID matches. Apply the returned entitlements before showing access. Successful processing of an expired/revoked transaction may finish without paid access. If StoreKit finish itself fails, re-enumerate/re-verify/retry finish; fulfilment remains idempotent. A context, GET or refresh response alone is not a transaction-completion acknowledgement.

Pending/Ask-to-Buy, cancelled purchase sheets and locally unverified transactions are client workflow states, not successful backend verification. Do not invent a transaction ID or finish an unsettled purchase merely to clear a spinner. A later verified StoreKit update can resume the flow.

### Revision 2: plan changes and retired products

New-subscription blocking does not block restore, verify, refresh or same-group subscription management. Monthly and annual products have the same content/service level in App Store Connect; their duration alone does not make annual an Apple upgrade. The app may offer **Manage Subscription / Change Plan** for an owned chain even while `purchaseAllowed` is false. Use Apple's native management flow for this first version, rather than opening another independent purchase flow.

Retain current benefits until Apple verifies the change is effective. Store verified pending product/renewal information in AppleSubscription and project the nullable next-product fields. A different-duration crossgrade normally takes effect at renewal; immediate changes follow Apple's actual signed transaction/status. Do not switch membership option, fabricate dates or revoke current access from the user's UI selection alone. Preserve the original chain owner; if Apple introduces another chain during a verified transition, apply the same token/unique-ownership rules and reconcile the resulting valid grants without stacking durations.

Keep recognised product mappings separate from sale availability. Retiring a product sets `availableForPurchase: false`; never delete its membership mapping or reject its legitimate renewal, notification, verify or restore solely because it is no longer sold. Context returns recognised retired IDs so the app's restore filter includes them even when StoreKit cannot return a saleable product listing. A verified product absent from the recognised catalogue returns `apple_product_not_supported`; resolve it through reviewed server catalogue configuration, never a client-selected membership mapping.

### Revision 2: deadlines, retries and partial refresh

Proposed initial timing contract, subject to real IIS/Apple tests before release:
- Keep the app's existing **15-second** HTTP timeout. Use a **12-second aggregate server action budget** for verify/refresh, including provider calls, certificate checks, coordination and commit work. Individual outbound calls have a maximum 5-second timeout further clipped to the remaining aggregate budget; no automatic provider retry inside a foreground action. Avoid session-lock waits that consume the whole budget. Target local context/entitlement reads within 3 seconds. These budgets are implementation targets, not a guarantee against network/proxy/IIS delays.
- Never declare `processed: true` before commit or assume cancellation can undo an in-progress commit. If the deadline is reached before a successful commit, return 503 `apple_verification_unavailable` where a response is still possible. If the connection is lost or commit outcome uncertain, the same-ID retry determines the durable result.
- Initial per-account rate limits: context 30/minute, verify 10/minute and refresh 6/minute, applied across workers; apply wider abuse controls separately. Coalesce identical in-flight verify requests for the same account/environment/transaction, without reusing one account's result for another. Return 429 `apple_rate_limited` with integer-seconds `Retry-After` (minimum 1, actual time until allowed); transient 503 responses include `Retry-After: 5`.
- On timeout/disconnect/503 use base delays of 2, 4, 8, 16, 30 and then 60 seconds, with up to 20% added jitter. Honour the longer valid Retry-After delay when supplied, including 429. Attempt at most three automatic retries during one foreground recovery cycle, then retain pending work for explicit retry or the next foreground/login/restart. Do not spin while offline or unauthenticated. Do not automatically retry permanent 400/403/409 errors, except the single CSRF refresh described above.
- Refresh commits each successfully verified chain independently; it is **not one all-or-nothing account-wide transaction**. If all known chains reconcile, return complete true, pendingSubscriptionCount 0 and retryAfterSeconds null. No known chains is a complete 200 result with both counts 0.
- If at least one chain commits but others fail or cannot be attempted within the budget, return 200 with complete false, the number committed in verifiedSubscriptionCount, remaining distinct chains in pendingSubscriptionCount, retryAfterSeconds 5 and the currently persisted combined entitlements. Counts describe this invocation, not cumulative lifetime progress. Mark unresolved chains for bounded background reconciliation using existing Hangfire; process fairly using persisted last-attempt times so one failing chain cannot starve others.
- If no chain commits and outstanding work remains, return 503 with the normal failure envelope; do not label stale data a successful refresh. The app may GET membership/entitlements for the last verified, time-bounded snapshot. Successful partial grants are never rolled back because another provider call failed, and a failure must never extend an unverified interval or erase an independent website grant.
- For a partial 200, the app honours retryAfterSeconds before another refresh, applying the same three-retry foreground cap. It can show currently verified access plus a localised refresh-pending status; it must not interpret complete false as a completed purchase acknowledgement.

### Revision 2: app/backend boundary

The app needs a development build with its IAP native module; Expo Go does not support that module. Keep explicit build/API/environment matching (Integration/Sandbox, Production/Production) and compare context.environment before purchases; changing a client setting cannot make one environment's evidence valid in another. The app maintains account-scoped entitlements separately from editable My Account form state: refresh only the entitlement/session state, preserving unsaved profile edits. Ignore stale or previous-account responses and clear paid UI state on logout. Record concrete app implementation/library notes in `ninestar-mobile/docs/`; this backend file owns only the shared contract.



### Server verification and ownership

For verify/restore, retrieve transaction information from Apple using the supplied ID, validate its signed transaction, then obtain current subscription status for the verified originalTransactionId. Verify signed renewal/transaction data, configured bundle ID and production app Apple ID, enabled environment, product allowlist/group, supported auto-renewable type, ownership and current expiry/grace/revocation state. Select the intended chain, not unrelated subscriptions included in Apple's response. A historical transaction's expiry alone cannot establish current access.

Validate JWS signatures, certificate chain to configured Apple trust roots and certificate policy/validity/revocation as required; never trust an embedded x5c root, an arbitrary client signing key, unverified decoded claims, or an unexpected algorithm. Keep provider API credentials/private signing keys on the server. Use fixed Apple API hosts, bounded timeouts and controlled retries; client input cannot select a URL. The concrete .NET-compatible verifier and its signed-data test fixtures are a pre-implementation technical gate.

Require the cryptographically verified appAccountToken to match this signed-in account's server-issued binding on initial link. Enforce immutable chain ownership with database uniqueness/transactions, not only application checks. On subsequent verification, renewals and restore, both chain ownership and token must remain consistent; fail closed/quarantine anomalies. Do not call Apple's Set App Account Token API to make a mismatched transaction fit. A UUID known by a caller is not independent proof of identity.

Tokenless purchases are not auto-linked by email, receipt possession or first caller. A previously linked tokenless legacy transaction may be reconciled only when a separately reviewed recovery process has explicitly recorded approved legacy status against the same immutable owner; a missing token does not create that status. Otherwise a linked missing-token transaction fails with 403. Initial tokenless adoption requires reviewed support recovery and is outside the first implementation. Family-shared transactions do not grant a second NineStar account access in this first version.

Production accepts only Production evidence. Integration accepts only Sandbox with isolated test users/database/configuration. A TestFlight sandbox purchase never grants production website membership. Do not fall back to Sandbox on arbitrary verification failure or mix environments. Xcode/local StoreKit test signatures are for isolated tests, never production entitlement proof.

### Notifications, renewal and recovery

Proposed server callback: POST `/payments/apple/notifications/v2` with `{ signedPayload }`, outside the mobile cookie/CSRF base. It is a server-to-server endpoint authenticated by verified Apple JWS, not a user session or shared public API key. Do not weaken CSRF on other routes. Verify the outer payload, nested signed payloads and configured app/environment before durable acceptance. The callback returns HTTP 200 after verified inbox commit (including an already-stored duplicate); invalid evidence returns 400, transient verification/storage failures return 503. Its provider acknowledgement is separate from the mobile JSON envelope.

Process the inbox with the existing job infrastructure. Atomically update transaction evidence, chain state, its membership grant and processing marker; use unique indexes, concurrency tokens and database coordination across workers. Repeated events and restore/verify races must not duplicate membership grants, free readings, mail or consultations. Provider calls should not hold long SQL locks. Use per-chain coordination/version checks and re-fetch on conflicts so an older concurrent response cannot overwrite a newer committed state.

Use verified current Apple status/transaction information to reconcile delayed/out-of-order events; never simply add a month/year, choose the largest historic expiry or treat receipt replay as a new purchase. A refund for an old period must not cancel an independently valid current renewal; a refund reversal can restore only the currently verified valid grant. Notification types are triggers for reconciliation, not unverified instructions to toggle an account.

| Verified state/event | Access result |
| --- | --- |
| Initial purchase / renewal / recovery | Upsert the chain grant to Apple's exact verified interval; renewals do not reset benefits/counters. |
| Auto-renew disabled / cancellation | Keep the already-paid interval until expiry; disabling future renewal is not immediate revocation. |
| Billing grace | Continue only through Apple's verified grace boundary when enabled and actually granted by Apple. |
| Billing retry without valid paid/grace interval | No Apple paid access; keep the chain for recovery and subscription management. |
| Expiry | Apple grant ends at the exclusive UTC boundary without depending on a job running. Other valid grants remain. |
| Refund / revocation | Reconcile and remove only access invalidated by Apple; retain audit, other valid periods and website grants. |
| Refund reversed | Reconcile current evidence; re-enable only if Apple now verifies a valid interval. |
| Unknown owner | Retain verified inbox evidence, do not create a user/grant, reconcile after a valid account link. |

Proposed operational target: process notifications within one minute under normal operation and reconcile active/grace/retry/recently expired chains every 15 minutes using Hangfire, with backoff/alerts and notification-history recovery for outages. These are design targets, not guarantees already achieved. Expiry is checked on every access; an unseen refund cannot be enforced before evidence arrives. On Apple outage retain only previously verified access through its known bound, never extend grace speculatively. Establish and measure stale-verification monitoring before release.

### Agreement, implementation and deployment gates

Agree this contract and the initial product mapping/overlap/grace/Family Sharing policies before the app adds purchasing. Subscription product setup and operational identifiers are still outstanding; no invented App Store IDs belong in code. This draft does not authorise live purchases or change product configuration.

After agreement: implement the verifier compatibility spike, additive DataAccess entities/model changes and owning service/controller updates; generate the **complete** EF6 migration/designer/resx from the real `LocalDb` using `DatabaseInitialiserLocal`. Do not create another DbContext, schema owner or migration stream. No migration is generated or executed at this proposal stage. Startup calls DbMigrator.Update: a deployment can apply schema changes, so a future migration must not be deployed implicitly with unrelated work. Review the migration/script and Integration backup/rollback plan and obtain the owner's deployment/schema instruction before applying it.

Required tests before release: existing website/free/weekly/monthly/annual/lifetime/promotion/consultation regression; shared app/web paid checks; exact UTC/grace boundaries; duplicate and simultaneous verify/restore/webhook; wrong account/token/product/bundle/environment/signature; original-chain ownership/deleted-account protection; cancellation/renewal/expiry/retry/refund/reversal; old-event/new-renewal ordering; provider outage/retry; concurrent website purchase preservation; admin provider-field protection; transaction/job recovery; sandbox notifications/real-device restore and both account screens. Verify no paid output is served from a stale cache after revocation. Do not claim production readiness from source checks alone.

Primary references checked for the proposal:
- [Apple appAccountToken](https://developer.apple.com/documentation/appstoreserverapi/appaccounttoken)
- [App Store Server API](https://developer.apple.com/documentation/appstoreserverapi)
- [Get All Subscription Statuses](https://developer.apple.com/documentation/appstoreserverapi/get-all-subscription-statuses)
- [Receiving Server Notifications V2](https://developer.apple.com/documentation/appstoreservernotifications/receiving-app-store-server-notifications)
- [Notification types](https://developer.apple.com/documentation/appstoreservernotifications/notificationtype)
- [Apple server library and signed-data verification examples](https://github.com/apple/app-store-server-library-node)

### Health panel parity

Full chart projections now add `health.strongOrgans` and `health.weakOrgans`. Each panel contains `title`, `htmlBody`, `yinOrgans` and `yangOrgans`. Titles and introductions use the same Dictionary properties as the website Health panels. Each organ entry contains its stable enum `value`, Dictionary-backed `label` (resolved through the same EnumDescriptionAttribute as the website Organ template), and catalogue `artworkId`. Missing organ collections are empty arrays. Existing top-level organ identifiers and artwork fields remain unchanged for compatibility.

Full energy projections also add `yinYangExpansionTitle`, taken from the existing calculated energy resource property, alongside `yinYangExpansionDescription`. To reproduce Health, use `sections.health` for the reading, `sections.illnesses` for common challenges, `energies.core.yinYangExpansionTitle` and `yinYangExpansionDescription` for the group panel, and `health.strongOrgans` / `health.weakOrgans` for the final two panels. Do not turn enum values into display labels. These additions stay within full, already authorised chart projections (including embedded profiles); the free overview and anonymous number-only projections are unchanged.

Regression coverage checks resource parity, calculated organ selection/order, display labels/artwork and exclusion from the free projection. Windows build/xUnit and signed-in Integration validation remain required; no deployment is performed by this change.

### Character navigation section titles

`MobileChartText` adds an optional `title` field alongside the existing `body` and `summary`. It is reusable by other reading sections, but this change populates only Character navigation and its existing stress subreadings. Unmapped sections may have `title: null`; clients must not invent wording by converting machine keys into labels.

These are navigation labels from `Views/PersonalChart/_ProfileCharacterEnergyDetails.cshtml`, not substitutes for explanatory `help` titles or longer reading-panel headings:

| JSON path under `data` | Website source | English value / meaning |
| --- | --- | --- |
| `sections.character.title` | `model.CharacterEnergy.DescriptiveTitle` | Existing energy-specific navigation label, e.g. `The Pioneer`; reuses the website formatter and Dictionary-backed descriptive-name metadata |
| `sections.childNature.title` | `Dictionary.ChildNature` | `Child Nature` |
| `sections.adultChildRelationship.title` | `Dictionary.AdultChildRelationshipTitle` | `Adult / Child Relationship` |
| `sections.stressResponse.title` | `Dictionary.StressResponse` | `Stress Response` |
| `sections.natalHouseStressResponse.title` | `Dictionary.StressResponse` | Part of the same Stress Response panel |
| `sections.childStressResponse.title` | `Dictionary.StressResponse` | Part of the same Stress Response panel |

`data.help.character.title` uses `Dictionary.AboutCharacterNature` (the generic Character introduction); `sections.character.title` remains the short energy-specific navigation label. The full energy reading heading remains `data.energies.character.title` (`EnergyTitle`). Neither should replace the short navigation label. No new `help` entries or prose are invented. The website combines `stressResponse.body`, `natalHouseStressResponse.body` and `childStressResponse.body`, in that order, under one Stress Response heading; do not render three navigation tabs.

Example response fragment (reading HTML abbreviated):

```json
{
  "data": {
    "sections": {
      "character": { "title": "The Pioneer", "body": "<p>...</p>", "summary": "<p>...</p>" },
      "childNature": { "title": "Child Nature", "body": "<p>...</p>", "summary": null },
      "adultChildRelationship": { "title": "Adult / Child Relationship", "body": "<p>...</p>", "summary": null },
      "stressResponse": { "title": "Stress Response", "body": "<p>...</p>", "summary": null },
      "natalHouseStressResponse": { "title": "Stress Response", "body": "<p>...</p>", "summary": null },
      "childStressResponse": { "title": "Stress Response", "body": "<p>...</p>", "summary": null }
    }
  }
}
```

Full authorised chart projections, including embedded profiles when `help` is suppressed, receive these titles. The free projection adds only the title of its already exposed `sections.character`; it does not add the paid Child Nature, Adult / Child Relationship or Stress Response sections. Anonymous previews and number-only responses are unchanged. Existing bodies, summaries, authentication, membership, age gates and complementary-reading rules are unchanged. Purchasing is unaffected. Older servers may omit `title`, so clients must treat it as optional.

Regression coverage checks website/resource parity, camel-case JSON fields, unchanged reading bodies/summaries and help, embedded titles, and free projection boundaries. Build/xUnit execution requires Windows/VS2017; source validation is not evidence of deployment. After an owner-authorised Integration deployment, verify the authenticated full `/api/mobile/v1/personal-chart/me` response for the exact fields above. An unauthenticated response cannot establish whether these private fields are live.

### Adult / Child Relationship headings

The website helper now uses `Dictionary.AboutAdultChildRelationship` (`About Adult / Child Relationship`) while its details panel and Character navigation use the existing `Dictionary.AdultChildRelationshipTitle` (`Adult / Child Relationship`). The pattern-specific `AdultChildRelationsihpTitle` is no longer used as that website panel heading; its calculation/property and reading body are unchanged.

The full standalone API chart mirrors that distinction:

```json
{
  "data": {
    "help": {
      "adultChildRelationship": {
        "title": "About Adult / Child Relationship",
        "htmlBody": "<p>...</p>",
        "culture": "en-GB"
      }
    },
    "sections": {
      "adultChildRelationship": {
        "title": "Adult / Child Relationship",
        "body": "<p>...</p>",
        "summary": null
      }
    }
  }
}
```

Mobile clients should use `help.adultChildRelationship.title` for the expandable explanation and `sections.adultChildRelationship.title` for its navigation/details heading. Help/body text and member-access rules are unchanged; free responses still omit this member helper and embedded profiles still suppress duplicated help.

### Contextual About headings

Website explanatory helper panels and matching API `help` titles use dedicated Dictionary resources, so navigation/section titles and reading headings remain unchanged. Knowledge-base article headings, individual energy/cycle readings, The Nine Yearly/Monthly Cycles collections and Detailed Interpretation keep their existing labels. More On Predictions (`Dictionary.MoreOnCycles`) and What Is Karmic Numerology? (`Dictionary.WhatIsKarmicNumerology`) are explicitly unchanged, as are the existing About Adult / Child Relationship, About 9 Star Ki Biorhythms and About The I Ching labels.

| Response/help key | Dictionary resource | English title |
| --- | --- | --- |
| chart overview | `Dictionary.AboutPersonalChart` | About Your Personal Chart |
| chart core | `Dictionary.AboutConstitution` | About Your Constitution |
| chart character | `Dictionary.AboutCharacterNature` | About Character Nature / The Inner Child |
| chart socialExpression | `Dictionary.AboutSocialExpression` | About Social Expression |
| chart dayStar | `Dictionary.AboutDayStar` | About The Day Star |
| chart generation | `Dictionary.AboutGenerationalKi` | About Generational Ki |
| chart epoch | `Dictionary.AboutEpochKi` | About Epoch Ki |
| chart relationships | `Dictionary.AboutRelationshipsAndIntimacy` | About Relationships & Intimacy |
| predictions day | `Dictionary.AboutDailyPredictions` | About Daily Predictions |
| predictions month | `Dictionary.AboutMonthlyPredictions` | About Monthly Predictions |
| predictions year | `Dictionary.AboutYearlyPredictions` | About Yearly Predictions |
| predictions generation | `Dictionary.AboutNineYearPredictions` | About 9-Year Predictions |
| predictions epoch | `Dictionary.AboutEightyOneYearPredictions` | About 81-Year Predictions |
| predictions globalKiWarning | `Dictionary.AboutGlobalKi` | About Global Ki |
| calendar overview/subscription | `Dictionary.AboutPersonalCalendar` | About Your Personal Calendar |
| compatibility overview | `Dictionary.AboutCompatibilityCalculator` | About The Compatibility Calculator |

The website 81-year helper retains `(Global Ki)` after its About heading. API prediction help retains its existing separate `epoch` and `globalKiWarning` entries. Only titles change: help HTML, section bodies/summaries, memberships and age/access restrictions remain unchanged.

### Short energy role labels

Full and free personal-chart responses add `data.roleLabels`, also retained on full embedded profiles when help is omitted. Use the same role label for each role card and its destination screen. This is separate from `help.<role>.title` (About explanations), `energies.<role>.title` (the full energy heading) and `sections.<key>.title` (reading/navigation subsection headings). Stable role keys remain untranslated; display values come directly from Dictionary on each response. No resources, access rules or paid readings are added to free responses.

| JSON field | Dictionary source |
| --- | --- |
| `roleLabels.core` | `Dictionary.MainEnergyLabel` |
| `roleLabels.character` | `Dictionary.CharacterEnergyLabel` |
| `roleLabels.socialExpression` | `Dictionary.SurfaceEnergyLabel` |
| `roleLabels.dayStar` | `Dictionary.DayStarLabel` |
| `roleLabels.generation` | `Dictionary.GenerationalEnergyLabel` |
| `roleLabels.epoch` | `Dictionary.EpochEnergyLabel` |

```json
{
  "roleLabels": {
    "core": "Constitution",
    "character": "Character Nature / Inner Child",
    "socialExpression": "Social Expression",
    "dayStar": "Day Star",
    "generation": "Generational Ki",
    "epoch": "Epoch Ki"
  }
}
```

These are six stable role labels, not age-dependent website navigation substitutions. Hourly energy retains its existing contract; no seventh label is invented. Older servers may omit `roleLabels`; clients must handle absence without deriving invented display text from role keys. Source/syntax checks do not establish live deployment; Windows build/xUnit and an authenticated Integration response remain release validation steps.

### Website display-string audit

The public Personal Chart calculator introduction now uses `Dictionary.AboutPersonalChart`, matching `help.overview.title`. Compatibility explanatory panel headings reuse `RelationshipsTitle`, `FiveTransformationsTitle` and `SexualityTitle` rather than literal strings. No API payload or memberships change in this patch.

A targeted audit of the calculator/helper views also found older hardcoded strings in `Views/Biorhythms/_BioRhythms.cshtml` (chart tooltip Date/Value/Description) and `Views/Numerology/Legacy/_NumerologyYearlyPlanner*.cshtml` (Age, Year, numerology-code headings and Roadmap/Foundation/Guide/Ascension/Elders). These remain recorded follow-ups; this is not a claim that the entire site is free of historic hardcoded display text. Do not change reading prose, structural HTML, CSS classes, route identifiers or chart format tokens as part of localisation.


## Predictions short period and scope labels

Full Predictions (`predictions/calculate`, `predictions/me`, `predictions/last`, and full saved-reading projections) and free/complementary Predictions (`readings/predictions/me/free`, `readings/predictions`, and their existing saved-reading projections) include `data.labels`. These are navigation labels, separate from longer `help` titles and energy-specific reading titles. Clients must use the supplied strings rather than translating enum keys or removing “About” from help titles.

| Field | Dictionary resource | English value |
| --- | --- | --- |
| `labels.periods.year` | `Year` | Year |
| `labels.periods.month` | `Month` | Month |
| `labels.periods.day` | `Day` | Day |
| `labels.periods.generation` | `Generation` | Generation |
| `labels.periods.epoch` | `Epoch` | Epoch |
| `labels.periods.hour` | `Hour` | Hour |
| `labels.scopes.personal` | `PersonalView` | Personal Houses |
| `labels.scopes.lunar` | `PersonalLunarView` | Personal Lunar View |
| `labels.scopes.global` | `GlobalView` | Global Ki |

Scope resources match the existing `EPanelCycleView` display labels. Website form controls also have distinct instructional labels such as `PersonalHouses` (“Display Houses Occupied”); these are not short navigation labels. `Epoch` and `Hour` are new Dictionary resources; all other resources already exist. No display wording is derived from transport keys.

Example fragment, identical in full and free responses:

```json
{
  "success": true,
  "data": {
    "labels": {
      "periods": { "year": "Year", "month": "Month", "day": "Day", "generation": "Generation", "epoch": "Epoch", "hour": "Hour" },
      "scopes": { "personal": "Personal Houses", "lunar": "Personal Lunar View", "global": "Global Ki" }
    }
  },
  "error": null
}
```

This fragment omits the existing reading fields. Free responses still expose only their existing yearly readings (`year`, `lunarYear`, `globalYear`); the presence of a month/day/hour label does not grant that reading. Full responses retain their existing access checks and reading structure. `globalYear.description` now explicitly selects `GlobalCycleDescription`, matching full Predictions global prose; personal and lunar yearly prose remain unchanged.

### Integration error verification and indexing

Public GET checks during investigation returned JSON: `metadata/options` 200, `predictions/me` 401, and `readings/predictions/me/free` 401. These checks used no signed-in cookies and do not establish that authenticated calculation or an earlier timeout is resolved. Do not confuse HEAD routing responses with GET behaviour. If signed-in failures persist, capture the timestamp, route, status and server exception from Integration logs, and compare a non-calculation endpoint to distinguish site/startup/SQL availability from calculation failure. Do not post cookies or birth details in diagnostic reports.

The shared website exception filter now leaves mobile paths to the existing controller/pipeline JSON handlers instead of selecting FriendlyError HTML. This does not repair IIS/startup failures before the application runs or eliminate server timeouts.

Integration was observed already sending `X-Robots-Tag: noindex, nofollow`. Persist this only in the Integration web.config transform, under `system.webServer/httpProtocol/customHeaders`, using XDT `SetAttributes` matched by name for an existing header, or `Insert` if absent (with `InsertIfMissing` for absent parent elements). Do not add it to shared or Production configuration. The repository transform is git-crypt protected and requires the owner’s plaintext upload for editing. Keep crawling enabled so Google can read noindex; temporary Search Console prefix removal can accelerate removal of existing Integration results.


## Predictions cycle cards matching the website

Full and free Predictions also include these fields under `data.labels`. These are the website’s exact card/panel resources, distinct from short `labels.periods` and explanatory `help` titles.

| Field | Dictionary resource | English value |
| --- | --- | --- |
| `labels.groups.primary` | `PrimaryCycles` | Primary Cycles |
| `labels.groups.additional` | `ExtendedCycles` | Additional Cycles |
| `labels.groups.global` | `GlobalCycles` | Global Cycles |
| `labels.groups.extendedGlobal` | `ExtendedGlobalCycles` | Extended Global Cycles |
| `labels.cycleTitles.year` | `YearlyCycle` | Yearly Ki |
| `labels.cycleTitles.month` | `MonthlyCycle` | Monthly Ki |
| `labels.cycleTitles.day` | `DailyCycleEnergy` | Daily Ki |
| `labels.cycleTitles.generation` | `NineYearlyEnergy` | Nine-Yearly Ki |
| `labels.cycleTitles.epoch` | `EightyOneYearlyEnergy` | 81-Yearly Ki |
| `labels.cycleTitles.hour` | `HourlyEnergy` | Hourly Ki |

Each non-null full cycle projection (`personal`, `lunar`, `global`, and existing planner/knowledge-base cycle projections) now includes `houseName`, taken directly from `NineStarKiEnergy.HouseName`. This is the same formatter used by the website: `ElementNameAndNumber` plus `Dictionary.House`, for example “7 Metal House”. Free yearly projections (`year`, `lunarYear`, `globalYear`) include the same field. No local string concatenation or hardcoded “House” is needed in the app. Null readings stay null, and free responses gain no additional readings.

Render the screenshot layout in this order:

1. Group heading: `data.labels.groups.primary` (year/month/day), or `.additional` (generation/epoch/hour).
2. Card heading: corresponding `data.labels.cycleTitles` value.
3. House line: reading’s `houseName`.
4. Existing energy image/artwork.
5. For year/month/day, existing `cycleDescriptiveName` beneath the image in full responses, or `title` for the free year. Preserve the existing text, including “Harvest / Abundance and Celebration”; do not replace it with the house line or natal energy title.

Example full-response fragment:

```json
{
  "labels": {
    "groups": { "primary": "Primary Cycles", "additional": "Additional Cycles" },
    "cycleTitles": { "year": "Yearly Ki", "month": "Monthly Ki", "day": "Daily Ki", "generation": "Nine-Yearly Ki", "epoch": "81-Yearly Ki", "hour": "Hourly Ki" }
  },
  "personal": {
    "year": {
      "houseName": "7 Metal House",
      "cycleDescriptiveName": "Harvest / Abundance and Celebration"
    }
  }
}
```

This fragment omits unchanged fields. The cycle description remains personalised reading content. Existing artwork availability, scopes, membership checks and complementary counters are unchanged. No new Dictionary wording is required for these card labels.


## Predictions and Global Ki introductions

Both full Predictions and free/complementary Predictions expose `data.help.overview` and `data.help.globalKi`, including existing saved-reading projections. Each has `title`, `htmlBody` and `culture`, following the standard help contract.

| Field | Title resource | Body resource | Purpose |
| --- | --- | --- | --- |
| `help.overview` | `PredictionsOverview` | `PredictionsIntroduction` | New two-paragraph general introduction for the top of the Predictions screen |
| `help.globalKi` | `AboutGlobalKi` | `GlobalKiIntroduction` | Dedicated explanation of collective/planetary Ki versus individual personal houses |

Render the supplied `htmlBody` using the existing safe HTML renderer. Do not hardcode or paraphrase either text. `help.moreOnCycles` is separate supplementary material, and `help.globalKiWarning` is only the existing Epoch-specific warning. Neither is the general introduction. Full help retains all existing entries. Free help adds only the two public introductions, not paid reading content, other help entries or additional cycle readings. Existing memberships, counters and ownership checks remain unchanged. The public introductory text describes available concepts without granting access to every period.

Example response fragment (bodies abbreviated here only):

```json
{
  "help": {
    "overview": { "title": "Predictions Overview", "htmlBody": "<p>Nine Star Ki predictions explore...</p><p>Use these readings...</p>", "culture": "en-GB" },
    "globalKi": { "title": "About Global Ki", "htmlBody": "<p>Global Ki describes the changing energy that influences the collective and the planet as a whole...</p><p>Your personal house describes...</p>", "culture": "en-GB" }
  }
}
```

Dictionary/resource XML and C# syntax can be checked here; Windows build/xUnit and authenticated Integration verification remain required. Resource changes require rebuilding the Globalisation assembly before deployment.


## Predictions detail-screen headings

Full and free Predictions expose `data.labels.predictionTitles` through the shared labels mapper, including existing saved-reading projections. These are the existing Dictionary prediction titles for detail-screen headings. Keep them separate from short `labels.periods`, card `labels.cycleTitles` and explanatory `help` titles.

| Field | Dictionary resource | English value |
| --- | --- | --- |
| `labels.predictionTitles.year` | `YearlyPrediction` | Yearly Prediction |
| `labels.predictionTitles.month` | `MonthlyPrediction` | Monthly Prediction |
| `labels.predictionTitles.day` | `DailyPrediction` | Daily Prediction |
| `labels.predictionTitles.generation` | `NineYearlyPrediction` | 9-Year Prediction |
| `labels.predictionTitles.epoch` | `EightyOneYearlyPrediction` | 81-Year Prediction |
| `labels.predictionTitles.hour` | `HourlyPrediction` | Hourly Prediction |

Example fragment:

```json
{
  "labels": {
    "predictionTitles": {
      "year": "Yearly Prediction",
      "month": "Monthly Prediction",
      "day": "Daily Prediction",
      "generation": "9-Year Prediction",
      "epoch": "81-Year Prediction",
      "hour": "Hourly Prediction"
    }
  }
}
```

The mobile app should select the heading centrally by period key and use the returned string verbatim. Do not derive it from “Year”, append “Prediction” locally, or substitute a help title. All six labels are public metadata; their presence does not grant access to readings. Existing full/free reading contents, access checks and complementary accounting remain unchanged. No new Dictionary wording is added.


`help.globalKi.htmlBody` now uses the dedicated two-paragraph `Dictionary.GlobalKiIntroduction`, explaining the collective/planetary energetic climate and how it differs from the individual personal house. It replaces the previously exposed membership-page description at the same API field; clients need no field-name change. `Dictionary.GlobalKiDescription` remains the original membership-page promotional text and is unchanged. The new resource lives in `webapp/Globalisation/Dictionary.resx`, with its accessor in `Dictionary.Designer.cs`, and is selected by `MobileHelpMapper.PredictionIntroductions()`. Both full and free Predictions receive it. It is distinct from the Epoch-only `help.globalKiWarning`; no reading access changes.


## Native personal calendar presentation

`GET /api/mobile/v1/calendar/month?year=2026&month=10` now adds presentation metadata to the existing month response. The route, optional year/month behaviour, Monday-first `offset`, member-timezone `today`, numeric houses, summary and full website description are preserved. Access still requires the existing paid predictions membership or administrator check. No new calculation, subscription, purchasing or persistence behaviour is introduced.

`data.labels` contains the website calendar resources:

| Field under `labels` | Dictionary resource |
| --- | --- |
| `title` | `PersonalCalendar` |
| `previousMonth`, `nextMonth`, `today` | `PreviousMonth`, `NextMonth`, `Today` |
| `help` | `CalendarHelp` |
| `googleCalendarLink` | `GoogleCalendarLink` (Google Calendar Link) |
| `copyCalendarLink`, `disableCalendarLink` | `CopyCalendarLink`, `DisableCalendarLink` |
| `copied`, `copyError` | `Copied`, `FailedToCopyToClipboard` |
| `expand`, `close` | `ExpandCalendar`, `CloseCalendar` |
| `revoked` | `CalendarLinkDisabled` |
| `error`, `unavailable` | `NetworkError`, `PersonalCalendarUnavailable` |
| `myAccount` | `K9.Base.Globalisation.Dictionary.MyAccountTitle` |
| `year`, `month`, `day` | `Year`, `Month`, `Day` |

All resources other than the explicitly qualified My Account title use `K9.Globalisation.Dictionary`. `labels.weekdays` is a Monday-to-Sunday array with `{ dayOfWeek, label, shortLabel }`. `dayOfWeek` uses ISO numbering 1 (Monday) to 7 (Sunday), not .NET/JavaScript Sunday-zero numbering. `label` comes from `Monday` through `Sunday`; `shortLabel` uses the first up-to-three characters, matching the website. Use these values for headings and accessibility labels rather than hardcoded English or device-generated abbreviations. Existing `data.help.overview` and `.subscription` retain their Dictionary-backed title/HTML/culture fields.

Each `data.entries[]` adds:

| Field | Meaning |
| --- | --- |
| `dateLabel` | Long date formatted with request `CurrentCulture`, matching website `DateLabel`; use ISO `date` for identity/navigation |
| `day` | Gregorian day number |
| `houses` | Website summary: Dictionary Year/Month/Day labels and number plus resource-backed energy name, separated by ` · ` |
| `energyName` | Daily number plus existing resource-backed energy name, matching the website |
| `yearHouseLabel`, `monthHouseLabel`, `dayHouseLabel` | The corresponding number plus energy name, using the website formatter convention |
| `afternoonDayHouseLabel` | Same format for the optional afternoon energy; null if absent |
| `artworkId` | Existing registry ID for the daily energy image |
| `yearArtworkId`, `monthArtworkId`, `afternoonArtworkId` | Existing registry IDs for those energies; afternoon is null if absent |

House labels intentionally use the website’s energy-name convention (e.g. the enum energy’s descriptive name), rather than the separate Predictions card `houseName` element/House format. Resolve IDs through the existing `/api/mobile/v1/metadata/artwork` catalogue and image/cache pipeline; do not construct asset URLs or interpret an ID as a URL. All images use the same `ninestar/energies/<energy>.png` assets as the website. Existing `yearHouse`, `monthHouse`, `dayHouse`, `afternoonDayHouse`, `summary` and `description` remain untouched. Missing afternoon values must not fabricate an afternoon reading or image.

Example abbreviated response structure:

```json
{
  "success": true,
  "data": {
    "year": 2026, "month": 10, "title": "October 2026",
    "today": "2026-10-26", "offset": 3,
    "labels": {
      "today": "Today",
      "weekdays": [{ "dayOfWeek": 1, "label": "Monday", "shortLabel": "Mon" }]
    },
    "entries": [{
      "date": "2026-10-01", "dateLabel": "01 October 2026", "day": 1,
      "yearHouse": 7, "monthHouse": 7, "dayHouse": 7,
      "afternoonDayHouse": null, "afternoonDayHouseLabel": null, "afternoonArtworkId": null
    }]
  }
}
```

This example omits the other six weekday objects, other labels, artwork IDs and unchanged reading text; use the actual returned long-date formatting and catalogue IDs. Presentation mapping is in `MobileCalculationMapper.CalendarLabels()` and `.CalendarDay()`; `MobileCalendarController.Month` continues to call the existing cancellation-aware calendar service once. Syntax/contract source checks do not replace a Windows build/xUnit run or authenticated Integration test.


The calendar subscription panel heading is `data.labels.googleCalendarLink` (Dictionary.GoogleCalendarLink: “Google Calendar Link”), returned by `GET /api/mobile/v1/calendar/month`. Use `data.help.subscription.htmlBody` for its existing instructions and `data.labels.copyCalendarLink` for the copy button. Keep the explanatory `AboutPersonalCalendar` help title separate from this link-panel heading. Existing subscription/revocation responses, permissions and private link handling are unchanged.


## Predictions screen and current-house headings

Full and free Predictions expose two additional strings through their shared labels projection (including existing saved-reading projections):

| Field | Dictionary resource | English value |
| --- | --- | --- |
| `data.labels.yourPredictions` | `YourPredictions` (new) | Your Predictions |
| `data.labels.housesYouCurrentlyOccupy` | `HousesYouCurrentlyOccupy` (existing website resource) | Houses You Currently Occupy |

Use `yourPredictions` for the screen heading and `housesYouCurrentlyOccupy` for the personal current-house section heading. Keep period detail headings (`predictionTitles`), card headings (`cycleTitles`), group headings (`groups`) and help titles separate. Both strings are display metadata and do not expand access to readings or change membership rules. Example fragment: `{ "labels": { "yourPredictions": "Your Predictions", "housesYouCurrentlyOccupy": "Houses You Currently Occupy" } }`. Build Globalisation and the website before deployment; native clients should use the supplied values rather than hardcoding them.


## Compatibility presentation and gauges

The existing paid `POST /api/mobile/v1/compatibility/calculate`, `GET /api/mobile/v1/compatibility/last`, and existing complementary `readings/compatibility` projections now add labels, gauges and a short Strengths and Challenges subtitle. The existing reading/score fields remain unchanged. Access checks, ownership, complementary-credit consumption and age/user-setting sexual-chemistry restrictions are unchanged; no new free endpoint is added.

### Gauges

`data.gauges` is ordered as on the website. Each item has `key`, Dictionary-backed `label`, numeric `value`, `minValue` (0), `maxValue` and `isInverted`. Values are enum ordinals, not percentages; use `(value - minValue) / (maxValue - minValue)` for needle position. Standard scores span 0-9 (`ECompatibilityScore.ExtremelyHigh`); Sexual Chemistry spans 0-15 (`ESexualChemistryScore.OffTheCharts`). `isInverted` reverses the colour desirability only: a high conflict score remains a high needle value and should be red, not green. Other gauges run red through yellow to green as their value increases. Use a native gauge, not an image of the website canvas.

| Gauge key | Dictionary resource | Maximum | Inverted |
| --- | --- | --- | --- |
| complementarity | ComplementarityScore | 9 | false |
| conflict | ConflictScore | 9 | true |
| harmony | HarmonyScore | 9 | false |
| spark | SparkScore | 9 | false |
| sexualChemistry | SexualChemistry | 15 | false |
| learningPotential | LearningPotentialScore | 9 | false |
| mutualUnderstanding | MutualUnderstanding | 9 | false |
| mutualSupport | MutualSupport | 9 | false |

When chemistry is hidden by request or either person is under 16, the sexualChemistry gauge and its section label are omitted. The other seven gauges retain their order, and existing sexualChemistryDetails/score hiding remains. If the calculated score is absent, gauges is null. Do not invent zero-valued gauges for absent data. Existing `details.score.*` enum-name fields remain available; the new numeric gauge projection avoids parsing enum names in mobile.

### Section labels and Yin/Yang reading

`data.labels.title`, `results`, `firstPerson`, `secondPerson` map to Dictionary.RelationshipCompatibilityCalculator, CompatibilityResults, FirstPerson and SecondPerson. `data.labels.sections.<key>` has `title` (panel heading) and `navigationTitle` (the shorter website mobile-tab label where different):

| Section key | Title resource | Navigation resource |
| --- | --- | --- |
| overview | CompatibilityOverview | OverviewLabel |
| strengthsAndChallenges | StrengthsWeaknesses | GiftsTrials |
| inDepth | InDepthAnalysis | InDepthAnalysis |
| solarElements | FundamentalElementsAnalysis | SolarElements |
| lunarElements | CharacterElementsAnalysis | LunarElements |
| allElements | AllElementsBreakdown | AllElements |
| yinYang | YinAndYangAnalysis | YinYangLabel |
| birthMonth | BirthMonthAnalysis | BirthMonth |
| sexualChemistry | SexualChemistryDetails | SexualChemistryDetails |

Score headings are supplied directly as `gauges[].label`. They are also exposed centrally under `data.labels.scores`, keyed to the existing score fields: `complementarityScore`, `conflictScore`, `harmonyScore`, `sparkScore`, `learningPotentialScore`, `mutualUnderstandingScore`, `supportScore`, and (only when visible) `sexualChemistryScore`. These use the same resources as the gauge table above. Keep these labels separate from generic `help.overview` explanations and from the personalised reading subtitles.

`data.details.gender.description` adds the website's selected Yin/Yang HTML explanation alongside the unchanged flags. It mirrors `_CompatibilityYinYang.cshtml`: both matching energies select `both_gender_same_yin` or `both_gender_same_yang` based on fundamental polarity; otherwise combine the appropriate `main_gender_same_yin`, `main_gender_same_yang` or `main_gender_opposite` with `character_gender_same_yin`, `character_gender_same_yang` or `character_gender_opposite`. Use the exact existing selection, including the both-matching branch, rather than inferring prose on mobile. The existing TemplateParser resolves SameTransformationLink, SupportiveTransformationLink and ChallengingTransformationLink to the website knowledge-base URL generated by MVC with its matching anchor. Render this as server-authored HTML using the app's existing safe renderer and open those links appropriately. No new Yin/Yang prose was written.

### Strengths and Challenges subtitles

The original opening heading comes from Dictionary.main_11 through main_99, backed by `Globalisation/Compatibility/en-gb/main-XY.htm`, selected by `CompatibilityModel.GetFundamentalEnergiesCompatibilityDetails()` and personalised with TemplateParser. There are 45 unique unordered constitution-energy pairs; swapping the two people uses the same reading family.

`data.sections.strengthsAndChallenges` adds `{ title, subtitle, body }`. `title` is Dictionary.StrengthsWeaknesses; `body` is exactly the existing `fundamentalEnergiesCompatibility` reading, which is also retained at its original field. `subtitle` is an independently localisable short label derived from that reading's opening heading, not dynamically scraped/truncated HTML. It uses new Dictionary.CompatibilitySubtitleXY resources, where X is the smaller fundamental energy number and Y the larger. The app must use the returned string verbatim, not derive it from the pair, score or long opening text.

| Pair | Resource | Subtitle |
| --- | --- | --- |
| 11 | CompatibilitySubtitle11 | Deep Emotional Harmony |
| 12 | CompatibilitySubtitle12 | Potentially Challenging |
| 13 | CompatibilitySubtitle13 | Vibrant And Harmonious |
| 14 | CompatibilitySubtitle14 | Vibrant And Harmonious |
| 15 | CompatibilitySubtitle15 | Complex With Strong Chemistry |
| 16 | CompatibilitySubtitle16 | Harmonious And Complementary |
| 17 | CompatibilitySubtitle17 | Harmonious And Uplifting |
| 18 | CompatibilitySubtitle18 | Potentially Very Challenging |
| 19 | CompatibilitySubtitle19 | Potentially Very Challenging |
| 22 | CompatibilitySubtitle22 | Deep Harmony And Friendship |
| 23 | CompatibilitySubtitle23 | Challenging With Growth Potential |
| 24 | CompatibilitySubtitle24 | Potentially Challenging |
| 25 | CompatibilitySubtitle25 | Harmonious And Supportive |
| 26 | CompatibilitySubtitle26 | Deeply Complementary And Harmonious |
| 27 | CompatibilitySubtitle27 | Naturally Harmonious And Complementary |
| 28 | CompatibilitySubtitle28 | Harmony And Mutual Support |
| 29 | CompatibilitySubtitle29 | Harmony And Mutual Understanding |
| 33 | CompatibilitySubtitle33 | Energetic And Harmonious |
| 34 | CompatibilitySubtitle34 | Harmonious And Creative |
| 35 | CompatibilitySubtitle35 | Challenging With Strong Chemistry |
| 36 | CompatibilitySubtitle36 | Challenging With Mutual Learning |
| 37 | CompatibilitySubtitle37 | Challenging With Growth Potential |
| 38 | CompatibilitySubtitle38 | Challenging And Transformative |
| 39 | CompatibilitySubtitle39 | Harmonious And Vibrant |
| 44 | CompatibilitySubtitle44 | Gentle And Harmonious |
| 45 | CompatibilitySubtitle45 | Contrasting Rhythms |
| 46 | CompatibilitySubtitle46 | Challenging With Growth Potential |
| 47 | CompatibilitySubtitle47 | Challenging With Strong Chemistry |
| 48 | CompatibilitySubtitle48 | Challenging With Growth Potential |
| 49 | CompatibilitySubtitle49 | Harmonious And Stimulating |
| 55 | CompatibilitySubtitle55 | Harmony And Stability |
| 56 | CompatibilitySubtitle56 | Harmonious And Complementary |
| 57 | CompatibilitySubtitle57 | Highly Harmonious And Complementary |
| 58 | CompatibilitySubtitle58 | Harmony And Lasting Friendship |
| 59 | CompatibilitySubtitle59 | Highly Harmonious And Complementary |
| 66 | CompatibilitySubtitle66 | Harmony With Leadership Challenges |
| 67 | CompatibilitySubtitle67 | Harmony And Mutual Respect |
| 68 | CompatibilitySubtitle68 | Harmonious And Stable |
| 69 | CompatibilitySubtitle69 | Challenging With Strong Chemistry |
| 77 | CompatibilitySubtitle77 | Harmony And Friendship |
| 78 | CompatibilitySubtitle78 | Harmony And Growth |
| 79 | CompatibilitySubtitle79 | Challenging With Growth Potential |
| 88 | CompatibilitySubtitle88 | Harmony And Friendship |
| 89 | CompatibilitySubtitle89 | Harmonious And Complementary |
| 99 | CompatibilitySubtitle99 | Harmony And Friendship |

Example abbreviated response fragment:

```json
{
  "gauges": [
    { "key": "conflict", "label": "Conflict Likelihood", "value": 7, "minValue": 0, "maxValue": 9, "isInverted": true }
  ],
  "labels": { "sections": { "yinYang": { "title": "Yin / Yang Analysis", "navigationTitle": "Yin / Yang" } } },
  "sections": { "strengthsAndChallenges": { "title": "Strengths & Challenges", "subtitle": "Potentially Challenging", "body": "existing personalised HTML reading" } },
  "details": { "gender": { "description": "existing selected Dictionary HTML with resolved links" } }
}
```

This fragment omits unchanged fields and other gauges. Actual title/navigation spelling comes from Dictionary and should never be substituted with the example text. Rebuild Globalisation and the website, run the Compatibility presentation regression tests in MobileHelpMapperTests, and verify paid/complementary results plus hidden/minor chemistry after Integration deployment. Source syntax/resource validation here does not establish Windows build/xUnit or live runtime success.
