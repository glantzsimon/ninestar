using System.Web;
using WebMatrix.WebData;

namespace K9.WebApplication.Helpers
{
    public static class Current
    {
        public static int UserId
        {
            get
            {
                var currentUserId = SessionHelper.GetCurrentUserId();
                return currentUserId > 0 ? currentUserId : WebSecurity.CurrentUserId;
            }
        }

        public static string UserName
        {
            get
            {
                var currentUsername = SessionHelper.GetCurrentUserName();
                return string.IsNullOrEmpty(currentUsername) ? WebSecurity.CurrentUserName : currentUsername;
            }
        }

        public static int GetUserId(HttpContext httpContext)
        {
            var currentUserId = SessionHelper.GetCurrentUserId(httpContext);
            return currentUserId > 0 ? currentUserId : WebSecurity.CurrentUserId;
        }

        public static string UserTimeZoneId
        {
            get { return SessionHelper.GetCurrentUserTimeZone(); }
        }

        public static void StartImpersonating(int userId, string username)
        {
            SessionHelper.SetCurrentUserId(userId);
            SessionHelper.SetCurrentUserName(username);
        }

        public static void StopImpersonating()
        {
            SessionHelper.CleaCurrentUserId();
            SessionHelper.ClearCurrentUserName();
        }

        public static bool IsImpersonating()
        {
            return SessionHelper.GetCurrentUserId() > 0;
        }
    }
}