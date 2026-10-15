# Combined predictions

## Scope and resource separation

The legacy predictions planner and its main month/day pills now show combined period readings. Generic prediction endpoints, the generic cycle-header partial, knowledge-base resources and Additional Cycles retain their previous behaviour. Moon phases remain separately calculated and displayed; they are not part of the combinations.

Personal and global interpretations are distinct. Personal daily readings retain the existing 729 `Predictions/Combined/en-gb/<year>/<month>/Y-M-D.htm` files and `_Y_M_D` resource keys. There are 81 new monthly `Y-M.htm` files in those same folders, with keys `CombinedPrediction_Y_M`. Their text reuses the year/month introductions in the existing daily readings, excluding the daily material. Numeric two-part keys such as `_5_5` already belong to numerology and must not be reused.

Global readings live separately under `GlobalPredictions/Combined/en-gb/<year>/<month>/`, with 81 monthly and 729 daily files and keys `GlobalCombinedPrediction_Y_M` / `GlobalCombinedPrediction_Y_M_D`. These are summaries of the owner's existing GlobalPredictions Yearly, Monthly and Day material, expressed as collective/public themes with nested time scales. They are not aliases or copies of personal-house readings. Existing global source files are unchanged. The owner should review the new global editorial text as well as the implementation.

All new files have ResXFileRef entries, public resource accessors and Globalisation project Content registrations. ResourceManager handles resource lookup/culture fallback. Combined keys are calculated dynamically; fixed UI labels retain strongly typed resources.

## Shared lookup and UI path

`CombinedPrediction` is an immutable context containing year, month, optional morning/afternoon day numbers and `IsGlobal`. `Helpers/CombinedPredictionHelper.cs` is a static helper in the `K9.WebApplication.Helpers` namespace, rather than an injected application service. `CombinedPredictionHelper.GetHtml` selects the appropriate resource family. Distinct afternoon readings retain the existing resource-backed afternoon label. `GoogleCalendarService` reuses the personal daily lookup and continues extracting paragraphs for plain-text ICS/UI entries; its calculations and output format are unchanged.

The planner's Year rows carry year/month context. Month rows carry year/month/day/afternoon context, and its parent carries year/month. The Day parent carries the daily context while hourly rows keep their generic descriptions. Both initial rendering and click responses select the combined description. Existing Year/generation/epoch descriptions stay generic.

`Predictions.GetCombinedMonthlyPredictions` and `GetCombinedDailyPredictions` are new endpoints. The latter keeps the existing separate lunar rendering. Their inputs are explicit numbers from the selected row plus display scope; they validate the number range. The old GetMonthlyPredictions/GetDailyPredictions and other generic actions are unchanged. Only planner URLs opt into the combined endpoints, including the initial My Personal Chart planner.

`PopulatePlanner` decorates request-owned UI planners after calculation, using the existing cached astronomy year/month batches and existing personal/global mapping methods to obtain enclosing period context. It does not recalculate a full profile per day, alter the numerical planner service or store rendered text in shared calculation templates. The Day parent uses one existing daily month batch to retain both morning and afternoon. Solar year/month boundaries come from astronomy, with a bounded retry for midnight on the Lichun date. Do not replace these periods with Gregorian month/year assumptions or derive enclosing houses from a reused profile's original selected-date snapshot.

Initial Year rendering can use its existing parent energy and month rows directly. Context objects are immutable, so the existing typed planner copies can safely retain their references. Combined HTML resolves in the current culture at display time. GlobalCycleEnergies now retains its already-calculated afternoon energy as Day2, matching the existing personal/lunar containers; no extra astronomy calculation is introduced.

The month/day sections in `_CyclesPills` use a new combined header partial while retaining the existing energy/element/season header and Bootstrap panel helpers. Personal solar, personal lunar and global panels each use their own energy containers and resource family. Core-cycle number links already target these pills and need no alternate client-side calculator. No Additional Cycles or knowledge-base view is changed.

## Validation and local checks

Static checks verify all new file/resource/accessor/project registrations, all 729 existing personal daily keys, unique keys, unchanged old resource entries and unchanged generic action bodies/numerical planner service. Node checks execute the actual Razor-script JavaScript with substituted model values and mocked AJAX to verify Simple/Advanced forwarding, separate alchemy routes/panels and personal/global month/day detail context including afternoon/date/energy values.

NineStarKiServiceTests add complete resource coverage and real-planner regressions for personal/global Year/Month/Day, previous/current/next navigation around February, and isolation of UI metadata from cached numerical templates. These C# tests require the Windows solution; no .NET Framework compiler, IIS or SwissEph runtime is available in the implementation environment.

Before merging, build and run the existing suite plus the new Combined* tests. Verify initial planner text and month/day clicks, switching Personal/Global, Year/Month/Day navigation across Lichun and solar-month boundaries, split days and both hemisphere/calculation options. Check the top primary-cycle numbers lead to combined month/day pills, including SolarAndLunarHouses. Verify lunar details stay separate and knowledge-base/Additional Cycles retain generic text. Compare a cold and warm planner request; no full profile should be calculated per displayed day.
