using K9.Base.DataAccessLayer.Enums;
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

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("predictions")]
    public partial class PredictionsController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly ISwissEphemerisService _swissEphemerisService;

        public PredictionsController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, ISwissEphemerisService swissEphemerisService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _swissEphemerisService = swissEphemerisService;
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

        [Route("calculator/result")]
        [HttpPost]
        public ActionResult IndexPost(NineStarKiModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PersonModel != null)
                {
                    var localNow = model.GetLocalNow();
                    if (model.DisplayDataForPeriod == EDisplayDataForPeriod.SelectedDate)
                    {
                        if (model.SelectedDate.HasValue)
                        {
                            if (model.SelectedTime.HasValue)
                            {
                                model.SelectedDate = model.SelectedDate.Value.Add(model.SelectedTime.Value);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(model.SelectedDate), Base.Globalisation.Dictionary.FieldIsRequired);
                            model.IsPredictionsScreen = true;
                            return View("Index", new PredictionsViewModel(model, _nineStarKiService.GetNineStarKiSummaryViewModel()));
                        }

                    }
                    else
                    {
                        model.SelectedDate = localNow;
                    }

                    var invertYinEnergies = model.CalculationMethod == ECalculationMethod.Chinese;

                    // Set user calculation method preference cookie
                    SessionHelper.SetCurrentUserCalculationMethod((int)model.CalculationMethod);
                    SessionHelper.SetCurrentUserHousesDisplay(model.HousesDisplay);
                    SessionHelper.SetInvertDailyAndHourlyKiForSouthernHemisphere(model.InvertDailyAndHourlyKiForSouthernHemisphere);
                    SessionHelper.SetInvertDailyAndHourlyCycleKiForSouthernHemisphere(model.InvertDailyAndHourlyCycleKiForSouthernHemisphere);

                    // Add time of birth
                    model.PersonModel.DateOfBirth = model.PersonModel.DateOfBirth.Add(model.PersonModel.TimeOfBirth);

                    var processedModel = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false,
                        model.SelectedDate, model.CalculationMethod, true, true, model.UserTimeZoneId, model.HousesDisplay, model.InvertDailyAndHourlyKiForSouthernHemisphere,
                        model.InvertDailyAndHourlyCycleKiForSouthernHemisphere,
                        model.DisplayDataForPeriod);

                    processedModel.DisplayDataForPeriod = model.DisplayDataForPeriod;
                    processedModel.SelectedTime = model.SelectedTime;
                    processedModel.CalculationMethod = model.CalculationMethod;
                    processedModel.UserTimeZoneId = model.UserTimeZoneId;
                    processedModel.SelectedDate = model.SelectedDate;
                    processedModel.SelectedTime = model.SelectedTime;
                    processedModel.HousesDisplay = model.HousesDisplay;
                    processedModel.InvertDailyAndHourlyKiForSouthernHemisphere = model.InvertDailyAndHourlyKiForSouthernHemisphere;
                    processedModel.InvertDailyAndHourlyCycleKiForSouthernHemisphere = model.InvertDailyAndHourlyCycleKiForSouthernHemisphere;

                    var plannerData = _nineStarKiService.GetPlannerData(model.PersonModel.DateOfBirth, model.PersonModel.BirthTimeZoneId, model.PersonModel.TimeOfBirth, model.PersonModel.Gender, model.SelectedDate.Value, model.UserTimeZoneId, model.CalculationMethod, model.DisplayDataForPeriod, model.HousesDisplay, model.InvertDailyAndHourlyKiForSouthernHemisphere, model.InvertDailyAndHourlyCycleKiForSouthernHemisphere,
                        EPlannerView.Year, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, processedModel);

                    UpdatePlannerUrls(plannerData);

                    processedModel.PlannerViewModel = plannerData;

                    if (Current.UserId > 0)
                    {
                        var user = My.UserService.Find(Current.UserId);
                        model.IsMyProfile = user.BirthDate == model.PersonModel.DateOfBirth;
                    }

                    processedModel.IsPredictionsScreen = true;
                    return View("Index", new PredictionsViewModel(
                        processedModel,
                        _nineStarKiService.GetNineStarKiSummaryViewModel()));
                }
            }

            model.IsPredictionsScreen = true;
            return View("Index", new PredictionsViewModel(model, _nineStarKiService.GetNineStarKiSummaryViewModel()));
        }

        [Route("get-planner")]
        public ActionResult GetPlanner(DateTime dateOfBirth, string birthTimeZoneId, TimeSpan timeOfBirth, EGender gender, DateTime selectedDateTime, string userTimeZoneId, ECalculationMethod calculationMethod, EDisplayDataForPeriod displayDataForPeriod, EHousesDisplay housesDisplay, bool invertDailyAndHourlyKiForSouthernHemisphere, bool invertDailyAndHourlyCycleKiForSouthernHemisphere, EPlannerView view, EScopeDisplay display, EPlannerNavigationDirection navigationDirection = EPlannerNavigationDirection.None)
        {
            var plannerData = _nineStarKiService.GetPlannerData(dateOfBirth, birthTimeZoneId, timeOfBirth, gender,
                selectedDateTime, userTimeZoneId, calculationMethod, displayDataForPeriod, housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere,
                invertDailyAndHourlyCycleKiForSouthernHemisphere, view, display, navigationDirection);

            UpdatePlannerUrls(plannerData);

            return PartialView("Planner/_GlobalPlanner", plannerData);
        }

        [Route("get-hourly-predictions")]
        [OutputCache(Duration = 2592000, VaryByParam = "energy", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetHourlyPredictions(ENineStarKiEnergy energy, EScopeDisplay display = EScopeDisplay.PersonalKi)
        {
            var summary = _nineStarKiService.GetNineStarKiSummaryViewModel();
            var cycle = summary.HourlyCycleEnergies.FirstOrDefault(e => e.Energy == energy);

            return PredictionsJsonResult(display, cycle);
        }
        
        [Route("get-daily-predictions")]
        [OutputCache(Duration = 2592000, VaryByParam = "energy", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetDailyPredictions(ENineStarKiEnergy energy, EScopeDisplay display = EScopeDisplay.PersonalKi)
        {
            var summary = _nineStarKiService.GetNineStarKiSummaryViewModel();
            var cycle = summary.DailyCycleEnergies.FirstOrDefault(e => e.Energy == energy);

            return PredictionsJsonResult(display, cycle);
        }

        [Route("get-monthly-predictions")]
        [OutputCache(Duration = 2592000, VaryByParam = "energy", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetMonthlyPredictions(ENineStarKiEnergy energy, EScopeDisplay display = EScopeDisplay.PersonalKi)
        {
            var summary = _nineStarKiService.GetNineStarKiSummaryViewModel();
            var cycle = summary.MonthlyCycleEnergies.FirstOrDefault(e => e.Energy == energy);

            return PredictionsJsonResult(display, cycle);
        }

        [Route("get-yearly-predictions")]
        [OutputCache(Duration = 2592000, VaryByParam = "energy", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetYearlyPredictions(ENineStarKiEnergy energy, EScopeDisplay display = EScopeDisplay.PersonalKi)
        {
            var summary = _nineStarKiService.GetNineStarKiSummaryViewModel();
            var cycle = summary.YearlyCycleEnergies.FirstOrDefault(e => e.Energy == energy);
            
            return PredictionsJsonResult(display, cycle);
        }

        [Route("get-nine-yearly-predictions")]
        [OutputCache(Duration = 2592000, VaryByParam = "energy", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetNineYearlyPredictions(ENineStarKiEnergy energy, EScopeDisplay display = EScopeDisplay.PersonalKi)
        {
            var summary = _nineStarKiService.GetNineStarKiSummaryViewModel();
            var cycle = summary.NineYearlyCycleEnergies.FirstOrDefault(e => e.Energy == energy);

            return PredictionsJsonResult(display, cycle);
        }

        [Route("get-eighty-one-yearly-predictions")]
        [OutputCache(Duration = 2592000, VaryByParam = "energy", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetEightyOneYearlyPredictions(ENineStarKiEnergy energy, EScopeDisplay display = EScopeDisplay.PersonalKi)
        {
            var summary = _nineStarKiService.GetNineStarKiSummaryViewModel();
            var cycle = summary.EightyOneYearlyCycleEnergies.FirstOrDefault(e => e.Energy == energy);

            return PredictionsJsonResult(display, cycle);
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

        private JsonResult PredictionsJsonResult(EScopeDisplay display, NineStarKiEnergy cycle)
        {
            return Json(new
            {
                cycle.CycleDescriptiveName,
                CycleDescription = display == EScopeDisplay.PersonalKi ? cycle.CycleDescription : cycle.GlobalCycleDescription
            }, JsonRequestBehavior.AllowGet);
        }

        private void UpdatePlannerUrls(PlannerViewModel plannerData)
        {
            switch (plannerData.View)
            {
                case EPlannerView.EightyOneYear:
                    plannerData.UpdateParentUrl = Url.Action("GetEightyOneYearlyPredictions");
                    plannerData.UpdateChildUrl = Url.Action("GetNineYearlyPredictions");
                    break;

                case EPlannerView.NineYear:
                    plannerData.UpdateParentUrl = Url.Action("GetNineYearlyPredictions");
                    plannerData.UpdateChildUrl = Url.Action("GetYearlyPredictions");
                    break;

                case EPlannerView.Month:
                    plannerData.UpdateParentUrl = Url.Action("GetMonthlyPredictions");
                    plannerData.UpdateChildUrl = Url.Action("GetDailyPredictions");
                    break;

                case EPlannerView.Day:
                    plannerData.UpdateParentUrl = Url.Action("GetDailyPredictions");
                    plannerData.UpdateChildUrl = Url.Action("GetHourlyPredictions");
                    break;

                default:
                    plannerData.UpdateParentUrl = Url.Action("GetYearlyPredictions");
                    plannerData.UpdateChildUrl = Url.Action("GetMonthlyPredictions");
                    break;
            }
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

