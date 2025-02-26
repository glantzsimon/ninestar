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
    public partial class ChakraCodesController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly IChakraCodesService _chakraCodesService;

        public ChakraCodesController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, IChakraCodesService chakraCodesService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _chakraCodesService = chakraCodesService;
        }

        [Route("calculator")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth
            };
            return View(new ChakraCodesModel(personModel));
        }

        [Route("calculator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ChakraCodesModel model)
        {
            if (model.PersonModel != null)
            {
                model = _chakraCodesService.CalculateChakraCodes(model);
            }
            return View("Index", model);
        }

        [Route("calculate-forecast")]
        public JsonResult CalculateForecast(EForecastType forecastType, DateTime dateOfBirth, int offset)
        {
            ChakraCodeForecast result;
            var personModel = new PersonModel { DateOfBirth = dateOfBirth };

            switch (forecastType)
            {
                case EForecastType.Daily:
                    result = _chakraCodesService.GetDailyForecast(personModel, offset);
                    break;

                case EForecastType.Monthly:
                    result = _chakraCodesService.GetMonthlyForecast(personModel, offset - 1);
                    break;

                default:
                    result = _chakraCodesService.GetYearlyForecast(personModel, offset - 1);
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

