using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/unsubscribe")]
    public class MobileUnsubscribeController : MobileApiControllerBase
    {
        public MobileUnsubscribeController(INineStarKiPackage package) : base(package) { }

        [HttpPost, Route("{target}")]
        public ActionResult Unsubscribe(string target, MobileUnsubscribeRequest request)
        {
            if (!ValidInput(request)) return ApiError(400, "invalid_unsubscribe_link");
            // The opaque external ID is the same capability carried by existing
            // unsubscribe links. It is never treated as an authenticated user ID.
            if (target != "user" && target != "contact") return ApiError(404, "unsubscribe_target_not_found");
            try
            {
                if (target == "user") Package.UserService.EnableMarketingEmails(request.ExternalId, false);
                else Package.ContactService.EnableMarketingEmails(request.ExternalId, false);
            }
            catch (Exception) { return ApiError(400, "invalid_unsubscribe_link"); }
            return ApiSuccess(new { Unsubscribed = true });
        }
    }
}
