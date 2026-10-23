using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.Helpers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/membership")]
    public class MobileMembershipController : MobileApiControllerBase
    {
        private readonly IPromotionService _promotions;
        public MobileMembershipController(INineStarKiPackage package, IPromotionService promotions) : base(package) { _promotions = promotions; }

        [HttpGet, Route("entitlements")]
        public ActionResult Entitlements()
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (Package.UserService.Find(id) == null) return ApiError(404, "account_not_found");
            var membership = Package.MembershipService.GetExistingActiveMembership(id);
            return ApiSuccess(MobileEntitlementsMapper.Map(id, membership, GetUtcNow()));
        }

        [HttpGet, Route("options")]
        public ActionResult Options()
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            return ApiSuccess(Package.MembershipService.GetMembershipViewModel(id).MembershipModels.Select(e => new
            {
                e.MembershipOption.Id, e.MembershipOption.SubscriptionType, e.MembershipOption.SubscriptionTypeNameLocal,
                e.SubscriptionPrice, e.MembershipOption.IsFree, e.MembershipOption.IsForever,
                e.IsSelectable, e.IsSubscribed, e.IsUpgrade
            }).ToArray());
        }

        [HttpGet, Route("quote")]
        public ActionResult Quote(int membershipOptionId)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ModelState.IsValid || membershipOptionId <= 0) return ApiError(400, "invalid_membership_option");
            var model = Package.MembershipService.GetPurchaseMembershipModel(membershipOptionId);
            return ApiSuccess(new
            {
                model.MembershipOption.Id, model.MembershipOption.SubscriptionTypeNameLocal,
                model.SubscriptionPrice, Currency = Models.StripeModel.GetSystemCurrencyCode(), model.IsUpgrade
            });
        }

        [HttpGet, Route("switch-quote")]
        public ActionResult SwitchQuote(int membershipOptionId)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var model = Package.MembershipService.GetSwitchMembershipModel(membershipOptionId, id);
            return ApiSuccess(new { model.MembershipOption.Id, model.SubscriptionPrice, model.IsUpgrade });
        }

        [HttpPost, Route("redeem-promotion")]
        public ActionResult RedeemPromotion(string code)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (string.IsNullOrWhiteSpace(code) || code.Length > 100) return ApiError(400, "invalid_promotion");
            var promotion = _promotions.Find(code);
            if (promotion == null || (!promotion.IsReusable && _promotions.FindForUser(code, id) == null) || _promotions.IsPromotionAlreadyUsed(code, id))
                return ApiError(400, "invalid_promotion");
            // This owner validates/consumes the promotion and grants only free offers.
            var free = Package.MembershipService.CreateMembershipFromPromoCode(id, code);
            return ApiSuccess(new { Completed = free, RequiresPayment = !free });
        }
    }
}
