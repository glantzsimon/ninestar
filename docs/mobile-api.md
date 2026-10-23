# Mobile JSON API

## Current implementation

Work remains on `feature/mobile-json-api`, separate from the plugin API. The expansion adds account/session operations, calculators, predictions/planners, calendar data and subscription management, reports, bookings, membership options/quotes, payment checkout, blog operations and support submissions. These are **implemented source**, not a declaration of complete controller parity or passing runtime tests. Custom administration, public content, typed selector metadata, owned messages and paid retrieve-last routes are also implemented. Complementary reading routes and constrained image uploads are also implemented. Some legacy administrative/account workflows and runtime verification remain on the checklist.

See [mobile-api-routes.md](mobile-api-routes.md) for declared routes. Every transport projection is an explicit object or DTO. Do not serialize the EF entity, calculated view-model graph, payment provider object or service result wholesale. The existing plugin ApiController is unchanged.

## Authentication and requests

The native client can use the existing Forms Authentication provider: retain the server cookies, call `GET account/csrf`, send the returned request token in `X-CSRF-Token` on **every POST**, and fetch a fresh token after login/logout. Login, reset/change-password and deletion require HTTPS outside localhost. Cookie-authenticated mobile operations do not use an embedded API key, a supplied user ID, or a second identity database. The OAuth implementation now on master belongs to the plugin project; it is not copied or replaced on this branch. OAuth/PKCE support for native apps requires integration with that implementation, not a parallel token system.

`MobileApiControllerBase` implements no-store JSON responses, redirects/error-page suppression, request-size limits (64 KiB), and CSRF validation before action execution. Actions explicitly check anonymous/member/admin and ownership requirements. `RequireUser` resolves existing Current identity/impersonation; checkout, account deletion and password changes refuse impersonation where the underlying provider operates on the actual login. Codes are protocol identifiers, not hardcoded UI messages. Native clients localise them.

JSON uses UTF-8, camelCase, ISO timestamps and no type metadata. Success is `{ success: true, data, error: null }`; failure is `{ success: false, data: null, error: { code } }` with an appropriate HTTP error status. Framework/routing failures before controller execution still require IIS checks.

Calculation requests explicitly include person, selected local date/time, birth/user timezone, calculation method, calculator type, house display and both hemisphere flags. Birth dates are date-only, birth time is separate, and selected timestamps have no UTC suffix. Validate real Windows/IANA timezones rather than silently falling back to GMT. Profile/compatibility calls combine birth time once. Planner calls pass birth date and time separately because the existing planner adds the time. Date-range limits and enum validation bound requests. The public chart preview returns numbers/overview only; full profile, compatibility, predictions and numerology use paid/admin routes. Separate `readings/*` routes apply the complementary section policy and counters.

## Shared services and projections

Calculations use NineStarKiService, NumerologyService, BiorhythmsService, IChingService, AstrologyService, ReportsService and existing planner batches. CombinedPredictionHelper populates request-owned planner results; personal and global descriptions remain separate. JSON projections preserve full source descriptions, morning/afternoon and inverted energies, lunar information, period boundaries, directions, numerology codes and series values. No Razor rendering is used by the new JSON endpoints. PDF and ICS remain existing file/download endpoints, with URLs or JSON metadata supplied as needed.

Profile projection preserves the website relationship age gate. Compatibility forwards the selected calculator and birth-time information and omits chemistry text/score when hidden or either person is below sixteen. I Ching templates are now copied before adding changing-line interpretations; otherwise one request could mutate the shared template used by another reading. A regression test covers that isolation.

The request culture is returned with profiles, but existing resource dictionaries and summary caches have inherited culture-isolation limitations. No claim of complete multilingual parity is made. Future work must fix those in their owning models/resources, rather than duplicating texts in the API.

## Account and ownership rules

Account updates load the authenticated user's entities and set an allow-list of profile fields. No client-supplied user ID, role, membership or password field is trusted. UserInfo is updated through the existing typed repository rather than passing untrusted strings to the legacy raw-SQL update helper. Preference updates accept only named calculation/timezone flags, never arbitrary preference keys. Reset-request responses never return the legacy service's password-reset token or indicate whether an email exists. Activation requires a real token or an OTP handle plus code/captcha. Deletion requires explicit confirmation and password reauthentication. All such endpoints only execute these effects when called; authoring them does not send messages or change accounts.

Booking list/calendar responses expose availability without other customers' names or bookings. Selecting a slot verifies the owning user-consultation association, matching duration, future time and availability, then delegates scheduling under the existing shared EF context's serializable transaction. Administrative slot edits are separately guarded.

