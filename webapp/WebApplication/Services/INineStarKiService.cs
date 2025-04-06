using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System;
using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface INineStarKiService : IBaseService
    {
        NineStarKiModel CalculateNineStarKiProfile(DateTime dateOfBirth, EGender gender = EGender.Male);

        NineStarKiModel CalculateNineStarKiProfile(PersonModel personModel, DateTime today);

        NineStarKiModel CalculateNineStarKiProfile(PersonModel personModel, bool isCompatibility = false,
            bool isMyProfile = false, DateTime? today = null,
            ECalculationMethod calculationMethod = ECalculationMethod.Chinese, bool includeCycles = false,
            bool includePlannerData = false, string userTimeZoneId = "", EHousesDisplay housesDisplay = EHousesDisplay.SolarHouse,
            bool invertDailyAndHourlyKiForSouthernHemisphere = false,
            bool invertDailyAndHourlyCycleKiForSouthernHemisphere = false,
            EDisplayDataForPeriod displayDataForPeriod = EDisplayDataForPeriod.SelectedDate);

        Task<NineStarKiModel> GetNineStarKiAlchemy(NineStarKiModel model);

        NineStarKiSummaryViewModel GetNineStarKiSummaryViewModel();
        CompatibilityModel CalculateCompatibility(DateTime dateOfBirth1, EGender gender1, DateTime dateOfBirth2,

            EGender gender2);

        CompatibilityModel CalculateCompatibility(PersonModel personModel1, PersonModel personModel2,
            bool isHideSexuality, ECalculationMethod calculationMethod = ECalculationMethod.Chinese);

        PlannerViewModel
            GetPlannerData(
                DateTime dateOfBirth,
                string birthTimeZoneId,
                TimeSpan timeOfBirth,
                EGender gender,
                DateTime selectedDateTime,
                string userTimeZoneId,
                ECalculationMethod calculationMethod,
                EDisplayDataForPeriod displayDataForPeriod,
                EHousesDisplay housesDisplay,
                bool invertDailyAndHourlyKiForSouthernHemisphere,
                bool invertDailyAndHourlyCycleKiForSouthernHemisphere,
                EPlannerView view = EPlannerView.Year,
                EScopeDisplay display = EScopeDisplay.PersonalKi,
                EPlannerNavigationDirection navigationDirection = EPlannerNavigationDirection.None,
                NineStarKiModel nineStarKiModel = null);
    }
}