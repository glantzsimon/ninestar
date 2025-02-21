using K9.SharedLibrary.Authentication;
using Microsoft.Owin;

namespace K9.WebApplication.Services
{
    using Hangfire.Dashboard;

    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var environment = context.GetOwinEnvironment();
            if (environment == null)
            {
                // Deny access if we can't get the OWIN environment
                return false;
            }

            var owinContext = new OwinContext(environment);
            var user = owinContext.Authentication?.User;

            // If user info is missing, deny access or use a fallback
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return false;
            }

            return user.Identity.Name == "system";
        }
    }
}