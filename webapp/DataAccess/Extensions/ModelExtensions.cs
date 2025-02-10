using K9.DataAccessLayer.Models;
using System;

namespace K9.DataAccessLayer.Extensions
{
    public static class ModelExtensions
    {

        public static DateTimeOffset? ToUserTimeZone(this TimeZoneBase model, DateTimeOffset value)
        {
            if (string.IsNullOrEmpty(model.UserTimeZone))
            {
                return null;
            }

            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(model.UserTimeZone);
            return TimeZoneInfo.ConvertTime(value, userTimeZone);
        }

        public static DateTimeOffset? ToMyTimeZone(this TimeZoneBase model, DateTimeOffset value)
        {
            if (string.IsNullOrEmpty(model.MyTimeZone))
            {
                return null;
            }

            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById(model.MyTimeZone);
            return TimeZoneInfo.ConvertTime(value, myTimeZone);
        }

        public static DateTimeOffset? ToUserTimeZone(this TimeZoneBase model, DateTimeOffset? value)
        {
            if (string.IsNullOrEmpty(model.UserTimeZone) || !value.HasValue)
            {
                return null;
            }

            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(model.UserTimeZone);
            return TimeZoneInfo.ConvertTime(value.Value, userTimeZone);
        }

        public static DateTimeOffset? ToMyTimeZone(this TimeZoneBase model, DateTimeOffset? value)
        {
            if (string.IsNullOrEmpty(model.MyTimeZone) || !value.HasValue)
            {
                return null;
            }

            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById(model.MyTimeZone);
            return TimeZoneInfo.ConvertTime(value.Value, myTimeZone);
        }

    }
}
