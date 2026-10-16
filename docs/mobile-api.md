# Mobile JSON API

## Purpose and boundaries

The website and mobile app share the existing application services and repositories. Mobile controllers project those results into explicit JSON transport models; they do not invoke MVC view actions, render Razor fragments, duplicate calculations, or serialize complete EF entities/view models. Keep the existing plugin `ApiController` and `ApiPureAlchemyController` unchanged: their API-key/account-number contract serves a different client.

Mobile routes use `/api/mobile/v1`. Start with domain controllers rather than putting all actions into another large controller. `MobileApiControllerBase` supplies only protected response/identity helpers and exception handling; it derives directly from MVC `Controller`, avoiding the public UI/upload actions and session-writing initialisation inherited from `BaseNineStarKiController`. Autofac already registers MVC controllers by assembly, and attribute routing is already enabled.

This branch currently implements **one endpoint**, not complete controller parity. The controller inventory below is a coverage plan. Personal-chart views and their nested partials/paywalls were inspected for the first projection; other families still need view-by-view field and entitlement reviews before implementation. Static pages and redirect/success actions generally map to content or structured outcomes, rather than one endpoint per HTML page. Admin and operational functionality belongs behind separate permissions and must never become public merely to obtain JSON parity.

## Authentication and access

The initial endpoint uses the existing authenticated **web session** and `Current.UserId`, preserving the application's impersonation convention. A plugin API key, account number, query-string user ID, or JSON payload cannot establish mobile identity. Native mobile login/token issuance, expiry, revocation, refresh and logout are **not implemented**. This endpoint is a development foundation that can be exercised from an authenticated same-origin browser session, not a finished standalone mobile authentication system.

`GET personal-chart/me` returns full details, including sections protected by `HtmlHelpers.PayWall` in the web views. It requires `IUserService.UserIsAdmin` or an active membership passing `IsAuthorisedToViewPaidContent()`. Identity and entitlement are checked on every request, before user data or calculations. This intentionally does not provide the website's limited free own-chart or complementary-reading experience yet; those require a separate field-level response policy and existing reading accounting. Do not relax the complete-response gate to implement a free preview.

The existing membership lookup can create a free membership if absent. Birth information is read through `UserInfosRepository`; this GET does not call `GetOrCreateUserInfo`, `AccountService.GetAccount`, preference setters, AI alchemy, email, payment, or booking workflows. It does not consume complementary readings. Repository calls remain sequential because the services share an EF context.

Future writes must retain ownership, permissions and validation. With cookie/session authentication they also require CSRF protection. Native bearer authentication needs an explicit contract before adapting these actions; never place a shared secret in the app and treat possession of it as user authentication. Administration, billing, account deletion, emails and maintenance must retain their existing specific safeguards.

## Implemented endpoint

| Method and route | Input | Output | Access |
| --- | --- | --- | --- |
| `GET /api/mobile/v1/personal-chart/me` | Authenticated session; saved birth details and preferences. No caller-selected account/user ID. | Allow-listed natal profile, full descriptions and summaries, energy artwork metadata, settings and person display fields. | Authenticated admin or active paid membership. |

The request resolves saved calculation method, calculator type, house display, user timezone, natal/cycle hemisphere inversion flags and birth-time-known preference. Preference fallbacks come from the saved UserInfo, following the calendar service convention. Missing birth/timezone details and invalid zones/settings return `409`; invalid timezone identifiers must not silently use the shared helper's GMT fallback. Windows and IANA zones use the existing TimeZoneConverter dependency and DateTimeHelper zone normalisation.

Birth date is normalised with `.Date`, then the saved birth time is added exactly once for `CalculateNineStarKiProfile`. Birth and selected local timestamps have `DateTimeKind.Unspecified`; birth/user timezones remain separate. Selected time is current UTC converted into the saved user timezone. The service receives every calculation option explicitly, with `isMyProfile: true`, `includeCycles: false`, `includePlannerData: false`, and `includeMoonPhases: false`. This is a natal-profile response: current houses, lunar data, directions, predictions, alchemy and planner/calendar data remain separate pending endpoints. Do not return the service's placeholder cycle energies when cycles are disabled. The profile service still performs its existing ancillary Lichun/direction work; this change does not claim to eliminate all non-natal calculation cost.

`MobilePersonalChartMapper` reads the request-owned profile without mutation. `ViewModels/MobileApi` contains transport-only types with no calculated getters or EF navigation properties. Dictionary keys and error codes are stable protocol names; the app supplies localised navigation/field labels. Description content comes from the existing model/resource getters, with full text preserved. Some existing energy dictionaries capture strings statically and the profile summary cache does not vary by culture; this first slice inherits that limitation. The `culture` field identifies the current resource/request culture, not a guarantee that every inherited cached description was resolved in it. Audit and correct culture isolation in the owning models before promising multilingual mobile parity. Body/description strings may contain existing resource HTML, but no rendered page, controls, layout, scripts or partial views are included. Native rich-text handling and a general localisation/metadata endpoint are still pending.

