# Mobile JSON API

The proposed shared website/app Apple subscription contract is in [Shared website/app membership and Apple subscriptions](#shared-websiteapp-membership-and-apple-subscriptions-proposed-contract). It is a draft for agreement, not an implemented billing feature.

## Current implementation

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

## Additional transport contracts

`GET personal-chart/me/free` returns numbers and the core, character and intellectual sections granted by the website for one's own chart. It never calls the full mapper or returns energy descriptions for paid sections. Full `/me` remains paid. The five calculator `/last` endpoints save only validated request inputs under a user-bound session key and recalculate after fresh entitlement checks. Saved settings include birth time/timezone and calculation flags; switching identity cannot retrieve another user's saved request. Logging out clears this session state. Anonymous previews are not saved.

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
| POST `subscriptions/apple/context` | `{}` | `{ appAccountToken, purchaseAllowed, purchaseBlockedReason, products, entitlements }`. Idempotently get/create this account's binding. `products` contains only enabled server-allowlisted `{ productId, subscriptionType }` mappings. Token is returned even when purchase is blocked, to support same-account restore. |
| POST `subscriptions/apple/verify` | `{ "transactionId": "opaque-storekit-id" }` | `{ processed: true, state, entitlements }` only after server verification and durable transaction/grant commit. Repeated same-owner submissions succeed without duplicate benefits. A valid inactive purchase can be recorded with `state: "expired"` or `"revoked"` and no Apple access. |
| POST `subscriptions/apple/refresh` | `{}` | `{ processed: true, entitlements }` after server reconciliation of this account's already-linked chains. No arbitrary transaction/account selector. Rate-limit/coalesce calls. |

`purchaseBlockedReason` is null when allowed, otherwise `website_membership_active` or `apple_subscription_active` for a current active/grace grant; current-chain billing retry uses `apple_subscription_requires_management` until provider reconciliation establishes the appropriate recovery route. No second subscription should be offered blindly. `products` is a product mapping, not a price quote: display actual StoreKit-localised pricing/periods and disclosures.

Example entitlement snapshot (illustrative timestamps and enum values, not a live response):

```json
{
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
      "autoRenewEnabled": true
    }
  ]
}
```

`paid` and `unlimited` represent membership benefits, not an admin-role bypass. `websiteAccess` and `mobileAccess` mean membership-derived paid access; free features still work when false. `sources` includes the current legacy grant (if any) plus owned Apple chains, including inactive chains needed for management; it never exposes provider evidence, other accounts, tokens, secrets or transaction history. Provider `website` represents existing website/admin/promotion grants; its `endsOn` is the legacy inclusive date (`yyyy-MM-dd`, null for lifetime) and `expiresAtUtc` is null. Apple uses the exclusive UTC access boundary, including verified grace when applicable, and null `endsOn`. A null legacy auto-renew flag may be returned when actual provider renewal state is unknown; legacy IsAutoRenew alone is not proof of a Stripe recurring subscription.

Add the **same** snapshot as an additive `entitlements` property on `GET account/me`; preserve its existing `membership` fields and meanings for compatibility. Do not replace or rename current fields. The dedicated GET and account snapshot use one shared projection. Existing paid routes continue server-side membership enforcement; the app must not calculate access from its own receipt or dates. Fetch account/entitlements after purchase, restore and foreground, and handle fresh membership-required responses.

Restore is a client StoreKit operation followed by server verification: enumerate restorable/current transactions for the configured products, call `verify` per transaction, then `refresh` and read account/entitlements. No separate receipt-trusting restore endpoint is needed. If no device transaction exists, `refresh` can still recover this account's known chains. Finish a StoreKit transaction only after the server returns committed processing success; retain/retry on transport/provider failures. StoreKit pending/Ask-to-Buy/cancelled/unverified client states never grant access. An expired/refunded processed transaction can finish without unlocking.

Proposed error contract uses the existing failure envelope:
- 400: `invalid_apple_request`, `invalid_apple_transaction`, `apple_product_not_supported`, `apple_environment_not_allowed`.
- 401: `authentication_required`; existing 400 `invalid_csrf_token` remains unchanged.
- 403: `impersonation_not_allowed`, `apple_account_mismatch` (wrong/missing token, unsupported ownership or another account's chain, without revealing its owner).
- 409: `apple_account_link_required` for a genuinely unbound/tokenless legacy purchase needing reviewed support recovery. Never silently claim it or overwrite its token.
- 429: `apple_rate_limited`.
- 503: `apple_verification_unavailable` for provider/network/verifier/reconciliation failure. No new/unverified access on failure; retry with the same transaction ID. Do not return a durable-success assertion before commit.

The existing request-size limits apply. Bound and validate transaction ID strings (maximum 128 characters); no client JWS, expiry, price, plan or success flag is required/trusted.

### Server verification and ownership

For verify/restore, retrieve transaction information from Apple using the supplied ID, validate its signed transaction, then obtain current subscription status for the verified originalTransactionId. Verify signed renewal/transaction data, configured bundle ID and production app Apple ID, enabled environment, product allowlist/group, supported auto-renewable type, ownership and current expiry/grace/revocation state. Select the intended chain, not unrelated subscriptions included in Apple's response. A historical transaction's expiry alone cannot establish current access.

Validate JWS signatures, certificate chain to configured Apple trust roots and certificate policy/validity/revocation as required; never trust an embedded x5c root, an arbitrary client signing key, unverified decoded claims, or an unexpected algorithm. Keep provider API credentials/private signing keys on the server. Use fixed Apple API hosts, bounded timeouts and controlled retries; client input cannot select a URL. The concrete .NET-compatible verifier and its signed-data test fixtures are a pre-implementation technical gate.

Require the cryptographically verified appAccountToken to match this signed-in account's server-issued binding on initial link. Enforce immutable chain ownership with database uniqueness/transactions, not only application checks. On subsequent verification, renewals and restore, both chain ownership and token must remain consistent; fail closed/quarantine anomalies. Do not call Apple's Set App Account Token API to make a mismatched transaction fit. A UUID known by a caller is not independent proof of identity.

Tokenless purchases are not auto-linked by email, receipt possession or first caller. Previously linked tokenless transactions may be reconciled only through the existing trusted immutable owner association; initial adoption requires a separately reviewed recovery process. Family-shared transactions do not grant a second NineStar account access in this first version.

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
