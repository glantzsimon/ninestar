using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("biorhythms")]
    public partial class BiorhythmsController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly IBiorhythmsService _biorhythmsService;

        public BiorhythmsController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, IBiorhythmsService biorhythmsService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _biorhythmsService = biorhythmsService;
        }

        [Route("calculator")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult _CalculatorForm(NineStarKiModel model)
        {
            return PartialView(model);
        }

        [Route("calculator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NineStarKiModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PersonModel != null || model.SelectedDate != DateTime.Today)
                {
                    var selectedDate = model.SelectedDate ?? DateTime.Today;
                    var isScrollToCyclesOverview = model.IsScrollToCyclesOverview;
                    var activeTabId = model.ActiveCycleTabId;

                    model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate);
                    model.SelectedDate = selectedDate;
                    model.IsScrollToCyclesOverview = isScrollToCyclesOverview;
                    model.ActiveCycleTabId = activeTabId;
                }
            }

            model.BiorhythmResultSet = _biorhythmsService.Calculate(model, model.SelectedDate ?? DateTime.Today);

            return View("Index", model);
        }

        [Route("retrieve-last")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RetrieveLastBiorhythms(bool todayOnly = false)
        {
            var lastBiorhythms = SessionHelper.GetLastBiorhythm(todayOnly)?.PersonModel;
            if (lastBiorhythms == null)
            {
                return RedirectToAction("Index");
            }
            var model = _nineStarKiService.CalculateNineStarKiProfile(lastBiorhythms.DateOfBirth, lastBiorhythms.Gender);
            return View("Index", model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

