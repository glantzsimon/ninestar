using K9.WebApplication.Helpers;
using System;
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

            ConfigureMiniProfiler();
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
            SetSameSiteNoneForCookies();
            SetCacheBehaviour();
            InitMiniProfiler();
        }

        protected void Application_EndRequest()
        {
            StopMiniProfiler();
        }

        private void ConfigureMiniProfiler()
        {
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

        private void InitMiniProfiler()
        {
#if DEBUG
            if (EnableMiniProfiler && Request.IsLocal) // Enable MiniProfiler only for local requests
            {
                MiniProfiler.StartNew();
                HttpContext.Current.Items["RequestStartTime"] = Stopwatch.StartNew();
            }
#endif
        }

        private static void SetCacheBehaviour()
        {
            if (HttpContext.Current != null && HttpContext.Current.Response != null)
            {
#if DEBUG
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
#else
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
#endif
            }
        }

        private static void SetSameSiteNoneForCookies()
        {
            HttpContext context = HttpContext.Current;
            if (context != null && context.Response != null)
            {
                context.Response.AddOnSendingHeaders(ctx =>
                {
                    foreach (string key in ctx.Response.Headers.Keys)
                    {
                        if (key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
                        {
                            string cookieHeader = ctx.Response.Headers[key];
                            // Ensure SameSite=None and Secure for cross-site cookies
                            if (!cookieHeader.Contains("SameSite"))
                            {
                                ctx.Response.Headers[key] = cookieHeader + "; SameSite=None; Secure";
                            }
                        }
                    }
                });
            }
        }

        private void StopMiniProfiler()
        {
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