## Payment contract

Mobile checkout uses the existing Stripe, membership, consultation and donation owners. Prices and eligibility come from server-side membership/consultation data; a client never posts a trusted PurchaseModel or successful-payment assertion. A caller supplies a stable request GUID. Stripe creation uses an idempotency key bound to that GUID and user. Confirmation reloads the server-created intent and verifies succeeded status, quoted amount and currency before fulfilling it.

Private checkout state is persisted as a purpose-bound MachineKey-protected JSON record under an internal `MobileCheckout:` UserPreference key, using the **existing database/context**, not a new table, migration or persistence project. These keys are not exposed through preference APIs. The existing public preference writer now accepts only ASCII identifier keys without colons, preventing users from overwriting/replaying internal checkout records, including through SQL-collation Unicode lookalikes. Protection is bound to both user ID and request GUID. Stable application MachineKey material must be retained for durable records to remain readable. A serializable SQL transaction protects creation/completion across workers; completed requests return the existing result rather than granting again. User ID, intent, amount and product association come from persisted server state. The existing Stripe interface has an additive idempotency overload; normal website callers retain their original method.

This requires Windows/SQL Server/Stripe integration tests for transaction sharing, concurrent completion, failed commit/retry and real provider responses. Legacy service email sends are not transactional with SQL and may require reconciliation if a surrounding transaction fails. No claim of exactly-once email delivery is made. Checkout is **not release-ready** before those checks. No payment, schema migration, merge or deployment has been executed during this work.

## Coverage and remaining work

| Family | Implementation status |
| --- | --- |
| PersonalChart | Full own chart, arbitrary-person full calculation, public numeric preview, free own-chart sections, alchemy and paid retrieve-last implemented. Complementary section projection and own/complementary alchemy implemented through the readings routes. |
| Predictions | Calculation, own snapshot, planner hierarchy, generic cycles, combined descriptions, moon phase and alchemy implemented. Paid retrieve-last, own/free yearly sections and complementary yearly sections/alchemy implemented. |
| Compatibility | Paid calculation with explicit privacy/age checks and retrieve-last implemented; complementary calculation omits the two full personal-chart payloads. |
| Numerology/Biorhythms | Paid calculations, numerology forecast and structured planners/series implemented. Paid retrieve-last implemented. Complementary numerology implemented. The website paywalls biorhythm gauges, planners and analysis; these remain paid. |
| IChing/KnowledgeBase | Public cast and structured knowledge-base data implemented. |
| PersonalCalendar | Month JSON and protected subscription/revoke implemented; existing ICS feed retained. |
| Reports | Structured yearly report, existing PDF URL and admin-only prompt implemented. |
| Account | Session login/logout, CSRF token, own account, profile/preferences updates, registration/activation, password operations and deletion implemented. Native OAuth/PKCE and some legacy external-auth/promotion flows pending. |
| Membership/Payments | Options and quotes, promo redemption and server-verified durable checkout implemented. Provider/SQL tests required. |
| Consultation | Owned bookings, slots/calendar, transaction-protected selection and admin slot operations implemented. |
| Blog/Support | Public articles/tags, comments/likes/moderation, contact/newsletter submission and donation totals implemented. Article editing, publishing and deletion delegate existing owners. Per-user messages mirror the existing owned CRUD without adding delivery semantics. |
| Generic administration and operations | Twenty explicit entity contracts provide admin-only schema/read/page operations and selected repository-backed writes. Users and processed email queue are read-only here; user lifecycle and sending use their owner services. Custom user lifecycle and assignments, impersonation, article publishing, template test/single/list sending, logs, free-reading maintenance and MailChimp sync are implemented. Six explicit association pairs support atomic replacement with target validation. SQL recipient preview, donor import, resource export and further hook review remain in progress. No arbitrary type/table or internal-preference endpoint exists. |

## Additional transport contracts

`GET personal-chart/me/free` returns numbers and the core, character and intellectual sections granted by the website for one's own chart. It never calls the full mapper or returns energy descriptions for paid sections. Full `/me` remains paid. The five calculator `/last` endpoints save only validated request inputs under a user-bound session key and recalculate after fresh entitlement checks. Saved settings include birth time/timezone and calculation flags; switching identity cannot retrieve another user's saved request. Logging out clears this session state. Anonymous previews are not saved.

`content/{page}` returns resource-backed title and authored content for privacy, terms, FAQ, data removal, calculation methods, southern hemisphere and AI information. Resource-authored HTML is content, not a rendered view. `metadata/options` supplies protocol values with resource-backed enum labels; `metadata/timezones` mirrors the existing extended selector. Administrative schemas include resource-backed field labels and enum choices. No complete multilingual guarantee is implied.

