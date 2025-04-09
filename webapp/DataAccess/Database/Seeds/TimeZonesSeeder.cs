using System;
using System.Data.Entity;
using System.Linq;
using TimeZoneConverter;
using TimeZone = K9.DataAccessLayer.Models.TimeZone;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class TimeZonesSeeder
    {
        public static void Seed(DbContext context)
        {
            var timeZones = TZConvert.KnownIanaTimeZoneNames.ToList();

            foreach (var timeZone in timeZones)
            {
                string windowsId = "";
                TimeZoneInfo tz = null;
                string friendlyName = timeZone;

                try
                {
                    windowsId = TZConvert.IanaToWindows(timeZone);
                    tz = TimeZoneInfo.FindSystemTimeZoneById(windowsId);
                    friendlyName = tz.DisplayName;
                }
                catch (Exception e)
                {
                }
                
                var timeZoneInfo = TimeZone.GetTimeZoneInfo(timeZone);

                AddOrEditTimeZone(context, timeZone, windowsId, friendlyName, timeZoneInfo?.DisplayName,
                    timeZoneInfo?.Abbreviation, timeZoneInfo?.UTCOffset, timeZoneInfo?.DisplayOrder);
            }

            context.SaveChanges();
        }

        private static void AddOrEditTimeZone(DbContext context, string timeZoneId, string windowsTimeZoneId, string friendlyName, string displayName,
            string abbreviation, string utcOffset, int? displayOrder)
        {
            var entity = context.Set<TimeZone>().FirstOrDefault(e => e.TimeZoneId == timeZoneId);

            if (entity == null)
            {
                context.Set<TimeZone>().Add(new TimeZone
                {
                    TimeZoneId = timeZoneId,
                    WindowsTimeZoneId = windowsTimeZoneId,
                    FriendlyName = friendlyName,
                    DisplayName = displayName,
                    Abbreviation = abbreviation,
                    UTCOffset = utcOffset,
                    DisplayOrder = displayOrder
                });
            }
            else
            {
                entity.TimeZoneId = timeZoneId;
                entity.WindowsTimeZoneId = windowsTimeZoneId;
                entity.FriendlyName = friendlyName;
                entity.DisplayName = displayName;
                entity.Abbreviation = abbreviation;
                entity.UTCOffset = utcOffset;
                entity.DisplayOrder = displayOrder;
                context.Entry(entity).State = EntityState.Modified;
            }
        }

    }
}
