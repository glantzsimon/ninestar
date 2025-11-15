using System;
using System.Linq;
using TimeZoneConverter;

namespace K9.SharedLibrary.Helpers
{
    public class DateTimeHelper
    {

        public static string ValidateTimeZoneId(string value)
        {
            switch (value)
            {
                case "Africa/Juba":
                    return "E. Africa Standard Time";

                case "America/Dawson":
                case "America/Whitehorse":
                case "Canada/Yukon":
                    return "Pacific Standard Time";

                default:
                    return value;
            }
        }

        public static DateTime ConvertToLocaleDateTime(DateTime rawdateTime, string timeZoneId)
        {
            rawdateTime = DateTime.SpecifyKind(rawdateTime, DateTimeKind.Utc);
            TimeZoneInfo tz;
            try
            {
                tz = TZConvert.GetTimeZoneInfo(ValidateTimeZoneId(timeZoneId));
            }
            catch
            {
                // fallback to GMT if unknown
                tz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            }
            return TimeZoneInfo.ConvertTimeFromUtc(rawdateTime, tz);
        }

        public static DateTime ConvertToUT(DateTime rawDateTime, string timeZoneId)
        {
            if (rawDateTime.Kind == DateTimeKind.Utc)
                return rawDateTime;

            if (string.IsNullOrEmpty(timeZoneId))
                return DateTime.SpecifyKind(rawDateTime, DateTimeKind.Utc);

            TimeZoneInfo tz;
            try
            {
                tz = TZConvert.GetTimeZoneInfo(ValidateTimeZoneId(timeZoneId));
            }
            catch
            {
                // fallback to GMT if unknown
                tz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            }

            DateTime localTime = DateTime.SpecifyKind(rawDateTime, DateTimeKind.Unspecified);

            // adjust if invalid due to DST transitions
            if (tz.IsInvalidTime(localTime))
                localTime = localTime.AddHours(1);

            try
            {
                return TimeZoneInfo.ConvertTimeToUtc(localTime, tz);
            }
            catch
            {
                // ultimate fallback
                return DateTime.SpecifyKind(rawDateTime, DateTimeKind.Utc);
            }
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
