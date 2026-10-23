using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using K9.WebApplication.ViewModels.MobileApi;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/admin/promotions")]
    public class MobileAdminPromotionsController : MobileApiControllerBase
    {
        private readonly IPromotionService _promotions;
        public MobileAdminPromotionsController(INineStarKiPackage package, IPromotionService promotions) : base(package)
        { _promotions = promotions; }

        [HttpPost, Route("email-registration")]
        public ActionResult EmailRegistration(MobilePromotionEmailRequest request)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_promotion_email");
            var promotion = _promotions.Find(request.PromotionId);
            if (promotion == null) return ApiError(404, "promotion_not_found");
            _promotions.SendRegistrationPromotion(new EmailPromoCodeViewModel
            { Promotion = promotion, Name = request.Name, EmailAddress = request.EmailAddress });
            return ApiSuccess(new { Accepted = true });
        }

        [HttpPost, Route("email-membership")]
        public ActionResult EmailMembership(int promotionId, int userId)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid || promotionId <= 0 || userId <= 0) return ApiError(400, "invalid_promotion_email");
            var promotion = _promotions.Find(promotionId);
            var user = Package.UserService.Find(userId);
            if (promotion == null || user == null) return ApiError(404, "promotion_recipient_not_found");
            _promotions.SendMembershipPromotion(new EmailPromoCodeViewModel
            { Promotion = promotion, UserId = userId, User = user, Name = user.FullName, EmailAddress = user.EmailAddress });
            // The legacy owner does not report delivery failures to its caller.
            // Acceptance is not confirmation that the email was delivered.
            return ApiSuccess(new { Accepted = true });
        }
    }
}
