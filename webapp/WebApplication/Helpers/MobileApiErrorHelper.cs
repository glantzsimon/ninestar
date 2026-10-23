using Newtonsoft.Json;
using System;
using System.Web;

namespace K9.WebApplication.Helpers
{
    public static class MobileApiErrorHelper
    {
        public static bool IsMobilePath(string applicationRelativePath)
        {
            const string prefix = "~/api/mobile/v1";
            return string.Equals(applicationRelativePath, prefix, StringComparison.OrdinalIgnoreCase) ||
                applicationRelativePath?.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase) == true;
        }

        public static int Status(Exception exception)
        {
            var status = (exception as HttpException)?.GetHttpCode() ?? 500;
            return status >= 400 && status <= 599 ? status : 500;
        }

        public static string Code(int status)
        {
            switch (status)
            {
                case 400: return "invalid_request";
                case 401: return "authentication_required";
                case 403: return "access_denied";
                case 404: return "endpoint_not_found";
                case 405: return "method_not_allowed";
                case 413: return "request_too_large";
                case 429: return "too_many_requests";
                default: return "internal_error";
            }
        }

        public static string Json(int status) => JsonConvert.SerializeObject(new
        { success = false, data = (object)null, error = new { code = Code(status) } },
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
    }
}
