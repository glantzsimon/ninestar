using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class MembershipController : BaseNineStarKiController
    {
        private readonly ILogger _logger;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IPromoCodeService _promoCodeService;

        public MembershipController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IMembershipService membershipService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository, IUserService userService, IPromoCodeService promoCodeService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _logger = logger;
            _membershipService = membershipService;
            _userService = userService;
            _promoCodeService = promoCodeService;
        }

        public ActionResult Index(string retrieveLast = null)
        {
            TempData["RetrieveLast"] = retrieveLast;
            return View(_membershipService.GetMembershipViewModel());
        }

        [Authorize]
        [Route("membership/unlock")]
        public ActionResult PurchaseStart(int membershipOptionId, string promoCode = "")
        {
            if (!string.IsNullOrEmpty(promoCode))
            {
                if (_promoCodeService.IsPromoCodeAlreadyUsed(promoCode))
                {
                    ModelState.AddModelError("", Globalisation.Dictionary.PromoCodeInUse);
                }

                var promoCodeModel = _promoCodeService.Find(promoCode);
                if (promoCodeModel == null)
                {
                    ModelState.AddModelError("", "Invalid promo code");
                }

                if (promoCodeModel.UsedOn.HasValue)
                {
                    ModelState.AddModelError("", Globalisation.Dictionary.PromoCodeInUse);
                }
            }

            try
            {
                var model = _membershipService.GetPurchaseMembershipModel(membershipOptionId, promoCode);
                return View(model);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.GetFullErrorMessage);
                throw;
            }
        }

        [Authorize]
        [Route("membership/unlock/payment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(int membershipOptionId)
        {
            var membershipModel = _membershipService.GetPurchaseMembershipModel(membershipOptionId);
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
                _membershipService.ProcessPurchase(purchaseModel);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipController => ProcessPurchase => Error: {ex.GetFullErrorMessage()}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Authorize]
        [Route("membership/unlock/success")]
        public ActionResult PurchaseSuccess()
        {
            var membership = _membershipService.GetActiveUserMembership(Current.UserId);
            var consultations = _userService.GetPendingConsultations(Current.UserId);
            var user = _userService.Find(Current.UserId);
            return View(new MyAccountViewModel
            {
                User = user,
                Membership = _membershipService.GetActiveUserMembership(user?.Id),
                Consultations = _userService.GetPendingConsultations(user.Id)
            });
        }

        [Authorize]
        [Route("membership/unlock/cancel/success")]
        public ActionResult PurchaseCancelSuccess()
        {
            return View();
        }

        [Authorize]
        [Route("membership/upgrade/payment")]
        public ActionResult SwitchStart(int membershipOptionId)
        {
            var switchMembershipModel = _membershipService.GetSwitchMembershipModel(membershipOptionId);
            ViewBag.Title = Globalisation.Dictionary.UpgradeMembership;

            return View("PurchaseStart", switchMembershipModel);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

    }
}
