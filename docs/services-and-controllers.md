# Application service and controller reuse

Scope: the current `webapp/WebApplication/Services` implementations/interfaces and `Controllers` sources, with the supporting packages, cache/model and session/membership contracts. The unencrypted sources were reviewed to map responsibilities and reuse points. `AstronomyService.cs` and `IAstronomyService.cs` are git-crypt protected; astronomy method names below are verified from their existing callers. Their underlying algorithms were not inspected or changed. The owner confirms the existing astronomy batch functions are tested and efficient. This map is not a runtime benchmark or a guarantee that every surrounding workflow is optimal.

## Shared infrastructure

`Services/BaseService.cs` inherits `Models/CachableBase.cs`, exposing the logger, repositories, authentication, configuration and mailer through `INineStarKiBasePackage`. Package declarations live under `ControllerPackages/` in namespace `K9.WebApplication.Packages`. `INineStarKiPackage` adds account, user, membership, contact and media-management services. Preserve constructor injection and the existing registrations.

`BaseNineStarKiController` sets roles/preferences, beta state and device information in `OnActionExecuting`. It provides preference actions, shared upload/image controls and `RenderPartialViewToString`. Its generic counterpart extends K9's `BaseController<T>` and converts curly markup to/from HTML through CRUD events. Existing admin CRUD controllers already use these hooks. Do not recreate generic CRUD or markup conversion.

`Current.UserId`/`GetUserId` preserve session impersonation before falling back to WebSecurity. `Helpers/SessionHelpers.cs` holds the application's `SessionHelper`; several controllers also alias K9's base helper. Inspect which helper is actually referenced. The user-preference collection getter tolerates absent session, but setters such as `SetValue` require it.

## Calculations and batching

| Existing entry point | Result and reuse |
| --- | --- |
| `NineStarKiService.CalculateNineStarKiProfile` | Full natal profile, optional cycles and moon phase/directions. Use for a snapshot; do not repeat per date when a planner batch fits. Pass all calculation options explicitly for non-session callers. |
| `NineStarKiService.CalculateCompatibility` | Shared compatibility calculation; used by Compatibility and API controllers. |
| `GetNineStarKiSummaryViewModel` | Cached energy/description collections used by the knowledge base and prediction endpoints. |
| `GetPlannerData`, EightyOneYear | Nine-year rows from `GetNineStarKiNineYearPeriodsWithinEightyOneYearPeriod`. |
| `GetPlannerData`, NineYear | Year rows from `GetNineStarKiYearlyPeriodsForNineYearPeriod`. |
| `GetPlannerData`, Year | Month rows from `GetNineStarKiMonthlyPeriods`; parent `Energy` is the year house. |
| `GetPlannerData`, Month | Daily/afternoon rows from `GetNineStarKiDailyEnergiesForMonth`; parent `Energy` is the month house when using its own selected-date profile. |
| `GetPlannerData`, Day | Hourly rows from `GetNineStarKiHourlyPeriodsForDay`. |
| `NineStarKiModel.GetPersonalCycleEnergy`/`GetGlobalCycleEnergy` | Existing personal/global mapping, house display and inversion logic; do not reimplement arithmetic in a controller/export. |
| `AstrologyService.GetMoonPhase` | Astronomy moon phase enriched with existing resource-backed lunar and yin/yang descriptions. |
| `NumerologyService.CalculateDailyPlannerCodes`, `CalculateMonthlyPlannerCodes`, `CalculateYearlyPlannerCodes`, `CalculateDharmaCodes` | Existing numerology batches. Use these rather than loops of individual forecast/profile calls. `GetDailyForecast`, `GetMonthlyForecast`, `GetYearlyForecast` supply detail. |
| `BiorhythmsService.Calculate` | Accepts an existing NineStarKiModel and produces results/summary with monthly astronomy boundaries. |
| `ReportsService.GetYearlyReport` | Account/report assembly and Year planner. Already demonstrates passing the calculated profile into `GetPlannerData`. |
| `GoogleCalendarService.GetCalendarEntries` | Year planner once per covered solar year, overlapping Month batches with the same profile, range trimming, houses and combined descriptions. ICS formatting/token handling stay in this service. |

