using HtmlAgilityPack;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Constants;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;

namespace K9.WebApplication.Services
{
    public class GoogleCalendarService : BaseService, IGoogleCalendarService
    {
        private const string SubscriptionPreferenceKey = "PersonalCalendarSubscriptionToken";
        private const string SubscriptionPurpose = "NineStarKi.PersonalCalendar.v1";
        private readonly INineStarKiService _nineStarKiService;
        private readonly IUserService _userService;

        public GoogleCalendarService(INineStarKiBasePackage my, INineStarKiService nineStarKiService, IUserService userService)
            : base(my)
        {
            _nineStarKiService = nineStarKiService;
            _userService = userService;
        }

        public List<CalendarEntry> GetCalendarEntries(int userId, DateTime startDate, DateTime endDate)
        {
            return GetCalendarEntries(userId, startDate, endDate, CancellationToken.None);
        }

        public List<CalendarEntry> GetCalendarEntries(int userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateUserId(userId);
            if (startDate.TimeOfDay != TimeSpan.Zero || endDate.TimeOfDay != TimeSpan.Zero)
                throw new ArgumentException("Supply calendar dates without a time component.");
            if (endDate < startDate)
                throw new ArgumentException("End date must be on or after start date.", nameof(endDate));

            var user = _userService.Find(userId);
            if (user == null)
                throw new ArgumentException("User not found.", nameof(userId));
            var info = My.UserInfosRepository.Find(e => e.UserId == userId).FirstOrDefault();
            if (info == null || user.BirthDate == default(DateTime))
                throw new InvalidOperationException("Birth details are required to generate a personal calendar.");

            var person = new PersonModel
            {
                Name = user.FullName,
                DateOfBirth = DateTime.SpecifyKind(user.BirthDate.Date.Add(info.TimeOfBirth), DateTimeKind.Unspecified),
                TimeOfBirth = info.TimeOfBirth,
                BirthTimeZoneId = info.BirthTimeZoneId,
                Gender = user.Gender
            };
            var timeZoneId = _userService.GetUserPreference(userId, SessionConstants.UserTimeZone, info.BirthTimeZoneId);
            if (string.IsNullOrWhiteSpace(timeZoneId))
                throw new InvalidOperationException("A calendar timezone must be saved before generating a calendar.");
            var calculationMethod = _userService.GetUserPreference(userId, SessionConstants.UserCalculationMethod, info.CalculationMethod);
            var calculatorType = _userService.GetUserPreference(userId, SessionConstants.DefaultCalculatorType, info.CalculatorType);
            var housesDisplay = _userService.GetUserPreference(userId, SessionConstants.UserHousesDisplay, info.HousesDisplay);
            var invertNatal = _userService.GetUserPreference(userId, SessionConstants.InvertDailyAndHourlyKiForSouthernHemisphere, info.InvertDailyAndHourlyKiForSouthernHemisphere);
            var invertCycles = _userService.GetUserPreference(userId, SessionConstants.InvertDailyAndHourlyCycleKiForSouthernHemisphere, info.InvertDailyAndHourlyKiForSouthernHemisphere);
            var descriptions = new Dictionary<string, string>();
            var entries = new List<CalendarEntry>();

            for (var date = startDate; date < endDate; date = date.AddDays(1))
            {
                cancellationToken.ThrowIfCancellationRequested();
                // Match Primary Cycles: supply the local selected date and its timezone
                // together. Converting midnight to UTC here can move the calculation
                // into the previous calendar date.
                var localDate = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
                var model = _nineStarKiService.CalculateNineStarKiProfile(person, today: localDate,
                    calculationMethod: calculationMethod, calculatorType: calculatorType, includeCycles: true,
                    userTimeZoneId: timeZoneId, housesDisplay: housesDisplay,
                    invertDailyAndHourlyKiForSouthernHemisphere: invertNatal,
                    invertDailyAndHourlyCycleKiForSouthernHemisphere: invertCycles,
                    displayDataForPeriod: EDisplayDataForPeriod.SelectedDate);
                cancellationToken.ThrowIfCancellationRequested();
                var houses = model.PersonalHousesOccupiedEnergies;
                var year = houses.Year.EnergyNumber;
                var month = houses.Month.EnergyNumber;
                var day = houses.Day.EnergyNumber;
                var afternoon = houses.Day2?.EnergyNumber;
                var description = GetDescription(year, month, day, descriptions);
                if (afternoon.HasValue && afternoon.Value != day)
                    description += $"\n\n{K9.Globalisation.Dictionary.AfternoonEnergyLabel} ({year}.{month}.{afternoon.Value}): " + GetDescription(year, month, afternoon.Value, descriptions);

                entries.Add(new CalendarEntry
                {
                    Date = localDate,
                    YearHouse = year,
                    MonthHouse = month,
                    DayHouse = day,
                    AfternoonDayHouse = afternoon.HasValue && afternoon.Value != day ? afternoon : null,
                    Summary = $"9Star · {year}.{month}.{day}",
                    Description = description
                });
            }
            return entries;
        }

        public string GenerateCalendar(int userId, DateTime startDate, DateTime endDate)
        {
            return ConvertToICalendar(userId, GetCalendarEntries(userId, startDate, endDate));
        }

        public DateTime GetCalendarToday(int userId)
        {
            ValidateUserId(userId);
            var info = My.UserInfosRepository.Find(e => e.UserId == userId).FirstOrDefault();
            var zone = _userService.GetUserPreference(userId, SessionConstants.UserTimeZone, info?.BirthTimeZoneId);
            if (string.IsNullOrWhiteSpace(zone))
                throw new InvalidOperationException("A calendar timezone must be saved before generating a calendar.");
            return DateTimeHelper.ConvertToLocaleDateTime(DateTime.UtcNow, zone).Date;
        }

