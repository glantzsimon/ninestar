using System;
using System.Web;

namespace K9.WebApplication.Helpers
{
    public static class MobileArtworkCacheHelper
    {
        // HttpCachePolicy can tighten an existing policy, but cannot relax it.
        // Leave this exact public GET to its controller; never make it public here,
        // since routing failures and unavailable artwork must remain uncacheable.
        public static void ApplyRequestDefault(HttpContextBase context, HttpCacheability defaultPolicy)
        {
            if (context?.Response == null) return;
            var request = context.Request;
            if (request != null
                && string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrEmpty(request.PathInfo)
                && string.Equals(request.AppRelativeCurrentExecutionFilePath?.TrimEnd('/'),
                    "~/api/mobile/v1/metadata/artwork", StringComparison.OrdinalIgnoreCase))
                return;

            context.Response.Cache.SetCacheability(defaultPolicy);
        }
    }
}
