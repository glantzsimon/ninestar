using K9.DataAccessLayer.Models;
using NodaTime;
using System;

namespace K9.DataAccessLayer.Extensions
{
    public static class Extensions
    {
        public static ZonedDateTime ToZonedTime(this DateTimeOffset value, string timeZoneId)
        {
            var tz = DateTimeZoneProviders.Tzdb[timeZoneId];
            var instant = Instant.FromDateTimeOffset(value);
            return instant.InZone(tz);
        }

        public static ZonedDateTime ToZonedTime(this DateTime value, string timeZoneId)
        {
            var tz = DateTimeZoneProviders.Tzdb[timeZoneId];
            var instant = Instant.FromDateTimeOffset(value);
            return instant.InZone(tz);
        }

        public static DateTimeOffset? ToUserTimeZone(this TimeZoneBase model, DateTimeOffset value)
        {
            if (string.IsNullOrEmpty(model.UserTimeZone))
            {
                return null;
            }

            return value.ToZonedTime(model.UserTimeZone).ToDateTimeOffset();
        }

        public static DateTimeOffset? ToMyTimeZone(this TimeZoneBase model, DateTimeOffset value)
        {
            if (string.IsNullOrEmpty(model.MyTimeZone))
            {
                return null;
            }

            return value.ToZonedTime(model.MyTimeZone).ToDateTimeOffset();
        }

        public static DateTimeOffset? ToUserTimeZone(this TimeZoneBase model, DateTimeOffset? value)
        {
            if (string.IsNullOrEmpty(model.UserTimeZone) || !value.HasValue)
            {
                return null;
            }

            return value.Value.ToZonedTime(model.UserTimeZone).ToDateTimeOffset();
        }

        public static DateTimeOffset? ToMyTimeZone(this TimeZoneBase model, DateTimeOffset? value)
        {
            if (string.IsNullOrEmpty(model.MyTimeZone) || !value.HasValue)
            {
                return null;
            }

            return value.Value.ToZonedTime(model.MyTimeZone).ToDateTimeOffset();
        }

    }
}
