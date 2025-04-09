using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace K9.DataAccessLayer.Database
{
    public class TimeZonesExtended : ICustomDataSet<TimeZone>
    {
        public string Key => TimeZone.ExtendedTimeZones;

        public List<ListItem> GetData(DbContext db)
        {
            var values = db.Set<TimeZone>().OrderBy(e => e.FriendlyName).ToList();
            return values
                .GroupBy(e => e.FriendlyName)
                .Select(g => new ListItem(g.First().Id, g.Key, g.First().TimeZoneId))
                .ToList();
        }
    }
}
