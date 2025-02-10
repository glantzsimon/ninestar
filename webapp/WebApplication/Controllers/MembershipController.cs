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
    [Authorize]
    public class MembershipController : BaseNineStarKiController
    {
        private readonly ILogger _logger;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;

        public MembershipController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IMembershipService membershipService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository, IUserService userService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _logger = logger;
            _membershipService = membershipService;
            _userService = userService;
        }

        public ActionResult Index(string retrieveLast = null)
        {
            TempData["RetrieveLast"] = retrieveLast;
            return View(_membershipService.GetMembershipViewModel());
        }

        [Route("membership/signup")]
        public ActionResult PurchaseStart(int membershipOptionId)
        {
            return View(_membershipService.GetPurchaseMembershipModel(membershipOptionId));
        }

        [HttpPost]
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

        [Route("membership/signup/success")]
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
        
        [Route("membership/signup/cancel/success")]
        public ActionResult PurchaseCancelSuccess()
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
        
        [HttpPost]
        [Route("membership/switch/processing")]
        [ValidateAntiForgeryToken]
        public ActionResult SwitchPurchaseProcess(PurchaseModel model)
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
