using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class MembershipController : BaseNineStarKiController
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _membershipService = membershipService;
        }

        public ActionResult Index()
        {
            return View(_membershipService.GetMembershipViewModel());
        }

        [Route("membership/signup")]
        public ActionResult PurchaseStart(int id)
        {
            return View(_membershipService.GetPurchaseMembershipModel(id));
        }

        [Route("membership/signup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(int id)
        {
            return View(_membershipService.GetPurchaseStripeModel(id));
        }

        [HttpPost]
        [Route("membership/signup/processing")]
        [ValidateAntiForgeryToken]
        public ActionResult PurchaseProcess(StripeModel model)
        {
            try
            {
                _membershipService.ProcessPurchase(model);
                return RedirectToAction("PurchaseSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Purchase", model);
        }

        [Route("membership/signup/success")]
        public ActionResult PurchaseSuccess()
        {
            return View();
        }

        [Route("membership/switch")]
        public ActionResult SwitchStart(int id)
        {
            var switchMembershipModel = _membershipService.GetSwitchMembershipModel(id);
            ViewBag.SubTitle = switchMembershipModel.IsUpgrade
                ? Globalisation.Dictionary.UpgradeMembership
                : Globalisation.Dictionary.ChangeMembership;

            if (switchMembershipModel.IsScheduledSwitch)
            {
                return View("SwitchScheduleStart", switchMembershipModel);
            }
            return View("SwitchPurchaseStart", switchMembershipModel);
        }

        [Route("membership/switch")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchPurchase(int id)
        {
            var switchMembershipModel = _membershipService.GetSwitchMembershipModel(id);
            ViewBag.SubTitle = switchMembershipModel.IsUpgrade
                ? Globalisation.Dictionary.UpgradeMembership
                : Globalisation.Dictionary.ChangeMembership;

            return View(_membershipService.GetPurchaseStripeModel(id));
        }

        [Route("membership/switch/schedule")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchSchedule(int id)
        {
            var switchMembershipModel = _membershipService.GetSwitchMembershipModel(id);
            ViewBag.SubTitle = switchMembershipModel.IsUpgrade
                ? Globalisation.Dictionary.UpgradeMembership
                : Globalisation.Dictionary.ChangeMembership;

            return View(switchMembershipModel);
        }

        [HttpPost]
        [Route("membership/switch/processing")]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchPurchaseProcess(StripeModel model)
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

            return View("SwitchPurchase", model);
        }

        [HttpPost]
        [Route("membership/switch/processing")]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchScheduleProcess(int id)
        {
            try
            {
                _membershipService.ProcessSwitch(id);
                return RedirectToAction("SwitchSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("SwitchFree", _membershipService.GetSwitchMembershipModel(id));
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
