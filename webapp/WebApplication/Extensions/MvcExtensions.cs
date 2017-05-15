
using System.Web.Mvc;

namespace K9.WebApplication.Extensions
{
	public static class MvcExtensions
	{

		public static string GetActiveClass(this ViewContext viewContext, string actionName, string controllerName)
		{
			return viewContext.RouteData.Values["action"].ToString() == actionName &&
				viewContext.RouteData.Values["controller"].ToString() == controllerName ? "active" : "";
		}

	}
}