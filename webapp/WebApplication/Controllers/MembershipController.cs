using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Web.Mvc;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class MembershipController : BaseNineStarKiController
    {
        private readonly ILogger _logger;
        private readonly IMembershipService _membershipService;

        public MembershipController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _logger = logger;
            _membershipService = membershipService;
        }

        public ActionResult Index()
        {
            return View(_membershipService.GetMembershipViewModel());
        }

        [Route("membership/signup")]
        public ActionResult PurchaseStart(int membershipOptionId)
        {
            return View(_membershipService.GetPurchaseMembershipModel(membershipOptionId));
        }

        [HttpPost]
        public ActionResult ProcessPurchase(PaymentModel paymentModel)
        {
            try
            {
                _membershipService.ProcessPurchase(paymentModel);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipController => ProcessPurchase => Error: {ex.GetFullErrorMessage()}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Route("membership/signup/success")]
        public ActionResult PurchaseSuccess()
        {
            return View();
        }

        [Route("membership/purchase-credits")]
        public ActionResult PurchaseCreditsStart()
        {
            return View(new PurchaseCreditsViewModel());
        }

        [Route("membership/purchase-credits")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchaseCredits(PurchaseCreditsViewModel model)
        {
            return View(model);
        }

        [Route("membership/signup/cancel/success")]
        public ActionResult PurchaseCancelSuccess()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProcessPurchaseCredits(PaymentModel paymentModel)
        {
            try
            {
                _membershipService.ProcessCreditsPurchase(paymentModel);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipController => ProcessPurchase => Error: {ex.GetFullErrorMessage()}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Route("membership/purchase-credits/success")]
        public ActionResult PurchaseCreditsSuccess()
        {
            return View();
        }

        [Route("membership/switch")]
        public ActionResult SwitchStart(int membershipOptionId)
        {
            var switchMembershipModel = _membershipService.GetSwitchMembershipModel(membershipOptionId);
            ViewBag.SubTitle = switchMembershipModel.IsUpgrade
                ? Globalisation.Dictionary.UpgradeMembership
                : Globalisation.Dictionary.ChangeMembership;

            if (switchMembershipModel.IsScheduledSwitch)
            {
                return View("SwitchScheduleStart", switchMembershipModel);
            }
            return View("SwitchPurchaseStart", switchMembershipModel);
        }

        [Route("membership/switch/review")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchPurchase(int membershipOptionId)
        {
            var switchMembershipModel = _membershipService.GetSwitchMembershipModel(membershipOptionId);
            ViewBag.SubTitle = switchMembershipModel.IsUpgrade
                ? Globalisation.Dictionary.UpgradeMembership
                : Globalisation.Dictionary.ChangeMembership;

            return View(switchMembershipModel);
        }

        [Route("membership/switch/schedule")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchSchedule(int membershipOptionId)
        {
            var switchMembershipModel = _membershipService.GetSwitchMembershipModel(membershipOptionId);
            ViewBag.SubTitle = switchMembershipModel.IsUpgrade
                ? Globalisation.Dictionary.UpgradeMembership
                : Globalisation.Dictionary.ChangeMembership;

            return View(switchMembershipModel);
        }

        [HttpPost]
        [Route("membership/switch/processing")]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchPurchaseProcess(PaymentModel model)
        {
            try
            {
                _membershipService.ProcessPurchase(model);
                return RedirectToAction("SwitchSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("SwitchPurchase", _membershipService.GetSwitchMembershipModel(model.ItemId));
        }

        [HttpPost]
        [Route("membership/switch/free/processing")]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchScheduleProcess(int membershipOptionId)
        {
            try
            {
                _membershipService.ProcessSwitch(membershipOptionId);
                return RedirectToAction("SwitchSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("SwitchFree", _membershipService.GetSwitchMembershipModel(membershipOptionId));
        }

        [Route("membership/switch/success")]
        public ActionResult SwitchSuccess()
        {
            return View();
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

    }
}
