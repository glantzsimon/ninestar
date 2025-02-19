using K9.Base.WebApplication.Filters;
using K9.WebApplication.Services;
using System.Web.Mvc;

namespace K9.WebApplication
{
    public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{

#if !DEBUG
			filters.Add(new CustomExceptionFilter());
    #endif

		    filters.Add(new CultureAttribute());
		    filters.Add(new ContentLoaderAttribute());
        }
	}
}