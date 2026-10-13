# Calculation ownership and access-cache safeguards

## Applied changes

NineStarKiService keeps profile/planner calculation templates in the existing cache and returns request-owned copies. The typed copy methods preserve scalar/private state without invoking constructors, serializing computed getters or rerunning astronomy. They copy people, energy containers/energies, moon phases, directions and lists, planner rows, energy-summary wrappers, magic squares and value-tuple period arrays. A reference-identity context preserves repeated references and cycles (including profile/planner back-references). The lazy personal magic squares and directions-chart presentation are rebuilt against the copy.

Model getters that need internal rebinding now have private setters; they remain read-only to application callers. Shared MemoryCache infrastructure and static/read-only metadata/resource tables are deliberately retained. The generic CachableBase contract remains shared-reference caching; this ownership policy applies at the profile/planner service boundary, not to arbitrary repository entities or every cache in the application.

Profile creation copies the supplied PersonModel before retaining it. Planner cache misses copy a supplied profile before storing a template. Every response binds the current supplied profile's owned copy, so names, display flags and attached presentation never come from a previous cached caller. When no profile is supplied, session presentation choices (scope, energy display, directions visibility and known birth time) are refreshed on the returned copy; explicit calculation/timezone/house/hemisphere options remain unchanged. AstrologyService copies the numerical moon phase before adding culture/energy-specific description fields.

CompatibilityModel is a presentation graph with profiles, summaries and a mutable details/score graph. Rather than attempt to serialize or shallow-copy that graph, CalculateCompatibility now builds a fresh composite from cached profile copies on every call. The expensive profile/astronomy calculations remain cached; compatibility presentation construction runs each time. Both overloads retain their signatures and display-flag behaviour. Benchmark compatibility rendering locally to assess this cost.

One correction to the earlier review: GetCompatibilityTest contains a string-clearing loop, but the current CompatibilityModel string properties are read-only, so that loop currently changes none of them. Its existence illustrates intended mutation, not an observed string-clearing bug. Actual controller mutations of profile/planner state and mutable compatibility details justify the ownership change.

ReportsController.YearlyReport and ApiController.GetKnowledgeBase now use Duration=0, NoStore=true and Location=None. The knowledge-base action's explicit cache-enabling headers were removed. Each request resolves identity/access and executes its action, while underlying calculations remain cached. Report unique-ID access and the anonymous HTML route needed by PDF generation are unchanged. This does not audit or alter every other controller's OutputCache/culture policy.

ApiController.Validate has a membership-aware callback overload; the original parameterless callback delegates to it. Predictions and compatibility actions use the already validated membership for their user-info lookup. Invalid-key, invalid-account, insufficient-membership, administrator and account-null checks remain in the existing envelope. This reduces the three repeated account-code/membership lookups to one per valid predictions/compatibility request; it adds no process-wide membership cache.

## Deliberately retained behaviour

The six-digit account-code lookup still uses the existing conversion and first-match selection. A persisted/indexed lookup needs code-generation/collision review and a database migration; no new column, index, uniqueness rule or payment/membership side effect is introduced here.

Profiles still use the existing date/time/culture and calculation cache identities. These changes isolate mutable objects but do not fix all current-time/culture caching concerns. Static resource tables and generic repository entities retain their existing semantics. Cancellation still occurs between synchronous batches; it does not interrupt a shared calculation halfway through.

## Validation

Added regressions for scalar and nested profile changes, person/energy isolation, planner URL/list/profile isolation, cyclic graph copying, compatibility details/name isolation and lunar decoration. The previous display-flag regression now expects a fresh compatibility composite on repeated calls.

ApiCacheSafetyTests exercise the validation envelope's invalid-key/account/permissions and successful membership callback, asserting one lookup and preference access only after validation. They assert NoStore settings on the two sensitive actions. These are unit/structural checks, not a substitute for the IIS pipeline.

Static checks cover XML/registrations, option/signature preservation, typed-copy fields, unchanged validation error branches, removal of explicit cache-enabling headers and no protected-file edits. No .NET Framework compiler/runtime, IIS or SQL Server is configured in the implementation environment: the Windows build and C# tests remain unexecuted.

Before merging: run NineStarKiServiceTests, ApiCacheSafetyTests, CachableBaseTests, ServicePerformanceTests and GoogleCalendarServiceTests. On IIS, request reports as two different signed-in users with userId omitted; compare identities. Repeat knowledge-base calls with valid/invalid API keys and after membership revocation. Check chart/compatibility/planner JSON, graph display, moon descriptions, profile/planner URL updates and PDF generation. Compare cold/warm calendar and compatibility response times to catch any presentation-copy cost. No deployment is triggered by these commits.

