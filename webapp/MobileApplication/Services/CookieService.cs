using System;
using System.Web;

namespace K9.WebApplication.Services
{
    public static class CookieService
    {
        private const string HideCookieWarning = "HideCookieWarning";

        public static bool IsDisplayCookiesWarning()
        {
            return !GetCookieBooleanValue(HideCookieWarning);
        }

        public static void HideCookiesWarning()
        {
            if (IsDisplayCookiesWarning())
            {
                SetCookie(HideCookieWarning, true, TimeSpan.FromDays(90));
            }
        }

        private static void SetCookie(string key, object value, TimeSpan expiresOn)
        {
            var cookie = new HttpCookie(key);
            cookie["value"] = value.ToString();
            cookie.Expires.Add(expiresOn);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private static string GetCookieValue(string key)
        {
            var cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
            {
                return cookie["value"];
            }
            return string.Empty;
        }

        private static bool GetCookieBooleanValue(string key)
        {
            bool.TryParse(GetCookieValue(key), out var value);
            return value;
        }
    }
}