# Personal calendar service

`IGoogleCalendarService` follows the existing `BaseService` / package / Autofac structure.

```csharp
// Member-local calendar dates: inclusive start, exclusive end.
var startDate = new DateTime(2027, 1, 1);
var endDate = new DateTime(2028, 1, 1);
var entries = googleCalendarService.GetCalendarEntries(userId, startDate, endDate);
var ics = googleCalendarService.ConvertToICalendar(userId, entries);

// Or perform both steps:
var calendar = googleCalendarService.GenerateCalendar(userId, startDate, endDate);
```

`CalendarEntry` contains the local date, personal occupied houses, compact summary and plain-text combined prediction. It can also be used by a future website calendar or report. The member must have birth details and a saved calendar timezone (birth timezone is the default). Saved calculation, calculator, house and hemisphere preferences are read from the database, without relying on the requesting browser's session.

One all-day, transparent event is produced per date. The title is `9Star · 7.1.5`. When the chart returns a different afternoon house, `AfternoonDayHouse` records it and the description includes its combined prediction. The primary triple uses the houses at the start of the local day. On dates with an intraday year/month transition, this is a daily snapshot rather than a timed transition event.

Any date range is supported. A January-to-December export still uses the existing Nine Star Ki calculation for year and month boundaries. No February boundary is hardcoded here. For a Nine Star Ki year export, supply the dates determined by the existing astronomy service.

The existing 729 combined descriptions are loaded through `K9.Globalisation.Dictionary.ResourceManager`, using your `Dictionary.resx` keys such as `_1_2_3` and the existing culture selection. No separate embedding rule or resource collection is added. Calendar text uses CRLF, RFC 5545 text escaping and UTF-8-aware 75-octet line folding. Event IDs remain stable for a member/date; the subscription URL should remain stable as well. Return ICS as UTF-8 with `text/calendar; charset=utf-8` when the controller is added.

This change implements data generation and ICS serialization. A public subscription endpoint, revocable unguessable member token, entitlement checks and My Account buttons are subsequent work. The future endpoint must authorize access; the `userId` service parameter is not an access credential. OAuth-based Google Calendar writes are not implemented.

The chart model now accepts an optional explicit calculator type. `NineStarKiService` passes its existing argument through before houses are calculated, allowing a calendar request to use the member's saved mode. Direct constructor callers that omit the argument retain existing behavior.

## Validation

`GoogleCalendarServiceTests` covers leap-day end dates, personal event IDs, event ordering, text escaping, Unicode folding, invalid input, timezone conversion, calculator mode and packaging all 729 descriptions. Run the test project on Windows with the existing solution dependencies restored. The implementation environment has no .NET Framework compiler or test runner; these C# tests have not been executed there. AstronomyService is encrypted in the repository and this change does not modify it.

## Review locally

```bash
git fetch origin
git checkout -b feature/personal-calendar-entries origin/feature/personal-calendar-entries
```

Open the existing solution in Visual Studio, restore packages, build and run `GoogleCalendarServiceTests`. Merge the review pull request after checking it; then switch to `master` and pull to obtain the merged change.
