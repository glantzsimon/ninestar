using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Exceptions;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    public class MembershipController : BaseNineStarKiController
    {
        private readonly IPromotionService _promotionService;

        public MembershipController(INineStarKiPackage nineStarKiPackage, IPromotionService promotionService)
            : base(nineStarKiPackage)
        {
            _promotionService = promotionService;
        }

        public ActionResult Index(string retrieveLast = null)
        {
            TempData["RetrieveLast"] = retrieveLast;
            return View(My.MembershipService.GetMembershipViewModel());
        }

        [Authorize]
        [Route("membership/unlock")]
        public ActionResult PurchaseStart(int membershipOptionId, string promoCode = "")
        {
            Promotion promotion = null;

            if (!string.IsNullOrEmpty(promoCode))
            {
                if (_promotionService.IsPromotionAlreadyUsed(promoCode, Current.UserId))
                {
                    ModelState.AddModelError("", Globalisation.Dictionary.PromoCodeInUse);
                }

                promotion = _promotionService.Find(promoCode);
                var userPromotion = _promotionService.FindForUser(promoCode, Current.UserId);

                if (promotion == null || userPromotion == null)
                {
                    ModelState.AddModelError("", Globalisation.Dictionary.InvalidPromoCode);
                }
                else if (userPromotion.UsedOn.HasValue)
                {
                    ModelState.AddModelError("", Globalisation.Dictionary.PromoCodeInUse);
                }
            }

            try
            {
                var model = My.MembershipService.GetPurchaseMembershipModel(membershipOptionId, promoCode);
                return View(model);
            }
            catch (UserAlreadySubscribedException e)
            {
                return View("AlreadySubscribed");
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e.GetFullErrorMessage);
                throw;
            }
        }

        [Authorize]
        [Route("membership/unlock/payment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(int membershipOptionId, string promoCode)
        {
            var membershipModel = My.MembershipService.GetPurchaseMembershipModel(membershipOptionId, promoCode);
            ViewBag.SubTitle = Globalisation.Dictionary.UpgradeMembership;
            return View(membershipModel);
        }

        [Authorize]
        [HttpPost]
        [Route("membership/unlock/payment/process")]
        public ActionResult ProcessPurchase(PurchaseModel purchaseModel)
        {
            try
            {
                My.MembershipService.ProcessPurchase(purchaseModel);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Logger.Error($"MembershipController => ProcessPurchase => Error: {ex.GetFullErrorMessage()}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize]
        [Route("membership/unlock/success")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult PurchaseSuccess()
        {
            var membership = My.MembershipService.GetActiveUserMembership(Current.UserId);
            var consultations = My.UserService.GetPendingConsultations(Current.UserId);
            var user = My.UserService.Find(Current.UserId);
            return View(new MyAccountViewModel
            {
                User = user,
                Membership = My.MembershipService.GetActiveUserMembership(user?.Id),
                Consultations = My.UserService.GetPendingConsultations(user.Id)
            });
        }

        [Authorize]
        [Route("membership/unlock/cancel/success")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult PurchaseCancelSuccess()
        {
            return View();
        }

        [Authorize]
        [Route("membership/upgrade/payment")]
        public ActionResult SwitchStart(int membershipOptionId)
        {
            var switchMembershipModel = My.MembershipService.GetSwitchMembershipModel(membershipOptionId);
            ViewBag.Title = Globalisation.Dictionary.UpgradeMembership;

            return View("PurchaseStart", switchMembershipModel);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

    }
}
