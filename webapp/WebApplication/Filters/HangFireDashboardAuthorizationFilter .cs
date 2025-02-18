using K9.WebApplication.Helpers;

namespace K9.WebApplication.Services
{
    using Hangfire.Dashboard;

    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return SessionHelper.CurrentUserIsAdmin();
        }
    }
}