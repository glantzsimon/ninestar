using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("karmic-numerology")]
    public partial class NumerologyController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly INumerologyService _numerologyService;

        public NumerologyController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, INumerologyService numerologyService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _numerologyService = numerologyService;
        }

        [Route("calculator")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None, VaryByHeader = "X-Requested-With")]
        public ActionResult _CalculatorForm(NumerologyModel model)
        {
            return PartialView(model);
        }

        [Route("calculator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NumerologyModel model)
        {
            if (model.PersonModel != null)
            {
                model = _numerologyService.Calculate(model);
            }
            return View("Index", model);
        }

        [Route("calculate-forecast")]
        [OutputCache(Duration = 2592000, VaryByParam = "forecastType;dateOfBirth;offset", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult CalculateForecast(EForecastType forecastType, DateTime dateOfBirth, int offset)
        {
            NumerologyForecast result;
            var personModel = new PersonModel { DateOfBirth = dateOfBirth };

            switch (forecastType)
            {
                case EForecastType.Daily:
                    result = _numerologyService.GetDailyForecast(personModel, offset);
                    break;

                case EForecastType.Monthly:
                    result = _numerologyService.GetMonthlyForecast(personModel, offset);
                    break;

                default:
                    result = _numerologyService.GetYearlyForecast(personModel, offset - 1);
                    break;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

