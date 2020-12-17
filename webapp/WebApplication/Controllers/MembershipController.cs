using K9.Base.WebApplication.Controllers;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class MembershipController : BaseNineStarKiController
    {
        private readonly ILogger _logger;
        private readonly IRepository<MembershipOption> _membershipOptionRepository;
        private readonly IRepository<UserMembership> _userMembershipRepository;
        private readonly IStripeService _stripeService;
        private readonly IContactService _contactService;
        private readonly IMembershipService _membershipService;
        private readonly StripeConfiguration _stripeConfig;

        public MembershipController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository, IOptions<StripeConfiguration> stripeConfig, IStripeService stripeService, IContactService contactService, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _logger = logger;
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _stripeService = stripeService;
            _contactService = contactService;
            _membershipService = membershipService;
            _stripeConfig = stripeConfig.Value;
        }

        public ActionResult Index()
        {
            return View(_membershipService.GetMembershipViewModel());
        }

        [Route("join")]
        public ActionResult PurchaseStart(int id)
        {
            return View(_membershipService.GetPurchaseMembershipModel(id));
        }

        [Route("join/purchase")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(int id)
        {
            return View(_membershipService.GetPurchaseStripeModel(id));
        }

        [HttpPost]
        [Route("join/processing")]
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

        [Route("join/success")]
        public ActionResult PurchaseSuccess()
        {
            return View();
        }

        [Route("switch")]
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

        [Route("switch/purchase")]
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

        [Route("switch/schedule")]
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
        [Route("switch/processing")]
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
        [Route("switch/processing")]
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

            return View("SwitchFree", id);
        }

        [Route("switch/success")]
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