The planner hierarchy does not return every lower level automatically: Year rows are months, not days. Expand the required child periods with the parent's `nineStarKiModel` to avoid repeating full profiles. Read year/month houses from their respective parent energies, rather than the reused profile's fixed snapshot. Obtain boundaries from the existing astronomy-backed periods; do not assume Gregorian months or hardcode February.

`GetPlannerData` takes a date-only birth date and separate time of birth and combines them itself. Pair local selected dates with user timezone and birth details with birth timezone. Preserve calculation method, calculator, house display, scope and both hemisphere options. `NineStarKiModel.IsCycleSwitchActive` depends on its selected date and existing `CYCLE_SWITCH_DATE`; refresh a reused profile across that state change without mutating cached objects.

## Other service owners

| Existing owner | Reuse and cost/side effects |
| --- | --- |
| `AccountService`/`AccountMailerService` | Registration/login, external-auth completion, activation/OTP, password reset, account assembly and account emails. `GetAccount` assembles related data; use a simple UserService lookup when only a user is needed. |
| `GoogleService.Authenticate` | Validates Google ID-token audience and returns ServiceResult for existing account flow. This is Google sign-in, separate from the calendar feed. |
| `UserService` | User lookup/admin checks, birth UserInfo, saved preferences, pending consultations, deletion and marketing preferences. Read preferences once per operation; each generic getter queries the repository. Updates also refresh session preferences. `GetOrCreateUserInfo` can create data. |
| `MembershipService` | Active membership selection, purchase/switch/assignment, free membership, complementary reading consumption, consultations and reminder scheduling. `GetActiveUserMembership` may create a missing free membership. Keep existing `UserMembership.IsAuthorisedToViewPaidContent`/`IsUnlimited` and accounting semantics. |
| `ContactService` | Match/create/update contacts by Stripe customer/email and synchronise marketing preferences with user records. `GetOrCreateContact` can update existing records. |
| `ConsultationService` | Available slots, weekly slots/bookings, slot lookup/selection, consultation creation and notifications. Reuse timezone and availability rules; weekly assembly hydrates related records. |
| `Stripe/StripeService` | Checkout sessions, customers and payment intents; used by payment flow. `Stripe/Charge.cs` is a data object, not another orchestration service. |
| `DonationService` | Donation creation, notification and funds total. |
| `PromotionService` | Promotion eligibility/consumption, registration/membership offers and scheduled reminders, integrated with existing queue/templates. |
| `EmailTemplateService` | Template lookup and Parse/ParseForUser/ParseForContact, unsubscribe URLs, base email wrapper and inline image expansion. Prefer overloads accepting an already-loaded template for bulk work. |
| `MailerService` | Single/test/bulk template orchestration; bulk path loads template/promotion once and records recipient results. |
| `EmailQueueService` | Queue insertion, duplicate-send window, delayed scheduling, configured batch size/pause, unsubscribe/upgrade checks and processed/sent outcomes. Keep these rules in the queue. Account/support workflows also use direct mail intentionally. |
| `MailingListService` | Saved and automatic lists and recipient selection. `Find`/`List` expose `includeUsers`; `ListAll` expands users and automatic membership lists. Inspect cost before using list helpers repeatedly. |
| `MailChimpService` | Existing external audience sync via AddContact/AddAllContacts. |
| `ArticlesService` | Article/tag hydration, comments/moderation, likes, views and dashboard. Preserve ownership/approval checks; detailed article loading expands related data. |
| `AIService` | Async report and text merge operations and report prompt construction. These make external requests; do not put them in calendar/date loops or create a second AI client workflow. |
| `PdfService` | HtmlToPdf/UrlToPdf using the configured wkhtmltopdf process and temporary-file cleanup. Reuse it instead of another process wrapper; it is not a cheap per-row formatting helper. |
| `MediaService`/`MediaManagementService` | Shared image/video base URLs, periodic hosted-media health/fallback and Storj upload process. Use the shared paths; do not probe media availability per displayed image. Base-controller local upload and Storj upload are separate existing paths. |
| `RecaptchaService` | Existing contact/registration captcha validation. |
| `IChingService` | Hexagram generation for the I Ching page/API. |
| `LogService`/`CookieService` | Existing log retrieval and static cookie-warning preference helpers. |