### Response contract

JSON is UTF-8 with `application/json`, camelCase property names, ISO date/time values and no type metadata. Both successes and errors set `Cache-Control` to prevent storage; IIS error replacement and Forms Authentication login redirects are suppressed. Unexpected action exceptions become `500/internal_error` without exception details. Unmatched routes and framework failures outside the controller are not covered by this action-level error contract and require IIS integration verification.

Success envelope:

```json
{ "success": true, "data": { "person": {}, "settings": {}, "culture": "en-GB", "summary": "...", "overview": "...", "energies": {}, "sections": {} }, "error": null }
```

Error envelope:

```json
{ "success": false, "data": null, "error": { "code": "authentication_required" } }
```

| HTTP status | Error codes |
| --- | --- |
| `401` | `authentication_required` |
| `403` | `membership_required` |
| `409` | `birth_details_required`, `timezone_required`, `invalid_timezone`, `invalid_calculation_settings` |
| `500` | `internal_error` |

| Data field | Content |
| --- | --- |
| `person` | Name, date-only `yyyy-MM-dd` birthday, `HH:mm:ss` birth time, known-time flag, birth timezone, gender enum name and age-category flags. No email, account number, user ID or entity. |
| `settings` | Calculation method, calculator type, house display, user timezone, selected local timestamp (no UTC suffix), and both inversion flags. |
| `culture`, `summary`, `overview` | Current resource culture and existing full profile overview text. |
| `energies` | `epoch`, `generation`, `core`, `character`, `socialExpression`, `dayStar`, `hour`. Each contains number/name/title, element/modality and descriptions, yin/yang, trigram and description, direction, family member, colour and shared image URL. A missing energy is null. |
| `sections` | Each value is `{ body, summary }`; body preserves full text and summary is null where no separate source summary exists. See keys below. |
| `strongYinOrgan`, `strongYangOrgan`, `weakYinOrgans`, `weakYangOrgans` | Existing organ enum names; nullable strong-organ values and empty arrays for absent weak-organ collections. |

Section keys: `core`, `character`, `socialExpression`, `intellectualQualities`, `interpersonalQualities`, `emotionalLandscape`, `career`, `occupations`, `finances`, `health`, `illnesses`, `spirituality`, `childNature`, `adultChildRelationship`, `stressResponse`, `natalHouseStressResponse`, `childStressResponse`. For people aged sixteen or over, also `relationships`, `relationshipsStraight`, `relationshipsLgbtq`, matching `_ProfileMainEnergyDetails`' relationship-tab gate. Use the existing PersonModel age rules; this change does not alter them.

## Coverage plan

