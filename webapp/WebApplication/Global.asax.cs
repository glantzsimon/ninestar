using K9.WebApplication.Helpers;
using StackExchange.Profiling;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

namespace K9.WebApplication
{
    public class MvcApplication : HttpApplication
    {
        private const bool EnableMiniProfiler = false;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DataConfig.InitialiseDatabase();
            AuthConfig.InitialiseWebSecurity();
            DataConfig.InitialiseUsersAndRoles();

            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;

            Stripe.StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["SecretKey"];

#if DEBUG
            if (EnableMiniProfiler)
            {
                MiniProfiler.Configure(new MiniProfilerOptions
                {
                    RouteBasePath = "~/profiler",
                    ResultsAuthorize = request => true // Allow all users to see results
                });
            }
#endif  
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom == "User")
            {
                return WebSecurity.IsAuthenticated ? Current.UserName : "Anonymous";
            }

            return base.GetVaryByCustomString(context, custom);
        }

        protected void Application_BeginRequest()
        {
            if (HttpContext.Current != null && HttpContext.Current.Response != null)
            {
#if DEBUG
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
#else
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
#endif
            }

#if DEBUG
            if (EnableMiniProfiler && Request.IsLocal) // Enable MiniProfiler only for local requests
            {
                MiniProfiler.StartNew();
                HttpContext.Current.Items["RequestStartTime"] = Stopwatch.StartNew();
            }
#endif
        }

        protected void Application_EndRequest()
        {
            var cookie = Response.Cookies["__RequestVerificationToken"];
            if (cookie != null)
            {
                cookie.Secure = true;
                cookie.SameSite = SameSiteMode.None;
            }

#if DEBUG
            if (EnableMiniProfiler && MiniProfiler.Current != null)
            {
                LogTimeElapsed();
                MiniProfiler.Current?.Stop();
            }
#endif
        }

        private static void LogTimeElapsed()
        {
            var stopwatch = (Stopwatch)HttpContext.Current.Items["RequestStartTime"];
            var url = HttpContext.Current.Request.Url.AbsoluteUri;
            var elapsedTime = stopwatch.ElapsedMilliseconds;

            // Log only if the request actually took time (ignore cached results)
            if (elapsedTime > 0)
            {
                Debug.WriteLine($"🚀 Total Request Time: {elapsedTime}ms for {url}");
            }
        }
    }
}