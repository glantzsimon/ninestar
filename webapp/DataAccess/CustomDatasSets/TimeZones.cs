using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace K9.DataAccessLayer.Database
{
    public class TimeZones : ICustomDataSet<TimeZone>
    {
        public string Key => TimeZone.StandardTimeZones;

        public List<ListItem> GetData(DbContext db)
        {
            var values = db.Set<TimeZone>().Where(e => !string.IsNullOrEmpty(e.DisplayName))
                .OrderBy(e => e.DisplayOrder).ToList();
            return new List<ListItem>(values.Select(e =>
            {
                return new ListItem(e.Id, e.DisplayName, e.TimeZoneId);
            })).Distinct().ToList();
        }
    }
}
