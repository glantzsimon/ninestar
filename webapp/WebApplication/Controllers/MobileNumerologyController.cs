using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/numerology")]
    public class MobileNumerologyController : MobileApiControllerBase
    {
        private readonly INumerologyService _numerology;
        public MobileNumerologyController(INineStarKiPackage package, INumerologyService numerology) : base(package) { _numerology = numerology; }

        [HttpPost, Route("calculate")]
        public ActionResult Calculate(MobilePersonRequest request)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_person_request");
            return ApiSuccess(MobileCalculationMapper.Numerology(_numerology.Calculate(new NumerologyModel(request.ToPerson(false)))));
        }

        [HttpPost, Route("forecast")]
        public ActionResult Forecast(MobilePersonRequest request, EForecastType period = EForecastType.Daily, int offset = 0)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ValidInput(request) || (!Enum.IsDefined(typeof(EForecastType), period) || period == EForecastType.Unspecified) || offset < -120 || offset > 120)
                return ApiError(400, "invalid_forecast_request");
            var person = request.ToPerson(false);
            NumerologyForecast result;
            switch (period)
            {
                case EForecastType.Daily: result = _numerology.GetDailyForecast(person, offset); break;
                case EForecastType.Monthly: result = _numerology.GetMonthlyForecast(person, offset); break;
                // Match the website's yearly forecast offset contract.
                default: result = _numerology.GetYearlyForecast(person, offset - 1); break;
            }
            return ApiSuccess(MobileCalculationMapper.Forecast(result));
        }
    }
}
