using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System;

namespace K9.WebApplication.Services
{
    public interface INineStarKiService : IBaseService
    {
        NineStarKiModel CalculateNineStarKiProfile(DateTime dateOfBirth, EGender gender = EGender.Male);
        NineStarKiModel CalculateNineStarKiProfile(PersonModel personModel, DateTime today);
        NineStarKiModel CalculateNineStarKiProfile(PersonModel personModel, bool isCompatibility = false, bool isMyProfile = false, DateTime? today = null, bool inverteYinEnergies = true, bool includeCycles = true, bool useHolograhpicCycleCalculation = false,
            bool invertDailyAndHourlyKiForSouthernHemisphere = false);
        NineStarKiModel CalculateNineStarKiProfile(NineStarKiModel model, bool isCompatibility = false,
            bool isMyProfile = false, DateTime? today = null, bool includeCycles = true);

        NineStarKiSummaryViewModel GetNineStarKiSummaryViewModel();

        CompatibilityModel CalculateCompatibility(DateTime dateOfBirth1, EGender gender1, DateTime dateOfBirth2,
            EGender gender2);
        CompatibilityModel CalculateCompatibility(PersonModel personModel1, PersonModel personModel2, bool isHideSexuality);
    }
}