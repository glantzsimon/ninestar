using K9.SharedLibrary.Helpers;
using K9.WebApplication.Constants;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using TimeZoneConverter;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/personal-chart")]
    public class MobilePersonalChartController : MobileApiControllerBase
    {
        private readonly INineStarKiService _nineStarKiService;

        public MobilePersonalChartController(INineStarKiPackage package, INineStarKiService nineStarKiService)
            : base(package)
        {
            _nineStarKiService = nineStarKiService;
        }

        [HttpGet]
        [Route("me")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult MyChart()
        {
            var userId = GetAuthenticatedUserId();
            if (userId <= 0)
                return ApiError(401, "authentication_required");

            // This first endpoint returns full profile details, including paid sections.
            // Free/complementary responses need their own field-level access policy.
            if (!Package.UserService.UserIsAdmin(userId) &&
                Package.MembershipService.GetActiveUserMembership(userId)?.IsAuthorisedToViewPaidContent() != true)
                return ApiError(403, "membership_required");

            var user = Package.UserService.Find(userId);
            var info = Package.UserInfosRepository.Find(e => e.UserId == userId).FirstOrDefault();
            if (user == null || info == null || user.BirthDate == default(DateTime) ||
                info.TimeOfBirth < TimeSpan.Zero || info.TimeOfBirth >= TimeSpan.FromDays(1))
                return ApiError(409, "birth_details_required");

            var users = Package.UserService;
            var timeZoneId = users.GetUserPreference(userId, SessionConstants.UserTimeZone, info.BirthTimeZoneId);
            if (string.IsNullOrWhiteSpace(timeZoneId) || string.IsNullOrWhiteSpace(info.BirthTimeZoneId))
                return ApiError(409, "timezone_required");

            DateTime selectedDate;
            try
            {
                // Validate both saved zones instead of accepting the helper's GMT fallback.
                TZConvert.GetTimeZoneInfo(DateTimeHelper.ValidateTimeZoneId(timeZoneId));
                TZConvert.GetTimeZoneInfo(DateTimeHelper.ValidateTimeZoneId(info.BirthTimeZoneId));
                selectedDate = DateTime.SpecifyKind(
                    DateTimeHelper.ConvertToLocaleDateTime(GetUtcNow(), timeZoneId), DateTimeKind.Unspecified);
            }
            catch (TimeZoneNotFoundException) { return ApiError(409, "invalid_timezone"); }
            catch (InvalidTimeZoneException) { return ApiError(409, "invalid_timezone"); }

            var calculationMethod = users.GetUserPreference(userId, SessionConstants.UserCalculationMethod, info.CalculationMethod);
            var calculatorType = users.GetUserPreference(userId, SessionConstants.DefaultCalculatorType, info.CalculatorType);
            var housesDisplay = users.GetUserPreference(userId, SessionConstants.UserHousesDisplay, info.HousesDisplay);
            if (!Enum.IsDefined(calculationMethod.GetType(), calculationMethod) ||
                !Enum.IsDefined(calculatorType.GetType(), calculatorType) ||
                !Enum.IsDefined(housesDisplay.GetType(), housesDisplay))
                return ApiError(409, "invalid_calculation_settings");

            var invertNatal = users.GetUserPreference(userId, SessionConstants.InvertDailyAndHourlyKiForSouthernHemisphere, info.InvertDailyAndHourlyKiForSouthernHemisphere);
            var invertCycles = users.GetUserPreference(userId, SessionConstants.InvertDailyAndHourlyCycleKiForSouthernHemisphere, info.InvertDailyAndHourlyKiForSouthernHemisphere);
            var birthTimeKnown = users.GetUserPreference(userId, SessionConstants.BirthTimeIsKnown, info.TimeOfBirth != TimeSpan.Zero);
            var person = new PersonModel
            {
                Name = user.FullName,
                // The profile service expects the birth time combined exactly once.
                DateOfBirth = DateTime.SpecifyKind(user.BirthDate.Date.Add(info.TimeOfBirth), DateTimeKind.Unspecified),
                TimeOfBirth = info.TimeOfBirth,
                BirthTimeZoneId = info.BirthTimeZoneId,
                Gender = user.Gender
            };
            var profile = _nineStarKiService.CalculateNineStarKiProfile(person,
                isCompatibility: false, isMyProfile: true, today: selectedDate,
                calculationMethod: calculationMethod, calculatorType: calculatorType,
                includeCycles: false, includePlannerData: false, userTimeZoneId: timeZoneId,
                housesDisplay: housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere: invertNatal,
                invertDailyAndHourlyCycleKiForSouthernHemisphere: invertCycles,
                displayDataForPeriod: EDisplayDataForPeriod.SelectedDate, includeMoonPhases: false);

            return ApiSuccess(MobilePersonalChartMapper.Map(profile, birthTimeKnown));
        }
    }
}