## Controller navigation map

| Controllers/files | Existing responsibility |
| --- | --- |
| `PersonalChart`, `Compatibility`, `Predictions` | Profile/compatibility calculation, saved/retrieved readings, complementary access and prediction detail/planner routes. Predictions.GetPlanner renders `Planner/_GlobalPlanner` and sets parent/child URLs. |
| `Biorhythms`, `Numerology`, `IChing`, `KnowledgeBase` | Their service-backed calculators, forecasts, saved-state or summary views. |
| `Reports` | Report, prompt and PDF endpoints using Reports/AI/PDF services. |
| `PersonalCalendar` | Direct MVC Controller with explicit access/culture handling, month data, protected subscription/revoke POSTs and private anonymous feed. Read-only month session is selected in Global.asax before acquiring session; preserve this special case. |
| `Account`/`AccountControllerAjax.cs` | Account/login/registration/activation/reset/external-auth/preferences and unsubscribe workflows. Both files are one partial controller. |
| `Api`/`ApiPureAlchemyController.cs` | Shared API-key and account/membership validation envelope and calculator endpoints; the latter file is part of ApiController. Reuse its validation/results instead of an independent account lookup per API step. |
| `Membership`, `PaymentController.cs`, `Consultation`, `Support` | Purchases/switching, Stripe intent-to-PurchaseModel conversion, booking and contact/donation flows. PaymentController.cs declares PaymentsController. Preserve specific access and POST/anti-forgery checks. |
| `Articles`, `Blog` | Admin editing/publishing and public reading/comments/likes/moderation, using ArticlesService and shared markup hooks. |
| `EmailTemplates`, `MailingLists`, `MailingListUsers`, `MailingListContacts`, `EmailQueue`, `Contacts` | Template/bulk/test sends, list membership/editing, queue CRUD and newsletter/MailChimp flows. |
| `Users`, `UserMemberships`, `UserConsultations`, `UserPromotions`, `UserRoles`, `MembershipOptions`, `Consultations`, `Promotions` | Generic administration with assignment and promotion/consultation custom actions. |
| `Roles`, `Permissions`, `RolePermissions`, `Countries`, `Donations`, `Messages`, `SystemSettings` | Existing generic CRUD controllers. Inspect each actual authorisation attribute rather than assuming identical permissions. |
| `BaseNineStarKi`, `Admin`, `Log`, `Home`, `Error`, `Iboga`, `Test` | Shared infrastructure, admin maintenance/resource exports, log display, static/error pages and utility/test pages. Iboga/Log/Test use K9 BaseController directly. |

## Cache and performance contracts

`CachableBase.GetOrAddToCache` uses process-wide `MemoryCache.Default`, defaults to 30 minutes, and returns the stored reference. NineStarKi profile/planner/summary calls explicitly use 30 days. The helper neither clones objects nor coalesces concurrent misses. Do not infer request isolation from service instances or mutate cached profiles for new dates.

Cache keys must reflect every input that changes the result. Planner keys include birth timezone and supplied-profile selected date, so inherited-profile month data does not collide with standalone month calculation. Inspect culture, selected-vs-now behaviour, calculation/calculator/house/hemisphere flags, person data, scope and navigation when changing cached paths. Controller OutputCache keys/custom variation are a separate layer; existing Global.asax `User` variation resolves Current.UserName. Preserve NoStore/private-member handling where used.

Reuse is the first step, not proof of constant cost. Inspect implementation and callers for repository queries, side effects, related-object hydration and external work. Prefer batch methods/options and load invariant preferences/templates/configuration outside loops. Extend the owning service if no appropriate batch exists, rather than duplicating its business rules in a controller or a new parallel service.

Source review did not execute the Windows/.NET Framework application, encrypted astronomy algorithms, external APIs, email delivery, payments or wkhtmltopdf. Validate changes with the existing relevant tests and local runtime; do not describe unexecuted checks as passing.
