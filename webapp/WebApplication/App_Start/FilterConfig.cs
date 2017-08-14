using System.Web.Mvc;
using K9.WebApplication.Filters;

namespace K9.WebApplication
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(new CultureAttribute());
		}
	}
}