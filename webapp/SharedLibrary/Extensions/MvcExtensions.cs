
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

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

		public static ViewDataDictionary MergeAttribute(this ViewDataDictionary viewDataDictionary, string key, string value)
		{
			if (viewDataDictionary.ContainsKey(key))
			{
				viewDataDictionary[key] += string.Format(" {0}", value);
			}
			else
			{
				viewDataDictionary.Add(key, value);
			}
			return viewDataDictionary;
		}

		public static ViewDataDictionary Extend(this ViewDataDictionary viewDataDictionary, object attributes)
		{
			if (attributes != null)
			{
				foreach (var propInfo in attributes.GetProperties())
				{
					viewDataDictionary.Add(propInfo.Name, attributes.GetProperty(propInfo.Name));
				}
			}
			return viewDataDictionary;
		}
		
	}
}