        public string GetOrCreateSubscriptionToken(int userId)
        {
            ValidateUserId(userId);
            if (_userService.Find(userId) == null)
                throw new ArgumentException("User not found.", nameof(userId));
            var token = _userService.GetUserPreference<string>(userId, SubscriptionPreferenceKey);
            if (GetUserIdFromSubscriptionToken(token) == userId)
                return token;

            // Store the complete protected token so the member always receives the same URL.
            var payload = userId.ToString(CultureInfo.InvariantCulture) + "|" + Guid.NewGuid().ToString("N");
            token = HttpServerUtility.UrlTokenEncode(MachineKey.Protect(Encoding.UTF8.GetBytes(payload), SubscriptionPurpose));
            _userService.UpdateUserPreference(userId, SubscriptionPreferenceKey, token);
            return token;
        }

        public int? GetUserIdFromSubscriptionToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Length > 1024)
                return null;
            try
            {
                var bytes = HttpServerUtility.UrlTokenDecode(token);
                if (bytes == null)
                    return null;
                var payload = MachineKey.Unprotect(bytes, SubscriptionPurpose);
                if (payload == null)
                    return null;
                var parts = Encoding.UTF8.GetString(payload).Split('|');
                if (parts.Length != 2 || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var userId) || userId <= 0)
                    return null;
                var saved = _userService.GetUserPreference<string>(userId, SubscriptionPreferenceKey);
                return string.Equals(token, saved, StringComparison.Ordinal) && _userService.Find(userId) != null ? (int?)userId : null;
            }
            catch (CryptographicException) { return null; }
            catch (FormatException) { return null; }
            catch (ArgumentException) { return null; }
        }

        public void RevokeSubscription(int userId)
        {
            ValidateUserId(userId);
            _userService.UpdateUserPreference(userId, SubscriptionPreferenceKey, string.Empty);
        }

        public string ConvertToICalendar(int userId, List<CalendarEntry> entries)
        {
            ValidateUserId(userId);
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            if (entries.Any(e => e == null || e.Date.Date == DateTime.MaxValue.Date))
                throw new ArgumentException("Entries must have a date with a valid following day.", nameof(entries));
            if (entries.GroupBy(e => e.Date.Date).Any(e => e.Count() > 1))
                throw new ArgumentException("Only one calendar entry per date is supported.", nameof(entries));

            var calendar = new StringBuilder();
            var stamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
            AppendLine(calendar, "BEGIN:VCALENDAR");
            AppendLine(calendar, "VERSION:2.0");
            AppendLine(calendar, "PRODID:-//Nine Star Ki//Personal Calendar//EN");
            AppendLine(calendar, "CALSCALE:GREGORIAN");
            AppendLine(calendar, "X-WR-CALNAME:Nine Star Ki");
            foreach (var entry in entries.OrderBy(e => e.Date))
            {
                var date = entry.Date.Date;
                AppendLine(calendar, "BEGIN:VEVENT");
                AppendLine(calendar, "UID:" + userId.ToString(CultureInfo.InvariantCulture) + "-" + date.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "@ninestarki.app");
                AppendLine(calendar, "DTSTAMP:" + stamp);
                AppendLine(calendar, "DTSTART;VALUE=DATE:" + date.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
                AppendLine(calendar, "DTEND;VALUE=DATE:" + date.AddDays(1).ToString("yyyyMMdd", CultureInfo.InvariantCulture));
                AppendLine(calendar, "SUMMARY:" + EscapeText(entry.Summary));
                AppendLine(calendar, "DESCRIPTION:" + EscapeText(entry.Description));
                AppendLine(calendar, "TRANSP:TRANSPARENT");
                AppendLine(calendar, "END:VEVENT");
            }
            AppendLine(calendar, "END:VCALENDAR");
            return calendar.ToString();
        }

        private static string GetDescription(int year, int month, int day, Dictionary<string, string> descriptions)
        {
            var key = $"{year}-{month}-{day}";
            if (descriptions.TryGetValue(key, out var description))
                return description;
            var resourceKey = string.Format(CultureInfo.InvariantCulture, "_{0}_{1}_{2}", year, month, day);
            var html = K9.Globalisation.Dictionary.ResourceManager.GetString(resourceKey, K9.Globalisation.Dictionary.Culture);
            if (string.IsNullOrWhiteSpace(html))
                throw new InvalidOperationException($"Combined prediction resource not found: {resourceKey}");
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var paragraphs = document.DocumentNode.SelectNodes("//p");
            if (paragraphs == null)
                throw new InvalidOperationException($"Combined prediction has no paragraphs: {resourceKey}");
            description = string.Join("\n\n", paragraphs.Select(p => HtmlEntity.DeEntitize(p.InnerText).Trim()));
            descriptions.Add(key, description);
            return description;
        }

        private static void ValidateUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));
        }

        private static string EscapeText(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\r\n", "\n").Replace("\r", "\n")
                .Replace("\n", "\\n").Replace(";", "\\;").Replace(",", "\\,");
        }

        private static void AppendLine(StringBuilder calendar, string line)
        {
            // RFC 5545 limits physical lines to 75 UTF-8 octets, including the continuation space.
            var octets = 0;
            for (var index = 0; index < line.Length; index++)
            {
                var length = char.IsHighSurrogate(line[index]) && index + 1 < line.Length && char.IsLowSurrogate(line[index + 1]) ? 2 : 1;
                var count = Encoding.UTF8.GetByteCount(line.Substring(index, length));
                if (octets + count > 75)
                {
                    calendar.Append("\r\n ");
                    octets = 1;
                }
                calendar.Append(line, index, length);
                octets += count;
                index += length - 1;
            }
            calendar.Append("\r\n");
        }
    }
}
