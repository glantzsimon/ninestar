
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

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

	}
}
