using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/predictions")]
    public class MobilePredictionsController : MobileApiControllerBase
    {
        private readonly INineStarKiService _profiles;
        private readonly IAstronomyService _astronomy;
        private readonly IAstrologyService _astrology;

        public MobilePredictionsController(INineStarKiPackage package, INineStarKiService profiles,
            IAstronomyService astronomy, IAstrologyService astrology) : base(package)
        { _profiles = profiles; _astronomy = astronomy; _astrology = astrology; }

        [HttpPost, Route("calculate")]
        public ActionResult Calculate(MobileCalculationRequest request)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_calculation_request");
            return ApiSuccess(MobileCalculationMapper.Predictions(CalculateProfile(_profiles, request, true, true), request.Person.TimeOfBirthKnown));
        }

        [HttpGet, Route("me")]
        public ActionResult MyPredictions(DateTime? selectedDateTime = null)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            var request = ReadOwnCalculationRequest(userId);
            if (request == null) return ApiError(409, "birth_details_required");
            request.SelectedDateTime = selectedDateTime;
            if (!ValidInput(request)) return ApiError(409, "invalid_calculation_settings");
            return ApiSuccess(MobileCalculationMapper.Predictions(CalculateProfile(_profiles, request, true, true, true), request.Person.TimeOfBirthKnown));
        }

        [HttpPost, Route("planner")]
        public ActionResult Planner(MobilePlannerRequest request)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ValidInput(request) || !Enum.IsDefined(typeof(EPlannerView), request.View) ||
                !Enum.IsDefined(typeof(EScopeDisplay), request.Display) ||
                !Enum.IsDefined(typeof(EPlannerNavigationDirection), request.NavigationDirection))
                return ApiError(400, "invalid_planner_request");
            // Planner adds birth time internally. Do not call ToPerson() here.
            var planner = _profiles.GetPlannerData(DateTime.SpecifyKind(request.Person.DateOfBirth.Value.Date, DateTimeKind.Unspecified),
                request.Person.BirthTimeZoneId, request.Person.TimeOfBirth, request.Person.Gender,
                request.GetSelectedDate(GetUtcNow()), request.UserTimeZoneId, request.CalculationMethod,
                request.CalculatorType, EDisplayDataForPeriod.SelectedDate, request.HousesDisplay,
                request.InvertDailyAndHourlyKiForSouthernHemisphere, request.InvertDailyAndHourlyCycleKiForSouthernHemisphere,
                request.View, request.Display, request.NavigationDirection, null, request.IncludeMoonPhases);
            CombinedPredictionHelper.PopulatePlanner(planner, _astronomy);
            return ApiSuccess(MobileCalculationMapper.Planner(planner));
        }

        [HttpGet, Route("cycle")]
        public ActionResult Cycle(ENineStarKiEnergyCycleType period, int energy, EScopeDisplay display = EScopeDisplay.PersonalKi)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ModelState.IsValid || energy < 1 || energy > 9 || !Enum.IsDefined(typeof(EScopeDisplay), display))
                return ApiError(400, "invalid_cycle_request");
            var summary = _profiles.GetNineStarKiSummaryViewModel();
            IEnumerable<NineStarKiEnergy> energies;
            switch (period)
            {
                case ENineStarKiEnergyCycleType.EpochEnergy: energies = summary.EightyOneYearlyCycleEnergies; break;
                case ENineStarKiEnergyCycleType.GenerationalEnergy: energies = summary.NineYearlyCycleEnergies; break;
                case ENineStarKiEnergyCycleType.YearlyCycleEnergy: energies = summary.YearlyCycleEnergies; break;
                case ENineStarKiEnergyCycleType.MonthlyCycleEnergy: energies = summary.MonthlyCycleEnergies; break;
                case ENineStarKiEnergyCycleType.DailyEnergy: energies = summary.DailyCycleEnergies; break;
                case ENineStarKiEnergyCycleType.HourlyEnergy: energies = summary.HourlyCycleEnergies; break;
                default: return ApiError(400, "invalid_cycle_request");
            }
            return ApiSuccess(MobileCalculationMapper.Cycle(energies.First(e => e.EnergyNumber == energy), display == EScopeDisplay.GlobalKi));
        }

        [HttpGet, Route("combined")]
        public ActionResult Combined(int year, int month, int? day = null, int? afternoonDay = null,
            EScopeDisplay display = EScopeDisplay.PersonalKi)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ModelState.IsValid || year < 1 || year > 9 || month < 1 || month > 9 ||
                (day.HasValue && (day < 1 || day > 9)) ||
                (afternoonDay.HasValue && (!day.HasValue || afternoonDay < 1 || afternoonDay > 9)) ||
                !Enum.IsDefined(typeof(EScopeDisplay), display)) return ApiError(400, "invalid_combined_request");
            return ApiSuccess(MobileCalculationMapper.Combined(new CombinedPrediction(year, month, day, afternoonDay, display == EScopeDisplay.GlobalKi)));
        }

        [HttpPost, Route("moon-phase")]
        public ActionResult MoonPhase(MobileCalculationRequest request)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_calculation_request");
            var profile = CalculateProfile(_profiles, request);
            return ApiSuccess(MobileCalculationMapper.Moon(_astrology.GetMoonPhase(request.GetSelectedDate(GetUtcNow()), request.UserTimeZoneId, true, profile.MainEnergy)));
        }

        [HttpPost, Route("alchemy")]
        public async Task<ActionResult> Alchemy(MobileCalculationRequest request)
        {
            var error = RequireUser(out var userId, paid: true);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_calculation_request");
            if (!Package.SystemSettings.IsEnabledAlchemy) return ApiSuccess(new { Enabled = false });
            var result = await _profiles.GetNineStarKiPredictionsAlchemy(CalculateProfile(_profiles, request, true));
            return ApiSuccess(new { Enabled = true, result.AlchemisedDescription, result.AlchemisedSummary });
        }
    }
}
