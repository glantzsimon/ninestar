using K9.SharedLibrary.Authentication;

namespace K9.WebApplication.Services
{
    using Hangfire.Dashboard;
    using Microsoft.Owin;

    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var owinContext = new OwinContext(context.GetOwinEnvironment());
            var user = owinContext.Authentication.User;
            return user?.Identity?.IsAuthenticated == true && user.IsInRole("Meows");
        }
    }
}