Bulk association pairs are `user-roles`, `role-permissions`, `user-mailing-lists`, `contact-mailing-lists`, `mailing-list-users` and `mailing-list-contacts`. Replacement takes `{ ownerId, selectedIds }`, rejects missing/deleted targets, and applies repository changes in one serializable transaction. An empty array clears the selection; automatic mailing lists cannot be edited as stored rows. General records and association reads exclude soft-deleted rows.

Administrative mail and operational endpoints execute effects only when explicitly called. Test/single/list sends reuse MailerService; free-reading maintenance is shared with the existing web controller through MembershipService. The all-account maintenance and MailChimp sync routes require `confirm: true` as part of their request contract. Updating another user's saved preferences no longer replaces the administrator's current session preferences.

## Complementary readings and images

`POST readings/personal-chart`, `readings/predictions`, `readings/compatibility` and `readings/numerology` are explicit credit-consuming operations for free memberships. They validate inputs, read available credits and delegate the relevant MembershipService decrement under the shared scoped DbContext's serializable transaction. Projection/serialization precedes commit so a failed calculation does not consume the credit. These transaction semantics still require SQL Server concurrency tests. Paid accounts use the normal calculator routes; they are not debited here.

Complementary profiles expose the same restricted sections as the own/free chart. Complementary predictions expose the yearly personal, lunar and global interpretations; they do not return all paid periods or the full profile graph. Complementary compatibility exposes compatibility detail and numeric personal charts, rather than returning unrelated paid natal sections. Numerology follows its complementary grant for the calculation and outlooks. `GET readings/{section}/last` recalculates the session's successfully granted reading without another debit. The grant is server-held, bound to the current user, expires with the session and cannot be created by the paid retrieve-last routes. New POSTs each consume a credit; clients should use the corresponding GET to reopen a successful reading rather than replaying POST. Own and successfully granted complementary profile/prediction alchemy use existing services and the enabled setting.

`POST media/avatar` writes only the current account's avatar. `POST media/article-image` and deletion require administration. JPEG/PNG payloads are limited to 2 MiB, decoded, checked for dimensions, resized and re-encoded as PNG with server-generated filenames. The media transport alone allows multipart framing above the normal 64 KiB JSON cap. Storage uses MediaManagementService and fixed mobile image folders. No caller-supplied filesystem folder, executable file extension, arbitrary CLR image-option type or arbitrary deletion path is accepted. Avatar removal clears the current account's reference and removes only an owned mobile image; it does not delete external/legacy media. Account responses include the avatar URL. Image decoding and storage still need Windows/IIS tests.

## Remaining scope decisions and validation gates

- Account external identity flows should link to the existing authentication/OAuth owner rather than return Facebook/Google redirect views or introduce another token store. Integration with current master is pending.
- Legacy email promotion and unsubscribe links, donor-to-contact import, recipient SQL preview and administrative resource exports remain to be mapped explicitly. Existing file endpoints (ICS/PDF) continue to serve their formats.
- Media management deliberately supports owned avatars and admin article images; arbitrary file upload, arbitrary type-based dynamic image controls and arbitrary-path deletion are excluded from the JSON transport.
- Pure view layouts, success/failure redirect pages, debug/test controllers, raw SQL maintenance utilities and controller infrastructure helpers are not mobile data endpoints. Any newly required operational capability must use a reviewed explicit contract.
- Build the complete branch on Windows with restored K9 packages and decrypted astronomy sources, run xUnit, then validate routing/error JSON, cookie/CSRF handling, ownership and section grants under IIS.
- Exercise SQL transaction sharing and concurrent complementary/booking/checkout writes; test failed commit/retry and Stripe confirmation before release. Source checks cannot prove these behaviours.

## Validation and next gates

Added input/timezone/birth-time, I Ching isolation and session identity/access-revocation, free-section projection and image-boundary tests alongside the existing mobile profile tests. C# syntax, project XML, explicit compile registrations, unchanged plugin-controller sources and whitespace are checked in this environment. The environment has no .NET Framework/MSBuild/Mono runtime or restored K9 binaries, and astronomy source is git-crypt protected. Compilation, xUnit execution and MVC/IIS/SQL/Stripe integration tests are **not executed**. A draft PR is not a deployment approval.

Continue the remaining coverage checklist on the same branch, then build/run the website tests in the owner's Windows checkout and validate authenticated requests and error paths under IIS. Do not mark the full API finished while any controller family remains pending.

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
