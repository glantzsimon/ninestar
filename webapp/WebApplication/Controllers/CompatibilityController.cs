using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("relationship-compatibility")]
    public partial class CompatibilityController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public CompatibilityController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("calculator")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", VaryByHeader="User-Agent", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            return View(new CompatibilityModel());
        }

        [Route("calculator/result")]
        [HttpPost]
        public ActionResult IndexPost(CompatibilityModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NineStarKiModel1?.PersonModel != null && model.NineStarKiModel2?.PersonModel != null)
                {
                    var processedModel = _nineStarKiService.CalculateCompatibility(model.NineStarKiModel1.PersonModel, model.NineStarKiModel2.PersonModel, model.IsHideSexualChemistry, model.NineStarKiModel1.CalculationMethod);

                    if (Current.UserId > 0)
                    {
                        var user = My.UserService.Find(Current.UserId);
                        var myAccount = My.AccountService.GetAccount(Current.UserId);
                        if (myAccount.Membership.MembershipOption.IsFree)
                        {
                            if (myAccount.Membership.ComplementaryCompatibilityReadingCount > 0)
                            {
                                My.MembershipService.UseComplementaryCompatibilityReading(Current.UserId);
                                processedModel.IsComplementary = true;
                            }
                            else
                            {
                                processedModel.IsComplementary = false;
                            }
                        }
                    }

                    return View("Index", processedModel);
                }
            }
            return View("Index", model);
        }

        [Route("retrieve-last")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RetrieveLastCompatibility(bool todayOnly = false)
        {
            var lastCompatibility = SessionHelper.GetLastCompatibility(todayOnly)?.CompatibilityModel;
            if (lastCompatibility == null)
            {
                return RedirectToAction("Index");
            }
            var model = _nineStarKiService.CalculateCompatibility(lastCompatibility.NineStarKiModel1.PersonModel, lastCompatibility.NineStarKiModel2.PersonModel, lastCompatibility.IsHideSexualChemistry);
            return View("Index", model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

