using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using K9.SharedLibrary.Helpers;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("yearly-and-monthly-predictions")]
    public partial class PredictionsController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public PredictionsController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("calculator")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = Methods.GetRandomGender()
            };
            var nineStarKiModel = new NineStarKiModel(personModel)
            {
                IsPredictionsScreen = true,
                DisplayDataForPeriod = EDisplayDataForPeriod.Now
            };
            return View(new PredictionsViewModel(nineStarKiModel, _nineStarKiService.GetNineStarKiSummaryViewModel()));
        }

        [Route("calculator")]
        [HttpPost]
        public ActionResult Index(NineStarKiModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PersonModel != null)
                {
                    var localNow = model.GetLocalNow(); 
                    model.SelectedDate = model.DisplayDataForPeriod == EDisplayDataForPeriod.SelectedDate ? model.SelectedDate ?? localNow : localNow;
                    
                    var invertYinEnergies = model.CalculationMethod == ECalculationMethod.Chinese;

                    // Set user calculation method preference cookie
                    SessionHelper.SetCurrentUserCalculationMethod((int)model.CalculationMethod);
                    SessionHelper.SetCurrentUserUseHolograhpicCycles(model.UseHolograhpicCycleCalculation);
                    SessionHelper.SetInvertDailyAndHourlyKiForSouthernHemisphere(model.InvertDailyAndHourlyKiForSouthernHemisphere);
                    SessionHelper.SetInvertDailyAndHourlyCycleKiForSouthernHemisphere(model.InvertDailyAndHourlyCycleKiForSouthernHemisphere);
                    
                    // Add time of birth
                    model.PersonModel.DateOfBirth = model.PersonModel.DateOfBirth.Add(model.PersonModel.TimeOfBirth);
                 
                    model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false,
                        model.SelectedDate, model.CalculationMethod, true, model.UseHolograhpicCycleCalculation, model.InvertDailyAndHourlyKiForSouthernHemisphere,
                        model.InvertDailyAndHourlyCycleKiForSouthernHemisphere);
                   
                    if (Current.UserId > 0)
                    {
                        var user = My.UserService.Find(Current.UserId);
                        model.IsMyProfile = user.BirthDate == model.PersonModel.DateOfBirth;
                    }
                }
                model.IsPredictionsScreen = true;
                return View(new PredictionsViewModel(model, _nineStarKiService.GetNineStarKiSummaryViewModel()));
            }

            model.IsPredictionsScreen = true;
            return View(model);
        }

        [Route("get-monthly-forecast")]
        [OutputCache(Duration = 2592000, VaryByParam = "energy", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetMonthlyForecast(ENineStarKiEnergy energy)
        {
            var summary = _nineStarKiService.GetNineStarKiSummaryViewModel();
            var cycle = summary.MonthlyCycleEnergies.FirstOrDefault(e => e.Energy == energy);
            
            return Json(cycle, JsonRequestBehavior.AllowGet);
        }

        [Route("get-yearly-forecast")]
        [OutputCache(Duration = 2592000, VaryByParam = "energy", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetYearlyForecast(ENineStarKiEnergy energy)
        {
            var summary = _nineStarKiService.GetNineStarKiSummaryViewModel();
            var cycle = summary.YearlyCycleEnergies.FirstOrDefault(e => e.Energy == energy);
            
            return Json(cycle, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [Route("my-free-predictions")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult MyCycles()
        {
            var myAccount = My.UsersRepository.Find(Current.UserId);
            return View(_nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                Name = myAccount.FullName,
                DateOfBirth = myAccount.BirthDate,
                Gender = myAccount.Gender
            }, false, true));
        }

        [Route("retrieve-last")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RetrieveLastPredictions(bool todayOnly = false)
        {
            var lastPredictions = SessionHelper.GetLastCompatibility(todayOnly)?.PersonModel;
            if (lastPredictions == null)
            {
                return RedirectToAction("Index");
            }
            var model = _nineStarKiService.CalculateNineStarKiProfile(lastPredictions.DateOfBirth, lastPredictions.Gender);
            return View("Index", model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

