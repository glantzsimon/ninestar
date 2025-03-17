using K9.DataAccessLayer.Models;
using System.Data.Entity;
using System.Linq;
using TimeZoneConverter;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class TimeZonesSeeder
    {
        public static void Seed(DbContext context)
        {
            var timeZones = TZConvert.KnownIanaTimeZoneNames.ToList();

            foreach (var timeZone in timeZones)
            {
                AddOrEditTimeZone(context, timeZone);
            }

            context.SaveChanges();
        }

        private static void AddOrEditTimeZone(DbContext context, string timeZoneId)
        {
            var entity = context.Set<TimeZone>().FirstOrDefault(e => e.TimeZoneId == timeZoneId);
            
            if (entity == null)
            {
                context.Set<TimeZone>().Add(new TimeZone
                {
                    TimeZoneId = timeZoneId
                });
            }
            else
            {
                entity.TimeZoneId = timeZoneId;
                context.Entry(entity).State = EntityState.Modified;
            }
        }

    }
}
