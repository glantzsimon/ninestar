using System;
using System.Web;

namespace K9.WebApplication.Helpers
{
    public static class CalculatorTrialHelper
    {
        public const string CookieName = "NineStarKi.CalculatorTrialUsed";
        private const string SessionKey = "CalculatorTrialUsed";

        public static bool HasUsedTrial(HttpContextBase context) =>
            context.Session?[SessionKey] as bool? == true || context.Request.Cookies[CookieName] != null;

        public static void RecordResult(HttpContextBase context)
        {
            if (context.Session != null) context.Session[SessionKey] = true;
            var applicationPath = context.Request.ApplicationPath;
            context.Response.Cookies.Add(new HttpCookie(CookieName, "1")
            {
                HttpOnly = true,
                Secure = context.Request.IsSecureConnection,
                SameSite = SameSiteMode.Lax,
                Path = string.IsNullOrEmpty(applicationPath) ? "/" : applicationPath.TrimEnd('/') + "/",
                Expires = DateTime.UtcNow.AddYears(1)
            });
        }
    }
}
