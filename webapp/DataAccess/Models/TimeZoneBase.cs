using K9.Base.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    public class TimeZoneBase : ObjectBase
	{
	    [NotMapped]
	    public string UserTimeZone { get; set; }

	    [NotMapped]
	    public string MyTimeZone { get; set; }
	}
}
