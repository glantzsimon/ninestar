using System.IO;
using System.Web;
using System.Web.SessionState;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class Helpers
    {
        public static HttpContext CreateHttpContextWithSession()
        {
            var request = new HttpRequest("", "https://example.com", "");
            var response = new HttpResponse(new StringWriter());
            var context = new HttpContext(request, response);

            // Create session container
            var sessionContainer = new HttpSessionStateContainer(
                "id",
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10,
                true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc,
                false
            );

            // Attach session to context
            SessionStateUtility.AddHttpSessionStateToContext(context, sessionContainer);

            return context;
        }
    }
}
