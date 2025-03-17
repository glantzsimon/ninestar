using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.TimeZones, PluralName = Globalisation.Strings.Names.TimeZones, Name = Globalisation.Strings.Names.TimeZone)]
    public class TimeZone : ObjectBase
	{
	    public string TimeZoneId { get; set; }
	}
}
