# Service and controller performance review

Reviewed branch snapshot: `abc4f7407c34d10e131ab286b7f8514073c570a1` (13 October 2026).

## Scope and evidence

Static triage covered all 109 unencrypted C# files under Services and Controllers: 62 service/interface/support files and 47 controller files, approximately 15,025 lines. The higher-cost calculation, list, booking, membership, article, account, email, report, media and API paths were inspected in more detail with relevant models and interfaces. The remaining generic CRUD controllers mostly delegate to K9. AstronomyService/IAstronomyService and configuration source protected by git-crypt were excluded from repository plaintext review. The earlier owner-supplied astronomy revision remains the basis for its caching documentation.

This is a source review, not a live benchmark. Repository-call counts below describe the code path; database round trips, EF lazy-loading, execution plans, request latency and external-service costs require Windows/runtime measurement.

## Changes applied

| Path | Previous work | Result |
| --- | --- | --- |
| MailingListService.GetMailingListListItems | Called ListAll, hydrating every saved mailing list and all automatic recipient lists merely to build selector items. | One saved-list metadata query plus automatic list metadata. No user, list-user or membership queries. Same automatic-first ordering, IDs and names. |
| MailingListService.Find for system IDs | Built all saved/automatic lists, then selected one. | Builds only the requested automatic list. It still loads that list's users even when includeUsers is false, matching the existing public behaviour. Unknown automatic IDs still return null. |
| ConsultationService.GetAllSlotsAndBookings | Per slot: lookup consultation by SlotId, reload it by ID, reload the same selected consultation's UserConsultation and its user. | One batch consultation query for the selected slots, one selected-booking lookup and at most one user lookup. Existing slot eligibility, admin filtering, week boundaries, selected-consultation binding and timezone decoration are retained. Empty grids skip hydration queries. |
| UserMembershipsController.Index | One membership list plus a user and option lookup per row (1 + 2N explicit repository calls). | Membership list plus two batch queries (3 for non-empty lists, 1 when empty). Active/start-date sorting and missing-reference null behaviour are retained. |
| PersonalChartController.MyProfile | Created an unused PredictionsViewModel, fetching the summary collection solely for that discarded wrapper. | Removes that unused construction/read. The chart and planner still use the existing profile path. |
| NineStarKiService.CalculateCompatibility | Cache key omitted isHideSexuality, so alternating display flags reused the first result. | Includes the flag. Existing calculation/model construction is unchanged. This is a cache correctness fix, not a claimed latency improvement. |

No new process-wide caching is added for memberships, bookings, recipients, or account data. These reads remain fresh per operation. No booking writes, payment/upgrade checks, complementary-reading accounting, mail sends, routes or authorisation filters are changed. Queries still use the existing repository interfaces and service/controller injection pattern.

## Prioritised follow-ups

| Priority | Finding | Next safe step |
| --- | --- | --- |
| High | Cached NineStarKiModel/CompatibilityModel/PlannerViewModel instances are shared and callers mutate them. Predictions/PersonalChart attach URLs/planner state; ApiController.GetCompatibilityTest clears string fields on a cached compatibility result. | Define ownership/clone contracts or separate immutable calculation data from request presentation. Add cross-request tests before changing model graphs. |
| High | ReportsController.YearlyReport caches by userId for seven days, but null userId resolves Current.UserId. ApiController.GetKnowledgeBase output caching can bypass validation on a cache hit. Culture is absent from several caches. | Audit output-cache identity/culture and access behaviour with signed-in/signed-out, different users/languages and revoked membership tests. Avoid extending page-cache durations. |
| High | ApiController predictions/compatibility paths look up membership in Validate, again in the action and again in GetUserInfo. MembershipService.GetActiveUserMembership(accountNumber) scans all membership names to resolve a six-digit code. | Pass the validated membership through the existing validation envelope; then design a collision-safe indexed account-code lookup. Verify all invalid-key/account/permission responses before changing this path. |
| Medium | ArticlesService.GetArticles fetches tags/author per article. GetDashboardViewModel reloads each comment's article and user/userinfo/role/likes. UserService.GetConsultations hydrates consultation/user per row. | Batch related data for the selected page and retain approval/owner/role filters. Measure repository calls and add fixtures with repeated authors/articles/users. |
| Medium | MailingListService.ListAll still expands saved-list recipients per list and recomputes overlapping paid-member sets for automatic lists. EmailQueueService hydrates users/contacts per email and membership options per membership. | Reuse request/job-local snapshots, batch distinct IDs and keep unsubscribe/upgrade checks fresh at the existing send stage. Use job tests; do not cache mailing eligibility across jobs. |
| Medium | NumerologyService.Calculate calls monthly planner generation through CurrentMonth, MonthlyPlannerCodes and forecast construction. | Reuse the calculated collections within one operation, preserving date rollover, offsets and resource formatting. Arithmetic is cheap; profile before adding a cache. |
| Medium | Full profile cache keys with today=null or Now mode can store time-dependent output for 30 days. Hour/minute-rounded astronomy/profile keys can collide around transitions; date-only birth time is precombined in some controller planner calls. | Specify effective time and input precision contracts, with midnight/DST/solar-term/birth-time regression cases. Do not normalise whole profiles to a year or alter boundary arithmetic as a performance shortcut. |
| Medium | MediaService creates HttpClient per health check and overlapping checks can duplicate requests; scheduled synchronous waits need context-aware review. AIService/MailChimp make external calls; PdfService launches wkhtmltopdf. | Measure external elapsed time; reuse appropriately scoped clients, deduplicate health checks and introduce timeouts/concurrency only with fake-service/process tests. |
| Medium | PdfService reads redirected stderr then stdout synchronously. A child filling stdout while stderr is being drained can block. | Drain both streams concurrently and test large-output/failure/timeout cases with a controlled child process before changing production process handling. |
| Low | Generic admin lists/log views are unbounded and some tag/contact paths load whole tables before filtering. | Add pagination/query-level filtering where UI contracts allow it. Do not parallelise EF repositories sharing a DbContext. |

Payment, membership creation, contact matching, account activation/OTP, queue/send rules and random I Ching generation have side effects or changing eligibility. Their results should not be blanket-cached. Small wrappers/CRUD controllers offer little evidence for additional caching.

## Validation and limitations

Added ServicePerformanceTests with strict repositories to reject recipient expansion in selectors and unrelated list expansion, and to assert single consultation/selected-booking/user hydration queries, empty-grid handling and unchanged booking/timezone mapping. Added a real-service compatibility display-flag cache regression. Existing astronomy-dependent fixtures need their Windows ephemeris configuration.

Static review verifies nullable SlotId filtering, unchanged public service signatures, retained controller authorisation and unique test project registration. No .NET Framework compiler/runtime or configured SQL Server/IIS is available here, so these C# tests and live query/latency measurements have not run. The booking and admin membership pages require local smoke checks.

Run ServicePerformanceTests, NineStarKiServiceTests, MembershipServiceTests and GoogleCalendarServiceTests in Visual Studio. Smoke-check the mailing selector/system-list send preview (without sending), empty/populated booking grids and membership admin ordering. Measure query counts on representative small and large datasets; performance gains must be measured rather than inferred as fixed milliseconds.
