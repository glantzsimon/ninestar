
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
        public static string AbsoluteAction(this UrlHelper helper, string actionName, string controllerName, object routeValues = null)
        {
            if (helper.RequestContext.HttpContext.Request.Url != null)
                return helper.Action(actionName, controllerName, routeValues, helper.RequestContext.HttpContext.Request.Url.Scheme);

            return helper.Action(actionName, controllerName);
        }

        public static string AbsoluteContent(this UrlHelper helper, string contentPath)
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).CombineWith(helper.Content(contentPath));
        }

        public static string ActionWithBookmark(this UrlHelper helper, string actionName, string controllerName, string bookmark)
        {
            return helper.RequestContext.RouteData.Values["action"].ToString().ToLower() == actionName.ToLower() &&
                   helper.RequestContext.RouteData.Values["controller"].ToString().ToLower() == controllerName.ToLower() 
                ? $"#{bookmark}"
                : $"{helper.Action(actionName, controllerName)}#{bookmark}";
        }

        public static string CombineWith(this string baseUrl, string url)
        {
            return new Uri(new Uri(baseUrl), url).ToString();
        }

        public static string CombineWith(this Uri uri, string url)
        {
            return new Uri(uri, url).ToString();
        }

        public static IDictionary<string, object> MergeAttribute(this IDictionary<string, object> dictionary, string key, object value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] += $" {value}";
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
            var queryString = controller.ControllerContext.RequestContext.HttpContext.Request.Unvalidated.QueryString;
            return queryString.AllKeys.Select(key => $"{key}={queryString.GetValue(key)}").Aggregate("", (a, b) => a + (string.IsNullOrEmpty(b) ? "" : $"&{b}"));
        }

        public static IStatelessFilter GetStatelessFilter(this HtmlHelper helper)
        {
            return helper.ViewContext.HttpContext.Request.GetStatelessFilter();
        }

        public static RouteValueDictionary GetFilterRouteValueDictionary(this ControllerBase controller)
        {
            return controller.ControllerContext.HttpContext.Request.GetStatelessFilter().GetFilterRouteValues();
        }

        public static IStatelessFilter GetStatelessFilter(this ControllerBase controller)
        {
            return controller.ControllerContext.HttpContext.Request.GetStatelessFilter();
        }

        public static RouteValueDictionary GetStatelessFilterRouteValues(this ControllerBase controller, string foreignKeyName, int foreignKeyValue)
        {
            return new StatelessFilter(foreignKeyName, foreignKeyValue).GetFilterRouteValues();
        }

        public static IStatelessFilter GetStatelessFilter(this HttpRequestBase request)
        {
            var queryString = request.Unvalidated.QueryString;
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
            return view.ViewContext.HttpContext.Request.Unvalidated.QueryString[key];
        }

        public static string GetDateTimeDisplayFormat(this CultureInfo cultureInfo)
        {
            return $"{cultureInfo.DateTimeFormat.ShortDatePattern} {cultureInfo.DateTimeFormat.ShortTimePattern}";
        }

    }
}
