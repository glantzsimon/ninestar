using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    // Deliberately avoids BaseNineStarKiController's public UI/upload actions and
    // session-writing initialisation. The plugin ApiController remains independent.
    public abstract class MobileApiControllerBase : Controller
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Culture = CultureInfo.InvariantCulture,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            TypeNameHandling = TypeNameHandling.None,
            Converters = { new StringEnumConverter() }
        };

        protected MobileApiControllerBase(INineStarKiPackage package)
        {
            Package = package;
        }

        protected INineStarKiPackage Package { get; }

        protected int GetAuthenticatedUserId()
        {
            // An API key or account number never establishes a mobile user identity.
            return Request.IsAuthenticated ? ResolveCurrentUserId() : 0;
        }

        protected virtual int ResolveCurrentUserId() => Current.UserId;

        protected virtual DateTime GetUtcNow() => DateTime.UtcNow;

        protected ContentResult ApiSuccess(object data)
        {
            return ApiJson(200, new { success = true, data, error = (object)null });
        }

        protected ContentResult ApiError(int statusCode, string code)
        {
            // Codes are protocol values; clients localise their own error messages.
            return ApiJson(statusCode, new { success = false, data = (object)null, error = new { code } });
        }

        private ContentResult ApiJson(int statusCode, object payload)
        {
            Response.StatusCode = statusCode;
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.TrySkipIisCustomErrors = true;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return Content(JsonConvert.SerializeObject(payload, JsonSettings), "application/json", Encoding.UTF8);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            // Do not include account/birth data or exception details in the response.
            Package.Logger.Error("Mobile API failed: " + filterContext.Exception.GetType().Name);
            filterContext.Result = ApiError(500, "internal_error");
            filterContext.ExceptionHandled = true;
        }
    }
}
