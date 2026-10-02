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

One all-day, transparent event is produced per date. The title is `9Star · 7.1.5`. When the chart returns a different afternoon house, `AfternoonDayHouse` records it and the description includes its combined prediction. The primary triple uses the Primary Cycles selected-date calculation: local calendar midnight (`DateTimeKind.Unspecified`) and the saved timezone are passed together, without first converting the date to UTC. Birth date and saved birth time are combined once and passed with the birth timezone. On dates with an intraday year/month transition, this is a daily snapshot rather than a timed transition event.

Any date range is supported. A January-to-December export still uses the existing Nine Star Ki calculation for year and month boundaries. No February boundary is hardcoded here. For a Nine Star Ki year export, supply the dates determined by the existing astronomy service.

The existing 729 combined descriptions are loaded through `K9.Globalisation.Dictionary.ResourceManager`, using your `Dictionary.resx` keys such as `_1_2_3` and the existing culture selection. No separate embedding rule or resource collection is added. Calendar text uses CRLF, RFC 5545 text escaping and UTF-8-aware 75-octet line folding. Event IDs remain stable for a member/date; the subscription URL should remain stable as well. The feed returns UTF-8 `text/calendar` content.

My Personal Chart now has a calendar panel beneath the current houses. It provides a Gregorian month grid using the existing energy images and `MediaService.BaseImagesPath`, month navigation, Today, selected-day combined text, help and a full-browser-window view. The expanded view closes with its button or Escape and keeps keyboard focus inside it. All fixed labels, messages, help and weekday names use strongly typed `Dictionary` properties backed by the Globalisation resource file; existing matching entries are reused. Only the calculated combined-prediction key uses a dynamic resource lookup. Date and month formatting use the request culture. The controls use the existing Bootstrap button helpers and the resource-labelled URL display template; dynamic day buttons clone a helper-rendered template. Navigation and Today occupy their natural width on the left. A flexible gap and padded vertical divider separate them from Copy calendar link, Expand and Help on the right. Copy and Expand are icon-only in narrow calendars, with resource-backed accessible labels and tooltips; the optional link-disable icon appears in the same action group. The toolbar wraps within the top area if space is insufficient. The selected-day separator has 16px above and below it, with the heading top margin removed. All controls are at the top; the bottom subscription button and its spacing are removed. A standalone anti-forgery token preserves the POST checks, and the resource-labelled URL display remains available for manual copying after a link is requested. Empty status and trailing paragraph margins do not add space. The expanded desktop layout separates grid and details into columns; its horizontal separator returns in the stacked mobile view. Year and nine-year navigation remain future work.

`PersonalCalendarController` provides authenticated month data and anti-forgery-protected subscription/revocation POST actions. The anonymous feed requires a valid private token and checks the existing paid predictions access (or administrator status) on every request. UI access uses the existing predictions paywall. Tokens are protected by ASP.NET MachineKey and stored in existing UserPreference records, so no database migration is required. The copied URL remains stable until revoked; disabling it makes the previous URL unusable. A new URL needs a new subscription in Google Calendar. The feed covers one local month behind today through twelve months ahead and reads saved calculation preferences each time. The subscriber's UI language is saved when the link is obtained. OAuth-based Google Calendar writes are not implemented.

Deployment: use a stable ASP.NET machine key across app restarts and any web-farm instances, as for forms authentication. Changing that key invalidates existing subscription links. The feed URL must be reachable from the internet; a localhost URL cannot be subscribed to by Google. Add the URL from Google Calendar on a computer via Other calendars > + > From URL. Do not log or share private subscription URLs.

The chart model now accepts an optional explicit calculator type. `NineStarKiService` passes its existing argument through before houses are calculated, allowing a calendar request to use the member's saved mode. Direct constructor callers that omit the argument retain existing behavior.

## Validation

`GoogleCalendarServiceTests` covers leap-day end dates, personal event IDs, event ordering, text escaping, Unicode folding, invalid input, local-date/birth-time/preference forwarding across timezones and daylight-saving dates, calculator mode and all 729 existing description resources. Further tests cover token stability, user binding, tampering, revocation, malformed tokens, feed access checks and the rolling date window. `NineStarKiServiceTests.OctoberFirst2026PersonalSolarHousesMatchPrimaryCycles` adds the reported 7.7.7 regression using the real astronomy service at midnight and noon. Run the test project on Windows with the existing solution dependencies restored. The implementation environment has no .NET Framework compiler or test runner; these C# tests have not been executed there. AstronomyService is encrypted in the repository and this change does not modify it.

## Review locally

```bash
git fetch origin
git checkout -b feature/personal-calendar-entries origin/feature/personal-calendar-entries
```

Open the existing solution in Visual Studio, restore packages, build and run `GoogleCalendarServiceTests`. Merge the review pull request after checking it; then switch to `master` and pull to obtain the merged change.


## UI checks on Windows

1. Sign in with paid predictions access and open My Personal Chart. Confirm the panel sits below the current houses, and today's date is selected in the saved timezone.
2. Compare a selected day's houses and combined description with the existing predictions calculator, including a split morning/afternoon day and dates near February and monthly solar-term changes.
3. Navigate across December/January, inspect February in a leap year, and use Today to return.
4. Expand and close on desktop and mobile. Check that the expanded calendar fills the browser window, Escape closes it, focus returns and the rest of the page does not scroll behind it.
5. Open help, copy the link, check the manual-copy input if the browser denies clipboard access, and subscribe from Google Calendar on a computer. Opening the copied URL without login should return ICS rather than a login page.
6. Disable the link and confirm it returns 404. Copy again and confirm a different link is produced. Confirm unpaid users cannot access month data, create a subscription or fetch a paid feed.

JavaScript syntax, resource-key uniqueness and all new resource references, project XML and unique file registrations were checked locally. The Playwright harness could not run because this environment has no browser executable; visual behaviour and actual Google refresh still need local/live verification. No .NET Framework build or C# test execution was possible here.
