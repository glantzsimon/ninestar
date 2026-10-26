# Combined predictions

## Scope and resource separation

The legacy predictions planner and its main month/day pills now show combined period readings. Generic prediction endpoints, the generic cycle-header partial, knowledge-base resources and Additional Cycles retain their previous behaviour. Moon phases remain separately calculated and displayed; they are not part of the combinations.

Personal and global interpretations are distinct. Personal daily readings use the existing 729 `Predictions/Combined/en-gb/<year>/<month>/Y-M-D.htm` files and `_Y_M_D` resource keys. There are 81 new monthly `Y-M.htm` files in those same folders, with keys `CombinedPrediction_Y_M`. Their text develops the year/month interactions in the daily readings using the richer personal Yearly and Monthly source material, excluding daily material. Numeric two-part keys such as `_5_5` already belong to numerology and must not be reused.

Global readings live separately under `GlobalPredictions/Combined/en-gb/<year>/<month>/`, with 81 monthly and 729 daily files and keys `GlobalCombinedPrediction_Y_M` / `GlobalCombinedPrediction_Y_M_D`. These develop the owner's existing GlobalPredictions Yearly, Monthly and Day material as collective/public themes with nested time scales and explicit year/month interactions. They are not aliases or copies of personal-house readings. Existing global source files are unchanged. The owner should review the new global editorial text as well as the implementation.

All new files have ResXFileRef entries, public resource accessors and Globalisation project Content registrations. ResourceManager handles resource lookup/culture fallback. Combined keys are calculated dynamically; fixed UI labels retain strongly typed resources.

## Editorial depth

All 81 monthly and 729 daily readings in each family use two substantial paragraphs. Monthly readings blend the enclosing year with the month's emotional, relational or collective emphasis, retaining concrete detail from the original generic sources. Personal daily readings lead with the day's character and practical guidance. Keep yearly/monthly influences as a short woven context for that day, rather than repeating the full yearly and monthly summaries first. Existing combination-specific daily guidance, including repeated/triple energies, is preserved. Global year/month combinations include individually worded interactions rather than relying only on a shared faster/slower bridge. Avoid padding through repetition, merely stacking generic descriptions, or shortening a reading to a single introductory sentence. Keep the original generic source files intact for their existing consumers.

## Concise subscription readings

Personal ICS feeds use separate CalendarCombinedPrediction_Y_M_D resources alongside the full daily files, named Y-M-D-calendar.txt. They lead their shorter paragraph with the daily influence and weave in concise year/month context. Two lists of three hyphen bullets precede it, with the day-specific item first where the list contains one item from each time scale. Template placeholders insert the shared localised headings IDEAL FOR and BE MINDFUL OF, with no blank line beneath a heading and one blank line between blocks. GetCalendarText in CombinedPredictionHelper performs validation, resource lookup and heading substitution. Website month/day readings and global resources keep their existing fuller text; the feed mode is selected only by GoogleCalendarService.GenerateCalendar. Split days keep a separate labelled afternoon reading.

## Shared lookup and UI path

`CombinedPrediction` is an immutable context containing year, month, optional morning/afternoon day numbers and `IsGlobal`. `Helpers/CombinedPredictionHelper.cs` is a static helper in the `K9.WebApplication.Helpers` namespace, rather than an injected application service. `CombinedPredictionHelper.GetHtml` selects the appropriate resource family. Distinct afternoon readings retain the existing resource-backed afternoon label. `GoogleCalendarService` reuses the full personal daily lookup and paragraph extraction for website entries; generated ICS feeds select the separate concise personal resource family. The numerical calculations, event identifiers and iCalendar escaping/folding are unchanged.

The planner's Year rows carry year/month context. Month rows carry year/month/day/afternoon context, and its parent carries year/month. The Day parent carries the daily context while hourly rows keep their generic descriptions. Both initial rendering and click responses select the combined description. Existing Year/generation/epoch descriptions stay generic.

`Predictions.GetCombinedMonthlyPredictions` and `GetCombinedDailyPredictions` are new endpoints. The latter keeps the existing separate lunar rendering. Their inputs are explicit numbers from the selected row plus display scope; they validate the number range. The old GetMonthlyPredictions/GetDailyPredictions and other generic actions are unchanged. Only planner URLs opt into the combined endpoints, including the initial My Personal Chart planner.

`PopulatePlanner` decorates request-owned UI planners after calculation, using the existing cached astronomy year/month batches and existing personal/global mapping methods to obtain enclosing period context. It does not recalculate a full profile per day, alter the numerical planner service or store rendered text in shared calculation templates. The Day parent uses one existing daily month batch to retain both morning and afternoon. Solar year/month boundaries come from astronomy, with a bounded retry for midnight on the Lichun date. Do not replace these periods with Gregorian month/year assumptions or derive enclosing houses from a reused profile's original selected-date snapshot.

Initial Year rendering can use its existing parent energy and month rows directly. Context objects are immutable, so the existing typed planner copies can safely retain their references. Combined HTML resolves in the current culture at display time. GlobalCycleEnergies now retains its already-calculated afternoon energy as Day2, matching the existing personal/lunar containers; no extra astronomy calculation is introduced.

The month/day sections in `_CyclesPills` use a new combined header partial while retaining the existing energy/element/season header and Bootstrap panel helpers. Personal solar, personal lunar and global panels each use their own energy containers and resource family. Core-cycle number links already target these pills and need no alternate client-side calculator. No Additional Cycles or knowledge-base view is changed.

## Validation and local checks

Static checks verify all new file/resource/accessor/project registrations, all 729 existing personal daily keys, unique keys, unchanged old resource entries and unchanged generic action bodies/numerical planner service. Node checks execute the actual Razor-script JavaScript with substituted model values and mocked AJAX to verify Simple/Advanced forwarding, separate alchemy routes/panels and personal/global month/day detail context including afternoon/date/energy values.

NineStarKiServiceTests add complete resource coverage and real-planner regressions for personal/global Year/Month/Day, previous/current/next navigation around February, and isolation of UI metadata from cached numerical templates. These C# tests require the Windows solution; no .NET Framework compiler, IIS or SwissEph runtime is available in the implementation environment.

Before merging, build and run the existing suite plus the new Combined* tests. Verify initial planner text and month/day clicks, switching Personal/Global, Year/Month/Day navigation across Lichun and solar-month boundaries, split days and both hemisphere/calculation options. Check the top primary-cycle numbers lead to combined month/day pills, including SolarAndLunarHouses. Verify lunar details stay separate and knowledge-base/Additional Cycles retain generic text. Compare a cold and warm planner request; no full profile should be calculated per displayed day.
