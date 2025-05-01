using System;
using System.Linq;
using TimeZoneConverter;

namespace K9.SharedLibrary.Helpers
{
    public class DateTimeHelper
    {

        public static DateTime ConvertToLocaleDateTime(DateTime rawdateTime, string timeZoneId)
        {
            rawdateTime = DateTime.SpecifyKind(rawdateTime, DateTimeKind.Utc);
            TimeZoneInfo tz = TZConvert.GetTimeZoneInfo(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(rawdateTime, tz);
        }

        public static string ResolveTimeZone(string userInput)
        {
            string input = userInput.Trim().ToLowerInvariant();
            var allTimezones = TZConvert.KnownIanaTimeZoneNames;

            // Try to find exact match by location part
            var match = allTimezones
                .FirstOrDefault(tz => tz.ToLowerInvariant().EndsWith("/" + input) ||
                                      tz.ToLowerInvariant().Contains("/" + input));

            if (match != null)
                return match;

            // If not found, try a softer match (anywhere in string)
            match = allTimezones
                .FirstOrDefault(tz => tz.ToLowerInvariant().Contains(input));

            return match ?? "";
        }

        public static TimeSpan ParseTime(string time)
        {
            return string.IsNullOrEmpty(time) ? new TimeSpan() : TimeSpan.Parse(time.Replace("-", ":"));
        }
    }
}
