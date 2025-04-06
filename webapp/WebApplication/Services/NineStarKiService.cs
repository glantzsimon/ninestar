using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public class NineStarKiService : BaseService, INineStarKiService
    {
        private readonly ISwissEphemerisService _swissEphemerisService;
        private readonly IAITextMergeService _textMergeService;

        public NineStarKiService(INineStarKiBasePackage my, ISwissEphemerisService swissEphemerisService, IAITextMergeService textMergeService) : base(my)
        {
            _swissEphemerisService = swissEphemerisService;
            _textMergeService = textMergeService;
        }

        public NineStarKiModel CalculateNineStarKiProfile(DateTime dateOfBirth, EGender gender = EGender.Male)
        {
            var cacheKey = $"CalculateNineStarKiProfile_{dateOfBirth.ToString()}_{gender}";
            return GetOrAddToCache(cacheKey, () =>
            {
                return CalculateNineStarKiProfile(new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    Gender = gender
                });

            }, TimeSpan.FromDays(30));
        }

        public NineStarKiModel CalculateNineStarKiProfile(PersonModel personModel, DateTime today)
        {
            return CalculateNineStarKiProfile(personModel, false, false, today);
        }

        public async Task<NineStarKiModel> CalculateNineStarKiProfileAsync(PersonModel personModel, bool isCompatibility = false,
            bool isMyProfile = false, DateTime? today = null,
            ECalculationMethod calculationMethod = ECalculationMethod.Chinese, bool includeCycles = false,
            bool includePlannerData = false, string userTimeZoneId = "",
            EHousesDisplay housesDisplay = EHousesDisplay.SolarHouse,
            bool invertDailyAndHourlyKiForSouthernHemisphere = false,
            bool invertDailyAndHourlyCycleKiForSouthernHemisphere = false,
            EDisplayDataForPeriod displayDataForPeriod = EDisplayDataForPeriod.SelectedDate)
        {
            var model = CalculateNineStarKiProfile(personModel, isCompatibility, isMyProfile, today,
                calculationMethod,
                includeCycles, includePlannerData, userTimeZoneId, housesDisplay,
                invertDailyAndHourlyKiForSouthernHemisphere,
                invertDailyAndHourlyCycleKiForSouthernHemisphere, displayDataForPeriod);

            model.AlchemisedSummary = await _textMergeService.MergeTextsIntoSummaryAsync(new[]
            {
                model.PersonalChartEnergies.Year.MainEnergySummary,
                model.PersonalChartEnergies.Year.IntellectualQualitiesSummary,
                model.PersonalChartEnergies.Year.InterpersonalQualitiesSummary,
                model.PersonalChartEnergies.Year.EmotionalLandscapeSummary,
                model.MainEnergyRelationshipsSummary,
                model.PersonalChartEnergies.Year.CareerSummary,
                model.PersonalChartEnergies.Year.Finances,
                model.PersonalChartEnergies.Year.Health,
                model.PersonalChartEnergies.Year.Spirituality,

                model.PersonalChartEnergies.Month.CharacterEnergySummary,
                model.PersonalChartEnergies.Month.ChildDescription,
                model.StressResponseDetails,
                
                model.PersonalChartEnergies.Surface.SurfaceEnergySummary,
                model.PersonalChartEnergies.Day.DayStarDescription,
                model.PersonalChartEnergies.Generation.GenerationDescription,
                model.PersonalChartEnergies.Epoch.EpochDescription,
            });

            model.AlchemisedDescription = await _textMergeService.MergeTextsAsync(new[]
            {
                model.AlchemisedSummary,
            });

            model.AIMergedProfileTextIsSet = true;

            return model;
        }

        public NineStarKiModel CalculateNineStarKiProfile(PersonModel personModel, bool isCompatibility = false,
            bool isMyProfile = false, DateTime? today = null, ECalculationMethod calculationMethod = ECalculationMethod.Chinese, bool includeCycles = false, bool includePlannerData = false, string userTimeZoneId = "", EHousesDisplay housesDisplay = EHousesDisplay.SolarHouse, bool invertDailyAndHourlyKiForSouthernHemisphere = false, bool invertDailyAndHourlyCycleKiForSouthernHemisphere = false, EDisplayDataForPeriod displayDataForPeriod = EDisplayDataForPeriod.SelectedDate)
        {
            var cacheKey = $"CalculateNineStarKiProfileFromModel_{personModel.DateOfBirth:yyyyMMddHHmm}_{personModel.TimeOfBirth.ToString()}_{personModel.BirthTimeZoneId}_{personModel.Name}_{personModel.Gender}_{isCompatibility}_{isMyProfile}_{calculationMethod}_{includeCycles}_{housesDisplay}_{today:yyyyMMddHHmm}_{invertDailyAndHourlyKiForSouthernHemisphere}_{invertDailyAndHourlyCycleKiForSouthernHemisphere}_{displayDataForPeriod}_{userTimeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                var selectedDateTime = displayDataForPeriod == EDisplayDataForPeriod.Now || today == null
                    ? DateTime.UtcNow
                    : today.Value;
                NineStarKiModel model = null;

                var preciseEpochEnergy = _swissEphemerisService.GetNineStarKiEightyOneYearKi(personModel.DateOfBirth, personModel.BirthTimeZoneId);
                var preciseGenerationalEnergy = _swissEphemerisService.GetNineStarKiNineYearKi(personModel.DateOfBirth, personModel.BirthTimeZoneId);
                var preciseMainEnergy = _swissEphemerisService.GetNineStarKiYearlyKi(personModel.DateOfBirth, personModel.BirthTimeZoneId);
                var preciseEmotionalEnergy = _swissEphemerisService.GetNineStarKiMonthlyKi(personModel.DateOfBirth, personModel.BirthTimeZoneId);
                var preciseEmotionalEnergyForInvertedYear = _swissEphemerisService.GetNineStarKiMonthlyKi(personModel.DateOfBirth, personModel.BirthTimeZoneId, true);
                var preciseDayStarEnergy = _swissEphemerisService.GetNineStarKiDailyKi(personModel.DateOfBirth, personModel.BirthTimeZoneId);
                var preciseHourlyEnergy = _swissEphemerisService.GetNineStarKiHourlyKi(personModel.DateOfBirth, personModel.BirthTimeZoneId);

                if (includeCycles)
                {
                    userTimeZoneId = displayDataForPeriod == EDisplayDataForPeriod.Now ? "" : string.IsNullOrEmpty(userTimeZoneId) ? personModel.BirthTimeZoneId : userTimeZoneId;

                    var preciseEightyOneYearEnergy =
                        _swissEphemerisService.GetNineStarKiEightyOneYearKi(selectedDateTime, userTimeZoneId);
                    var preciseNineYearEnergy =
                        _swissEphemerisService.GetNineStarKiNineYearKi(selectedDateTime, userTimeZoneId);
                    var preciseYearEnergy =
                        _swissEphemerisService.GetNineStarKiYearlyKi(selectedDateTime, userTimeZoneId);
                    var preciseMonthEnergy =
                        _swissEphemerisService.GetNineStarKiMonthlyKi(selectedDateTime, userTimeZoneId);
                    var preciseDailyEnergies =
                        _swissEphemerisService.GetNineStarKiDailyKis(selectedDateTime, userTimeZoneId);
                    var preciseHourlyCycleEnergy =
                        _swissEphemerisService.GetNineStarKiHourlyKi(selectedDateTime, userTimeZoneId);

                    model = new NineStarKiModel(personModel, preciseEpochEnergy, preciseGenerationalEnergy,
                        preciseMainEnergy, preciseEmotionalEnergy, preciseEmotionalEnergyForInvertedYear,
                        preciseDayStarEnergy.DailyKi, preciseHourlyEnergy,
                        preciseEightyOneYearEnergy, preciseNineYearEnergy, preciseYearEnergy, preciseMonthEnergy,
                        preciseDailyEnergies, preciseHourlyCycleEnergy, selectedDateTime, calculationMethod, housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere, userTimeZoneId, displayDataForPeriod);
                }
                else
                {
                    model = new NineStarKiModel(personModel, preciseEpochEnergy, preciseGenerationalEnergy,
                        preciseMainEnergy, preciseEmotionalEnergy, preciseEmotionalEnergyForInvertedYear,
                        preciseDayStarEnergy.DailyKi, preciseHourlyEnergy,
                        5, 5, 5, 5, new (int DailyKi, int? InvertedDailyKi)[] { (5, 5), (5, 5) }, 5, selectedDateTime, calculationMethod, housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere, userTimeZoneId, displayDataForPeriod);
                }

                model.Lichun = _swissEphemerisService.GetLichun(DateTime.UtcNow, "");

                model.YearlyDirections = GetYearlyDirections(model);
                model.MonthlyDirections = GetMonthlyDirections(model);

                if (isCompatibility)
                {
                    model.IsShowSummary = false;
                }

                model.IsMyProfile = isMyProfile;
                model.IsProcessed = true;
                model.IsCompatibility = isCompatibility;

                return model;

            }, TimeSpan.FromDays(30));
        }

        public CompatibilityModel CalculateCompatibility(DateTime dateOfBirth1, EGender gender1, DateTime dateOfBirth2,
            EGender gender2)
        {
            var cacheKey = $"CalculateCompatibility_{dateOfBirth1.ToString()}_{gender1}_{dateOfBirth2.ToString()}_{gender2}";
            return GetOrAddToCache(cacheKey, () =>
            {
                return CalculateCompatibility(
                    new PersonModel
                    {
                        DateOfBirth = dateOfBirth1,
                        Gender = gender1
                    }, new PersonModel
                    {
                        DateOfBirth = dateOfBirth2,
                        Gender = gender2
                    }, false);

            }, TimeSpan.FromDays(30));
        }

        public CompatibilityModel CalculateCompatibility(PersonModel personModel1, PersonModel personModel2,
            bool isHideSexuality, ECalculationMethod calculationMethod = ECalculationMethod.Chinese)
        {
            var cacheKey = $"CalculateCompatibilityFromModel_{personModel1.DateOfBirth.ToString()}_{personModel1.Name}_{personModel1.Gender}_{personModel2.DateOfBirth.ToString()}_{personModel2.Name}_{personModel2.Gender}";
            return GetOrAddToCache(cacheKey, () =>
            {
                var nineStarKiModel1 = CalculateNineStarKiProfile(personModel1, true, false, null, calculationMethod);
                var nineStarKiModel2 = CalculateNineStarKiProfile(personModel2, true, false, null, calculationMethod);

                var model = new CompatibilityModel(nineStarKiModel1, nineStarKiModel2)
                {
                    IsProcessed = true,
                    IsHideSexualChemistry = isHideSexuality
                };

                if (string.IsNullOrEmpty(nineStarKiModel1.PersonModel.Name))
                {
                    nineStarKiModel1.PersonModel.Name = Dictionary.FirstPerson;
                }

                if (string.IsNullOrEmpty(nineStarKiModel2.PersonModel.Name))
                {
                    nineStarKiModel2.PersonModel.Name = Dictionary.SecondPerson;
                }

                return model;
            }, TimeSpan.FromDays(30));
        }

        public NineStarKiSummaryViewModel GetNineStarKiSummaryViewModel()
        {
            return GetOrAddToCache("GetNineStarKiSummaryViewModel", () =>
            {
                var eightyOneYearEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Soil, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Thunder, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Heaven, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Mountain, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.EpochEnergy}
                };

                var nineYearEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Soil, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Thunder, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Heaven, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Mountain, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy}
                };

                var yearEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Soil, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Thunder, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Heaven, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Mountain, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.MainEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy}
                };

                var monthEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Soil, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Thunder, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Heaven, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Mountain, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.CharacterEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy}
                };

                var dailyEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Soil, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Thunder, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Heaven, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Mountain, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.DailyEnergy}
                };

                var hourlyEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Soil, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Thunder, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Heaven, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Mountain, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy},
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.HourlyEnergy}
                };

                var dynamicEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Thunder, ENineStarKiEnergyType.MainEnergy),
                    new NineStarKiEnergy(ENineStarKiEnergy.Heaven, ENineStarKiEnergyType.MainEnergy),
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.MainEnergy)
                };
                var staticEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Soil, ENineStarKiEnergyType.MainEnergy),
                    new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyType.MainEnergy),
                    new NineStarKiEnergy(ENineStarKiEnergy.Mountain, ENineStarKiEnergyType.MainEnergy)
                };
                var reflectiveEnergies = new List<NineStarKiEnergy>
                {
                    new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.MainEnergy),
                    new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.MainEnergy),
                    new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.MainEnergy)
                };

                return new NineStarKiSummaryViewModel(eightyOneYearEnergies, nineYearEnergies, yearEnergies,
                    monthEnergies, dailyEnergies, hourlyEnergies, dynamicEnergies, staticEnergies,
                    reflectiveEnergies);
            }, TimeSpan.FromDays(30));
        }

        public PlannerViewModel
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
                NineStarKiModel nineStarKiModel = null)
        {
            return GetOrAddToCache($"GetPlannerData_{view.ToString()}_{dateOfBirth:yyyyMMddHHmm}_{timeOfBirth.ToString()}_" +
                                   $"{gender}_{selectedDateTime:yyyyMMddHHmm}_{userTimeZoneId}_{calculationMethod}_{displayDataForPeriod}" +
                                   $"{userTimeZoneId}_{housesDisplay}_" +
                                   $"{invertDailyAndHourlyKiForSouthernHemisphere}_" +
                                   $"{invertDailyAndHourlyCycleKiForSouthernHemisphere}_" +
                                   $"{display}_{navigationDirection}", () =>
            {
                var energies = new List<PlannerViewModelItem>();
                var lichun = _swissEphemerisService.GetLichun(selectedDateTime, userTimeZoneId);
                // Add time of birth
                dateOfBirth = dateOfBirth.Add(timeOfBirth);
                nineStarKiModel = nineStarKiModel ?? CalculateNineStarKiProfile(new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    BirthTimeZoneId = birthTimeZoneId,
                    TimeOfBirth = timeOfBirth,
                    Gender = gender
                }, false, false, selectedDateTime, calculationMethod, true, false, userTimeZoneId,
                    housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere);

                var plannerModel = new PlannerViewModel
                {
                    View = view,
                    Display = display,
                    HousesDisplay = housesDisplay,
                    NineStarKiModel = nineStarKiModel,
                    Lichun = lichun,
                    SelectedDateTime = selectedDateTime
                };

                if (view == EPlannerView.Day)
                {
                    plannerModel.MoonPhase = _swissEphemerisService.GetMoonIlluminationPercentage(selectedDateTime, userTimeZoneId);
                }

                var localNow = nineStarKiModel.SelectedDate.Value;

                switch (view)
                {
                    case EPlannerView.EightyOneYear:
                        var eightyOneYearPeriod = _swissEphemerisService.GetNineStarKiEightyOneYearPeriod(selectedDateTime, userTimeZoneId);

                        if (navigationDirection == EPlannerNavigationDirection.Forward)
                        {
                            eightyOneYearPeriod =
                                _swissEphemerisService.GetNineStarKiEightyOneYearPeriod(
                                    eightyOneYearPeriod.PeriodEndsOn.AddDays(3), userTimeZoneId);
                            selectedDateTime = selectedDateTime.AddYears(81);
                        }
                        else if (navigationDirection == EPlannerNavigationDirection.Back)
                        {
                            eightyOneYearPeriod =
                                _swissEphemerisService.GetNineStarKiEightyOneYearPeriod(
                                    eightyOneYearPeriod.PeriodStartsOn.AddDays(-3), userTimeZoneId);
                            selectedDateTime = selectedDateTime.AddYears(-81);
                        }

                        if (!selectedDateTime.IsBetween(eightyOneYearPeriod.PeriodStartsOn,
                            eightyOneYearPeriod.PeriodEndsOn))
                        {
                            selectedDateTime = eightyOneYearPeriod.PeriodStartsOn;
                        }

                        var nineYearPeriodsForPeriod =
                            _swissEphemerisService.GetNineStarKiNineYearPeriodsWithinEightyOneYearPeriod(eightyOneYearPeriod.PeriodStartsOn.AddDays(3), userTimeZoneId);

                        foreach (var nineYearPeriodSlot in nineYearPeriodsForPeriod)
                        {
                            var energy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.GetPersonalCycleEnergy(nineYearPeriodSlot.NineYearKi,
                                        ENineStarKiEnergyCycleType.GenerationalEnergy)

                                : nineStarKiModel.GetGlobalCycleEnergy(nineYearPeriodSlot.NineYearKi, ENineStarKiEnergyCycleType.GenerationalEnergy);

                            var isActive =
                                localNow.Date.IsBetween(nineYearPeriodSlot.PeriodStartsOn, nineYearPeriodSlot.PeriodEndsOn);

                            energies.Add(new PlannerViewModelItem(energy, energy, nineYearPeriodSlot.PeriodStartsOn, nineYearPeriodSlot.PeriodEndsOn, isActive, EPlannerView.NineYear));
                        }

                        if (navigationDirection != EPlannerNavigationDirection.None)
                        {
                            nineStarKiModel = CalculateNineStarKiProfile(new PersonModel
                            {
                                DateOfBirth = dateOfBirth,
                                BirthTimeZoneId = birthTimeZoneId,
                                TimeOfBirth = timeOfBirth,
                                Gender = gender
                            }, false, false, eightyOneYearPeriod.PeriodStartsOn.AddDays(3), calculationMethod, true, false, userTimeZoneId,
                                                  housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere, EDisplayDataForPeriod.SelectedDate);
                        }

                        plannerModel.Energy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.PersonalHousesOccupiedEnergies.Epoch : nineStarKiModel.GlobalCycleEnergies.Epoch;
                        plannerModel.PeriodStarsOn = eightyOneYearPeriod.PeriodStartsOn;
                        plannerModel.PeriodEndsOn = eightyOneYearPeriod.PeriodEndsOn;
                        plannerModel.Energies = energies;

                        break;

                    case EPlannerView.NineYear:
                        var nineYearPeriod = _swissEphemerisService.GetNineStarKiNineYearPeriod(selectedDateTime, userTimeZoneId);

                        if (navigationDirection == EPlannerNavigationDirection.Forward)
                        {
                            nineYearPeriod = _swissEphemerisService.GetNineStarKiNineYearPeriod(nineYearPeriod.PeriodEndsOn.AddDays(3), userTimeZoneId);
                            selectedDateTime = selectedDateTime.AddYears(9);
                        }
                        else if (navigationDirection == EPlannerNavigationDirection.Back)
                        {
                            nineYearPeriod = _swissEphemerisService.GetNineStarKiNineYearPeriod(nineYearPeriod.PeriodStartsOn.AddDays(-3), userTimeZoneId);
                            selectedDateTime = selectedDateTime.AddYears(-9);
                        }

                        if (!selectedDateTime.IsBetween(nineYearPeriod.PeriodStartsOn,
                            nineYearPeriod.PeriodEndsOn))
                        {
                            selectedDateTime = nineYearPeriod.PeriodStartsOn;
                        }

                        var yearsForNineYearPeriod =
                            _swissEphemerisService.GetNineStarKiYearlyPeriodsForNineYearPeriod(nineYearPeriod.PeriodStartsOn.AddDays(3), userTimeZoneId);

                        foreach (var year in yearsForNineYearPeriod)
                        {
                            var energy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.GetPersonalCycleEnergy(year.YearlyKi, ENineStarKiEnergyCycleType.YearlyCycleEnergy) : nineStarKiModel.GetGlobalCycleEnergy(year.YearlyKi, ENineStarKiEnergyCycleType.YearlyCycleEnergy);

                            var isActive =
                                localNow.Date.IsBetween(year.PeriodStartsOn, year.PeriodEndsOn);

                            energies.Add(new PlannerViewModelItem(energy, energy, year.PeriodStartsOn, year.PeriodEndsOn, isActive, EPlannerView.Year));
                        }

                        if (navigationDirection != EPlannerNavigationDirection.None)
                        {
                            nineStarKiModel = CalculateNineStarKiProfile(new PersonModel
                            {
                                DateOfBirth = dateOfBirth,
                                BirthTimeZoneId = birthTimeZoneId,
                                TimeOfBirth = timeOfBirth,
                                Gender = gender
                            }, false, false, nineYearPeriod.PeriodStartsOn.AddDays(3), calculationMethod, true, false, userTimeZoneId,
                                housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere, EDisplayDataForPeriod.SelectedDate);
                        }

                        plannerModel.Energy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.PersonalHousesOccupiedEnergies.Generation : nineStarKiModel.GlobalCycleEnergies.Generation;
                        plannerModel.PeriodStarsOn = nineYearPeriod.PeriodStartsOn;
                        plannerModel.PeriodEndsOn = nineYearPeriod.PeriodEndsOn;
                        plannerModel.Energies = energies;

                        break;

                    case EPlannerView.Month:
                        var selectedMonthPeriod = _swissEphemerisService.GetNineStarKiMonthlyPeriodBoundaries(selectedDateTime, userTimeZoneId);

                        if (navigationDirection == EPlannerNavigationDirection.Forward)
                        {
                            selectedMonthPeriod = _swissEphemerisService.GetNineStarKiMonthlyPeriodBoundaries(selectedMonthPeriod.PeriodEndsOn.AddDays(3), userTimeZoneId);

                            selectedDateTime = selectedDateTime.AddMonths(1);
                        }
                        else if (navigationDirection == EPlannerNavigationDirection.Back)
                        {
                            selectedMonthPeriod = _swissEphemerisService.GetNineStarKiMonthlyPeriodBoundaries(selectedMonthPeriod.PeriodStartsOn.AddDays(-3), userTimeZoneId);

                            selectedDateTime = selectedDateTime.AddMonths(-1);
                        }

                        if (!selectedDateTime.IsBetween(selectedMonthPeriod.PeriodStartsOn,
                            selectedMonthPeriod.PeriodEndsOn))
                        {
                            selectedDateTime = selectedMonthPeriod.PeriodStartsOn;
                        }

                        var dailyPeriods =
                            _swissEphemerisService.GetNineStarKiDailyEnergiesForMonth(selectedMonthPeriod.PeriodStartsOn.AddDays(3), userTimeZoneId);

                        foreach (var dailyEnergy in dailyPeriods)
                        {
                            var preciseDailyCycleMorningEnergy = invertDailyAndHourlyCycleKiForSouthernHemisphere
                                ? NineStarKiModel.GetOppositeEnergyInMagicSquare(dailyEnergy.MorningKi)
                                : dailyEnergy.MorningKi;

                            var preciseInvertedDailyCycleMorningEnergy = dailyEnergy.InvertedMorningKi.HasValue && invertDailyAndHourlyCycleKiForSouthernHemisphere
                                ? NineStarKiModel.GetOppositeEnergyInMagicSquare(dailyEnergy.InvertedMorningKi.Value)
                                : dailyEnergy.InvertedMorningKi;

                            var preciseDailyCycleAfternoonEnergy = dailyEnergy.AfternoonKi.HasValue && invertDailyAndHourlyCycleKiForSouthernHemisphere
                                ? NineStarKiModel.GetOppositeEnergyInMagicSquare(dailyEnergy.AfternoonKi.Value)
                                : dailyEnergy.AfternoonKi;

                            var preciseInvertedDailyCycleAfternoonEnergy = dailyEnergy.InvertedAfternoonKi.HasValue && invertDailyAndHourlyCycleKiForSouthernHemisphere
                                ? NineStarKiModel.GetOppositeEnergyInMagicSquare(dailyEnergy.InvertedAfternoonKi.Value)
                                : dailyEnergy.InvertedAfternoonKi;

                            var morningEnergy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.GetPersonalCycleEnergy(preciseDailyCycleMorningEnergy, ENineStarKiEnergyCycleType.DailyEnergy) : nineStarKiModel.GetGlobalCycleEnergy(preciseDailyCycleMorningEnergy, ENineStarKiEnergyCycleType.DailyEnergy);

                            var afternoonEnergy = display == EScopeDisplay.PersonalKi ? preciseDailyCycleAfternoonEnergy.HasValue ? nineStarKiModel.GetPersonalCycleEnergy(preciseDailyCycleAfternoonEnergy.Value, ENineStarKiEnergyCycleType.DailyEnergy) : morningEnergy : nineStarKiModel.GetGlobalCycleEnergy(preciseDailyCycleAfternoonEnergy.Value, ENineStarKiEnergyCycleType.DailyEnergy);

                            var isActive = dailyEnergy.Day.Date == localNow.Date;

                            var moonPhase = _swissEphemerisService.GetMoonIlluminationPercentage(dailyEnergy.Day.Date, userTimeZoneId);

                            energies.Add(new PlannerViewModelItem(morningEnergy, afternoonEnergy, dailyEnergy.Day, dailyEnergy.Day, isActive, EPlannerView.Day, moonPhase));
                        }

                        if (navigationDirection != EPlannerNavigationDirection.None)
                        {
                            nineStarKiModel = CalculateNineStarKiProfile(new PersonModel
                            {
                                DateOfBirth = dateOfBirth,
                                BirthTimeZoneId = birthTimeZoneId,
                                TimeOfBirth = timeOfBirth,
                                Gender = gender
                            }, false, false, selectedDateTime, calculationMethod, true, false, userTimeZoneId,
                                housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere,
                                EDisplayDataForPeriod.SelectedDate);
                        }

                        plannerModel.Energy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.PersonalHousesOccupiedEnergies.Month : nineStarKiModel.GlobalCycleEnergies.Month;
                        plannerModel.PeriodStarsOn = selectedMonthPeriod.PeriodStartsOn;
                        plannerModel.PeriodEndsOn = selectedMonthPeriod.PeriodEndsOn;
                        plannerModel.Energies = energies;

                        break;

                    case EPlannerView.Day:

                        if (navigationDirection == EPlannerNavigationDirection.Forward)
                        {
                            selectedDateTime = selectedDateTime.AddDays(1);
                        }
                        else if (navigationDirection == EPlannerNavigationDirection.Back)
                        {
                            selectedDateTime = selectedDateTime.AddDays(-1);
                        }

                        var hourlyPeriods =
                            _swissEphemerisService.GetNineStarKiHourlyPeriodsForDay(selectedDateTime, userTimeZoneId);

                        foreach (var hourlyPeriod in hourlyPeriods)
                        {
                            var preciseHourlyCycleEnergy = invertDailyAndHourlyCycleKiForSouthernHemisphere
                                ? NineStarKiModel.GetOppositeEnergyInMagicSquare(hourlyPeriod.HourlyKi)
                                : hourlyPeriod.HourlyKi;

                            var presonalHourlyEnergy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.GetPersonalCycleEnergy(preciseHourlyCycleEnergy, ENineStarKiEnergyCycleType.HourlyEnergy) : nineStarKiModel.GetGlobalCycleEnergy(preciseHourlyCycleEnergy, ENineStarKiEnergyCycleType.HourlyEnergy);

                            var isActive = localNow.IsBetween(hourlyPeriod.SegmentStartsOn, hourlyPeriod.SegmentEndsOn);

                            energies.Add(new PlannerViewModelItem(presonalHourlyEnergy, presonalHourlyEnergy, hourlyPeriod.SegmentStartsOn, hourlyPeriod.SegmentEndsOn, isActive, EPlannerView.Day));
                        }

                        if (navigationDirection != EPlannerNavigationDirection.None)
                        {
                            nineStarKiModel = CalculateNineStarKiProfile(new PersonModel
                            {
                                DateOfBirth = dateOfBirth,
                                BirthTimeZoneId = birthTimeZoneId,
                                TimeOfBirth = timeOfBirth,
                                Gender = gender
                            }, false, false, selectedDateTime, calculationMethod, true, false, userTimeZoneId,
                                housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere,
                                EDisplayDataForPeriod.SelectedDate);
                        }

                        plannerModel.Energy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.PersonalHousesOccupiedEnergies.Day : nineStarKiModel.GlobalCycleEnergies.Day;
                        plannerModel.PeriodStarsOn = selectedDateTime.Date;
                        plannerModel.PeriodEndsOn = selectedDateTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                        plannerModel.Energies = energies;

                        break;

                    // Year
                    default:
                        var yearlyPeriod = _swissEphemerisService.GetNineStarKiYearlyPeriod(selectedDateTime, userTimeZoneId);

                        if (navigationDirection == EPlannerNavigationDirection.Forward)
                        {
                            yearlyPeriod = _swissEphemerisService.GetNineStarKiYearlyPeriod(yearlyPeriod.PeriodEndsOn.AddDays(3), userTimeZoneId);

                            selectedDateTime = selectedDateTime.AddYears(1);
                        }
                        else if (navigationDirection == EPlannerNavigationDirection.Back)
                        {
                            yearlyPeriod = _swissEphemerisService.GetNineStarKiYearlyPeriod(yearlyPeriod.PeriodStartsOn.AddDays(-3), userTimeZoneId);

                            selectedDateTime = selectedDateTime.AddYears(-1);
                        }

                        if (!selectedDateTime.IsBetween(yearlyPeriod.PeriodStartsOn,
                            yearlyPeriod.PeriodEndsOn))
                        {
                            selectedDateTime = yearlyPeriod.PeriodStartsOn;
                        }

                        var monthlyPeriodsForYear =
                            _swissEphemerisService.GetNineStarKiMonthlyPeriods(yearlyPeriod.PeriodStartsOn.AddDays(3), userTimeZoneId);

                        foreach (var monthlyPeriod in monthlyPeriodsForYear)
                        {
                            var energy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.GetPersonalCycleEnergy(monthlyPeriod.MonthlyKi, ENineStarKiEnergyCycleType.MonthlyCycleEnergy) : nineStarKiModel.GetGlobalCycleEnergy(monthlyPeriod.MonthlyKi, ENineStarKiEnergyCycleType.MonthlyCycleEnergy);

                            var isActive =
                                localNow.Date.IsBetween(monthlyPeriod.PeriodStartsOn, monthlyPeriod.PeriodEndsOn);

                            energies.Add(new PlannerViewModelItem(energy, energy, monthlyPeriod.PeriodStartsOn, monthlyPeriod.PeriodEndsOn, isActive, EPlannerView.Month));
                        }

                        if (navigationDirection != EPlannerNavigationDirection.None)
                        {
                            nineStarKiModel = CalculateNineStarKiProfile(new PersonModel
                            {
                                DateOfBirth = dateOfBirth,
                                BirthTimeZoneId = birthTimeZoneId,
                                TimeOfBirth = timeOfBirth,
                                Gender = gender
                            }, false, false, selectedDateTime, calculationMethod, true, false, userTimeZoneId,
                                housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere,
                                EDisplayDataForPeriod.SelectedDate);
                        }

                        plannerModel.Energy = display == EScopeDisplay.PersonalKi ? nineStarKiModel.PersonalHousesOccupiedEnergies.Year : nineStarKiModel.GlobalCycleEnergies.Year;
                        plannerModel.PeriodStarsOn = yearlyPeriod.PeriodStartsOn;
                        plannerModel.PeriodEndsOn = yearlyPeriod.PeriodEndsOn;
                        plannerModel.Energies = energies;

                        break;
                }

                // Update selected time (in case of navigation)
                plannerModel.SelectedDateTime = selectedDateTime;

                return plannerModel;

            }, TimeSpan.FromDays(30));
        }

        private NineStarKiEnergy GetInvertedEnergy(NineStarKiEnergy energy)
        {
            return new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(energy.EnergyNumber));
        }

        private NineStarKiDirections GetYearlyDirections(NineStarKiModel model)
        {
            var houseOfFive = model.GlobalCycleEnergies.Year.GetHouseOfFive();

            return new NineStarKiDirections(houseOfFive,
                GetInvertedEnergy(houseOfFive),
                model.PersonalHousesOccupiedEnergies.Year,
                GetInvertedEnergy(model.PersonalHousesOccupiedEnergies.Year),
                model.PersonalChartEnergies.Year);
        }

        private NineStarKiDirections GetMonthlyDirections(NineStarKiModel model)
        {
            var houseOfFive = model.GlobalCycleEnergies.Month.GetHouseOfFive();

            return new NineStarKiDirections(houseOfFive,
                GetInvertedEnergy(houseOfFive),
                model.PersonalHousesOccupiedEnergies.Month,
                GetInvertedEnergy(model.PersonalHousesOccupiedEnergies.Month),
                model.PersonalChartEnergies.Month);
        }
    }
}