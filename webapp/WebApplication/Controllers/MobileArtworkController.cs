using K9.WebApplication.Helpers;
using K9.WebApplication.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace K9.WebApplication.Controllers
{
    // Deliberately public, identity-free and independent of private API no-store responses.
    [AllowAnonymous]
    [SessionState(SessionStateBehavior.Disabled)]
    [RoutePrefix("api/mobile/v1/metadata")]
    public class MobileArtworkController : Controller
    {
        [HttpGet, Route("artwork")]
        public ActionResult Artwork()
        {
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.TrySkipIisCustomErrors = true;
            // Read once: a scheduled media health check can switch this base concurrently.
            var imageBase = GetBaseImagesPath();
            Uri absoluteBase;
            if (!Uri.TryCreate(imageBase, UriKind.Absolute, out absoluteBase)
                || (absoluteBase.Scheme != Uri.UriSchemeHttp && absoluteBase.Scheme != Uri.UriSchemeHttps)
                || !string.IsNullOrEmpty(absoluteBase.Query) || !string.IsNullOrEmpty(absoluteBase.Fragment)
                || !string.IsNullOrEmpty(absoluteBase.UserInfo))
            {
                Response.StatusCode = 503;
                Response.Cache.SetNoStore();
                return Content("{\"success\":false,\"data\":null,\"error\":{\"code\":\"artwork_unavailable\"}}",
                    "application/json", Encoding.UTF8);
            }

            var data = MobileArtworkRegistry.Create(imageBase);
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetMaxAge(TimeSpan.FromMinutes(5));
            Response.Cache.SetETag("W/\"" + data.Version + "\"");
            if (MobileArtworkRegistry.MatchesETag(Request.Headers["If-None-Match"], data.Version))
                return new HttpStatusCodeResult(304);

            return Content(JsonConvert.SerializeObject(new { success = true, data, error = (object)null },
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                    TypeNameHandling = TypeNameHandling.None
                }), "application/json", Encoding.UTF8);
        }

        protected virtual string GetBaseImagesPath() => MediaService.BaseImagesPath;
    }
}