| Controller family | Data/outcomes to expose | Existing owner and access considerations | Status |
| --- | --- | --- | --- |
| PersonalChart | Calculator defaults/options, arbitrary-person charts, saved own chart, energy details, alchemy and retrieval state. | NineStarKiService, UserService, membership reading accounting; free/own/complementary/paid fields differ. | Full paid own natal chart implemented; other actions pending. |
| Predictions | Current personal/global cycles, planner rows/boundaries, detail and combined month/day descriptions, alchemy. | GetPlannerData batches and CombinedPredictionHelper; preserve all options, personal/global resource families and access. No rendered planner HTML. | Pending. |
| PersonalCalendar | Month entries, energy metadata, subscription creation/revocation. | GoogleCalendarService, existing paid/admin checks; token secrecy, timezone and CSRF rules. Feed remains ICS, not JSON masquerading as a feed. | Pending. |
| Compatibility | Both profiles, compatibility details/summaries, optional sexual chemistry, retrieval state. | CalculateCompatibility; preserve calculator semantics, privacy flag and complementary-reading consumption. | Pending. |
| Biorhythms | Calculated series, summaries and display period. | BiorhythmsService using an existing calculated profile; inspect view paywall. | Pending. |
| Numerology | Calculated model, forecast detail and planner codes. | NumerologyService and batch APIs; complementary karmic reading accounting and period/offset conventions. | Pending. |
| IChing | Generated hexagram, lines and existing interpretation data. | IChingService and IChingViewModel; distinguish retrieval from a new random toss. | Pending. |
| KnowledgeBase | Energy/element/modality summaries and interpretation content. | GetNineStarKiSummaryViewModel and resource-backed models; public vs gated content review. | Pending. |
| Reports | Structured report sections and download/job outcomes. | ReportsService, AIService, PdfService; retain specific authorisation. PDFs remain file responses, with JSON metadata where needed. | Pending. |
| Account and AccountControllerAjax | Authentication, registration, activation, reset, safe account/profile/preferences data, deletion outcomes. | AccountService, GoogleService and UserService; own-account checks, captcha/OTP/rate controls and CSRF. Never serialize account/user entities. Native auth is a prerequisite for standalone app use. | Pending. |
| Membership and MembershipOptions | Plans, eligibility, current entitlements, checkout/switch outcomes. | MembershipService and pricing models; separate public catalogue from admin plan CRUD. | Pending. |
| Payments (PaymentController.cs) | Payment-intent/checkout metadata and verified purchase outcomes. | Existing StripeService/PurchaseModel workflow. Do not trust client success or expose payment secrets/admin data. | Pending. |
| Consultation and Consultations | Slot availability, owned bookings, scheduling results; separate admin slot/booking operations. | ConsultationService, timezone/availability rules, paid entitlements, ownership and administrator permissions. | Pending. |
| Blog and Articles | Articles/tags, comments/likes, moderation and publication outcomes. | ArticlesService; public reading, authenticated owned edits and admin moderation/publishing are distinct. | Pending. |
| Support, Donations and Contacts | Contact/newsletter submissions, donation checkout/outcomes; separate admin contact/donation data. | Existing captcha, ContactService, DonationService, Stripe and mail workflows. | Pending. |
| EmailTemplates, EmailQueue, MailingLists, MailingListUsers, MailingListContacts | Authorised template/list/queue data and sending or list-edit outcomes. | Existing email/template/queue services and per-operation permissions. Email sends are writes, never incidental GET side effects. | Pending admin API review. |
| Users, UserMemberships, UserConsultations, UserPromotions, Promotions, UserRoles | Admin records, assignments, credits/promotions and safe account views. | Existing membership/promotion/consultation services and admin/permission checks; avoid arbitrary-user access. | Pending admin API review. |
| Roles, Permissions, RolePermissions, Countries, Messages, SystemSettings | Allow-listed reference/configuration/CRUD data. | Existing generic BaseNineStarKiController CRUD and actual controller permissions; Countries must not be assumed to share the other controllers' attributes. No secret-bearing settings projection. | Pending individual review. |
| Home, Iboga, Error | Informational content, FAQ and structured error outcomes where the app needs them. | Resources/content, not rendered landing/error layouts. | Pending content review. |
| BaseNineStarKi | Language/preferences, retrieval navigation and shared media/dynamic-field helpers. | Explicit app endpoints only; do not inherit or automatically export public base-controller helpers. | Pending individual review. |
| Admin, Log, Test | Maintenance, logs, text/resource export, test email and diagnostics. | Operational/admin-only functionality; explicit permission and necessity review before any mobile exposure. | No endpoints added. |
| Api and ApiPureAlchemy | Existing plugin calculations, knowledge base, I Ching and utility actions. | Existing plugin contract; mobile consumes shared services rather than calling these actions. | Unchanged. |

Inherited generic CRUD actions are not fully enumerated by the concrete-file action inventory below. Inspect K9's actual base-controller permission/event behaviour before providing equivalents. The table records every current controller family; it is not evidence that every family has passed a complete security or view-field audit.

## Next implementation sequence

1. Verify the first slice on Windows/IIS, including actual session resolution, expired membership, culture, routing and JSON errors.
2. Design native app authentication and safe account/preference contracts using existing account services.
3. Add free/complementary chart projections and explicit arbitrary-person calculation inputs, preserving existing accounting.
4. Add predictions/planner/calendar endpoints using existing batch contracts, then compatibility, numerology, biorhythms, I Ching, knowledge base and reports.
5. Cover booking, payments, content and other writes with their existing permissions and mutation protections; review admin/operational exposure separately.
6. Add a machine-readable API specification and contract checks as routes settle. This first slice does not introduce a new framework/dependency or migrate the application.

## Validation

Added xUnit/Moq tests for anonymous/unresolved identity, free/expired memberships, missing birth details, missing/invalid zones, saved options, birth-time normalisation, membership revocation between requests, response allow-list/full-text projection, child relationship gating, and JSON/no-store/redirect behaviour. They mock calculations and do not establish astronomical correctness.

C# parsing with tree-sitter found no syntax errors in the five added C# files. Both project XML files parse, every added C# file is explicitly registered, the controller inventory covers all 47 existing controller source files, and whitespace checks pass. The plugin controller files are unchanged.

This environment has no .NET Framework/MSBuild/Mono runtime or restored proprietary K9 dependencies, and astronomy sources remain git-crypt protected. C# tests and the application cannot be compiled or executed here. Local source/project checks do not replace a Windows build and IIS checks. No merge or TeamCity deployment is part of this work.

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
