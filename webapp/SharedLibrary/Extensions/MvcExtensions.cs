
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Extensions
{
	public static class MvcExtensions
	{
		/// <summary>
		/// Returns the absolute path with domain, etc, e.g. http://{domain}/path
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="actionName"></param>
		/// <param name="controllerName"></param>
		/// <returns></returns>
		public static string AsboluteAction(this UrlHelper helper, string actionName, string controllerName, object routeValues = null)
		{
			if (helper.RequestContext.HttpContext.Request.Url != null)
				return helper.Action(actionName, controllerName, routeValues, helper.RequestContext.HttpContext.Request.Url.Scheme);

			return helper.Action(actionName, controllerName);
		}

		public static string AbsoluteContent(this UrlHelper helper, string contentPath)
		{
			return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + helper.Content(contentPath);
		}

		public static IDictionary<string, object> MergeAttribute(this IDictionary<string, object> dictionary, string key, object value)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] += string.Format(" {0}", value);
			}
			else
			{
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		public static bool IsActionActive(this ViewContext viewContext, string actionName, string controllerName)
		{
			return viewContext.RouteData.Values["action"].ToString() == actionName &&
				   viewContext.RouteData.Values["controller"].ToString() == controllerName;
		}

		public static string GetQueryString(this ControllerBase controller)
		{
			var queryString = controller.ControllerContext.RequestContext.HttpContext.Request.QueryString;
			return queryString.AllKeys.Select(key => string.Format("{0}={1}", key, queryString.GetValue(key))).Aggregate("", (a, b) => a + (string.IsNullOrEmpty(b) ? "" : string.Format("&{0}", b)));
		}

		public static IStatelessFilter GetStatelessFilter(this HtmlHelper helper)
		{
			return helper.ViewContext.HttpContext.Request.GetStatelessFilter();
		}

		public static IStatelessFilter GetStatelessFilter(this HttpRequestBase request)
		{
			var queryString = request.QueryString;
			var foreignKeyName = queryString[Constants.Constants.Key];
			var foreignKeyValue = queryString[Constants.Constants.Value];
			var emptyFilter = new StatelessFilter(string.Empty, 0);

			try
			{
				return (foreignKeyName != null && foreignKeyValue != null) ? new StatelessFilter(foreignKeyName, int.Parse(foreignKeyValue)) : emptyFilter;
			}
			catch (Exception)
			{
			}
			return emptyFilter;
		}

		public static string GetQueryStringValue(this WebViewPage view, string key)
		{
			return view.ViewContext.HttpContext.Request.QueryString[key];
		}

		public static string GetDateTimeDisplayFormat(this CultureInfo cultureInfo)
		{
			return string.Format("{0} {1}", cultureInfo.DateTimeFormat.ShortDatePattern, cultureInfo.DateTimeFormat.ShortTimePattern);
		}

	}
}
