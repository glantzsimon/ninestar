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
			const string defaultLanguageCode = "en-GB";
			var languageCode = "";

			if (
				filterContext.RequestContext != null && 
				filterContext.RequestContext.HttpContext != null &&
				filterContext.RequestContext.HttpContext.Session != null)
			{
				try
				{
					languageCode = filterContext.RequestContext.HttpContext.Session["languageCode"].ToString();	
				}
				catch (Exception)
				{
				}
			}
			
			languageCode = string.IsNullOrEmpty(languageCode) ? defaultLanguageCode : languageCode;

			var culture = CultureInfo.GetCultureInfo(languageCode);
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

			base.OnActionExecuting(filterContext);
		}
	}
}