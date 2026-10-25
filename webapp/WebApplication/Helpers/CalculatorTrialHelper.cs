using System;
using System.Web;

namespace K9.WebApplication.Helpers
{
    public static class CalculatorTrialHelper
    {
        private const string CookiePrefix = "NineStarKi.CalculatorTrialUsed.";
        private const string SessionPrefix = "CalculatorTrialUsed:";

        public static string SectionKey(string controller)
        {
            switch (controller?.ToLowerInvariant())
            {
                case "personalchart": case "personal-chart": return "personal-chart";
                case "predictions": return "predictions";
                case "compatibility": return "compatibility";
                case "numerology": return "numerology";
                case "biorhythms": return "biorhythms";
                case "iching": case "i-ching": return "i-ching";
                default: throw new ArgumentException("Unknown calculator section.", nameof(controller));
            }
        }

        public static string CookieNameFor(string section) => CookiePrefix + SectionKey(section);

        public static bool HasUsedTrial(HttpContextBase context, string section) =>
            context.Session?[SessionPrefix + SectionKey(section)] as bool? == true ||
            context.Request.Cookies[CookieNameFor(section)] != null;

        public static void RecordResult(HttpContextBase context, string section)
        {
            if (context.Session != null) context.Session[SessionPrefix + SectionKey(section)] = true;
            var applicationPath = context.Request.ApplicationPath;
            context.Response.Cookies.Add(new HttpCookie(CookieNameFor(section), "1")
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
