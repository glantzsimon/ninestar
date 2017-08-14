using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace K9.WebApplication.Filters
{
	public class CultureAttribute : ActionFilterAttribute
	{
		
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (
				filterContext.RequestContext != null && 
				filterContext.RequestContext.HttpContext != null &&
				filterContext.RequestContext.HttpContext.Session != null)
			{
				try
				{
					var languageCode = filterContext.RequestContext.HttpContext.Session["languageCode"].ToString();
					if (!string.IsNullOrEmpty(languageCode))
					{
						var culture = CultureInfo.GetCultureInfo(languageCode);
						Thread.CurrentThread.CurrentCulture = culture;
						Thread.CurrentThread.CurrentUICulture = culture;
					}
				}
				catch (Exception)
				{
				}
			}
			
			base.OnActionExecuting(filterContext);
		}
	}
}