using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/payments")]
    public class MobilePaymentsController : MobileApiControllerBase
    {
        private readonly IMobileCheckoutService _checkout;
        private readonly Config.StripeConfiguration _stripe;
        public MobilePaymentsController(INineStarKiPackage package, IMobileCheckoutService checkout, IOptions<Config.StripeConfiguration> stripe) : base(package)
        { _checkout = checkout; _stripe = stripe.Value; }

        [HttpPost, Route("start")]
        public ActionResult Start(MobileCheckoutRequest request)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!Request.IsSecureConnection && !Request.IsLocal) return ApiError(400, "https_required");
            if (!ValidInput(request)) return ApiError(400, "invalid_checkout_request");
            try { return ApiSuccess(new { _stripe.PublishableKey, Checkout = _checkout.Start(id, request) }); }
            catch (MobileCheckoutException e) { return ApiError(e.Status, e.Code); }
        }

        [HttpPost, Route("complete")]
        public ActionResult Complete(Guid requestId)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ModelState.IsValid) return ApiError(400, "invalid_checkout_request");
            try { return ApiSuccess(_checkout.Complete(id, requestId)); }
            catch (MobileCheckoutException e) { return ApiError(e.Status, e.Code); }
        }
    }
}
