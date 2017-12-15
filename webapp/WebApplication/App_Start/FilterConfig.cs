using K9.Base.WebApplication.Filters;
using System.Web.Mvc;

namespace K9.WebApplication
{
    public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(new CultureAttribute());
		    filters.Add(new ContentLoaderAttribute());
        }
	}
}