using K9.Base.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations.Schema;
using K9.SharedLibrary.Helpers;

namespace K9.DataAccessLayer.Models
{
    public class TimeZoneBase : ObjectBase
	{
	    [NotMapped]
	    public string UserTimeZone { get; set; }

	    [NotMapped]
	    public string MyTimeZone { get; set; }

	    public string TimeZoneDisplayText => TemplateProcessor.PopulateTemplate(
	        Globalisation.Dictionary.LocalTimeDisplay,
	        new
	        {
	            TimeZone = UserTimeZone
	        });
	}
}
