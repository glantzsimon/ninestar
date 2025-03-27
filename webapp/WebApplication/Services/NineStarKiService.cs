using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Services
{
    public class NineStarKiService : BaseService, INineStarKiService
    {
        private readonly ISwissEphemerisService _swissEphemerisService;

        public NineStarKiService(INineStarKiBasePackage my, ISwissEphemerisService swissEphemerisService) : base(my)
        {
            _swissEphemerisService = swissEphemerisService;
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

        public NineStarKiModel CalculateNineStarKiProfile(PersonModel personModel, bool isCompatibility = false,
            bool isMyProfile = false, DateTime? today = null, ECalculationMethod calculationMethod = ECalculationMethod.Chinese, bool includeCycles = false, bool includePlannerData = false, string userTimeZoneId = "", bool useHolograhpicCycleCalculation = false, bool invertDailyAndHourlyKiForSouthernHemisphere = false, bool invertDailyAndHourlyCycleKiForSouthernHemisphere = false, EDisplayDataForPeriod displayDataForPeriod = EDisplayDataForPeriod.Now)
        {
            var cacheKey = $"CalculateNineStarKiProfileFromModel_{personModel.DateOfBirth:yyyyMMddHHmm}_{personModel.TimeOfBirth.ToString()}_{personModel.BirthTimeZoneId}_{personModel.Name}_{personModel.Gender}_{isCompatibility}_{isMyProfile}_{calculationMethod}_{includeCycles}_{useHolograhpicCycleCalculation}_{today:yyyyMMddHHmm}_{invertDailyAndHourlyKiForSouthernHemisphere}";
            return GetOrAddToCache(cacheKey, () =>
            {
                var selectedDateTime = today == null
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
                        preciseDailyEnergies, preciseHourlyCycleEnergy, selectedDateTime, calculationMethod, useHolograhpicCycleCalculation, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere);
                }
                else
                {
                    model = new NineStarKiModel(personModel, preciseEpochEnergy, preciseGenerationalEnergy,
                        preciseMainEnergy, preciseEmotionalEnergy, preciseEmotionalEnergyForInvertedYear,
                        preciseDayStarEnergy.DailyKi, preciseHourlyEnergy,
                        5, 5, 5, 5, new (int DailyKi, int? InvertedDailyKi)[] { (5, 5), (5, 5) }, 5, selectedDateTime, calculationMethod, useHolograhpicCycleCalculation, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere);
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
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.EpochEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy}
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
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.GenerationalEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy}
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
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.DailyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy}
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
                    new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.HourlyEnergy){EnergyCycleType = ENineStarKiEnergyCycleType.YearlyCycleEnergy}
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
                ECalculationMethod calculationMethod,
                string userTimeZoneId,
                bool useHolograhpicCycleCalculation,
                bool invertDailyAndHourlyKiForSouthernHemisphere,
                bool invertDailyAndHourlyCycleKiForSouthernHemisphere,
                EPlannerView view = EPlannerView.Year,
                NineStarKiModel nineStarKiModel = null)
        {
            return GetOrAddToCache($"GetPlannerData_{view.ToString()}_{dateOfBirth:yyyyMMddHHmm}_{timeOfBirth.ToString()}_" +
                                   $"{gender}_{selectedDateTime:yyyyMMddHHmm}_{userTimeZoneId}_{calculationMethod}_" +
                                   $"{userTimeZoneId}_{useHolograhpicCycleCalculation}_" +
                                   $"{invertDailyAndHourlyKiForSouthernHemisphere}_" +
                                   $"{invertDailyAndHourlyCycleKiForSouthernHemisphere}", () =>
            {
                var energies = new List<(NineStarKiEnergy Energy, NineStarKiEnergy SecondEnergy, DateTime EnergyStartsOn, DateTime EnergyEndsOn, bool IsSelected)>();
                var lichun = _swissEphemerisService.GetLichun(selectedDateTime, userTimeZoneId);
                // Add time of birth
                dateOfBirth = dateOfBirth.Add(timeOfBirth);
                nineStarKiModel = nineStarKiModel ?? CalculateNineStarKiProfile(new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    BirthTimeZoneId = birthTimeZoneId,
                    TimeOfBirth = timeOfBirth,
                    Gender = gender
                }, false, false, selectedDateTime, calculationMethod, false, false, userTimeZoneId,
                    useHolograhpicCycleCalculation, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere);

                switch (view)
                {

                    case EPlannerView.Month:
                        var selectedMonthPeriod = _swissEphemerisService.GetNineStarKiMonthlyPeriodBoundaries(selectedDateTime, userTimeZoneId);
                        var dailyPeriods =
                            _swissEphemerisService.GetNineStarKiDailyEnergiesForMonth(selectedDateTime, userTimeZoneId);

                        foreach (var dailyEnergy in dailyPeriods)
                        {
                            var morningEnergy = nineStarKiModel.GetPersonalCycleEnergy(dailyEnergy.MorningKi, useHolograhpicCycleCalculation ? nineStarKiModel.PersonalChartEnergies.Day.EnergyNumber : nineStarKiModel.MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy);

                            var afternoonEnergy = dailyEnergy.AfternoonKi.HasValue ? nineStarKiModel.GetPersonalCycleEnergy(dailyEnergy.AfternoonKi.Value, useHolograhpicCycleCalculation ? nineStarKiModel.PersonalChartEnergies.Day.EnergyNumber : nineStarKiModel.MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy) : morningEnergy;

                            var isSelected = dailyEnergy.Day.Date == selectedDateTime.Date;

                            energies.Add((morningEnergy, afternoonEnergy, dailyEnergy.Day, dailyEnergy.Day, isSelected));
                        }

                        return new PlannerViewModel
                        {
                            View = view,
                            NineStarKiModel = nineStarKiModel,
                            Energy = nineStarKiModel.PersonalHousesOccupiedEnergies.Month,
                            Lichun = lichun,
                            PeriodStarsOn = selectedMonthPeriod.PeriodStartsOn,
                            PeriodEndsOn = selectedMonthPeriod.PeriodEndsOn,
                            Energies = energies
                        };

                    // Year
                    default:
                        var yearlyPeriod = _swissEphemerisService.GetNineStarKiYearlyPeriod(selectedDateTime, userTimeZoneId);
                        var monthlyPeriodsForYear =
                            _swissEphemerisService.GetNineStarKiMonthlyPeriods(selectedDateTime, userTimeZoneId);

                        foreach (var monthlyPeriod in monthlyPeriodsForYear)
                        {
                            var energy = nineStarKiModel.GetPersonalCycleEnergy(monthlyPeriod.MonthlyKi, useHolograhpicCycleCalculation ? nineStarKiModel.PersonalChartEnergies.Month.EnergyNumber : nineStarKiModel.MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.MonthlyCycleEnergy);

                            var isSelected =
                                selectedDateTime.IsBetween(monthlyPeriod.PeriodStartsOn, monthlyPeriod.PeriodEndsOn);

                            energies.Add((energy, energy, monthlyPeriod.PeriodStartsOn, monthlyPeriod.PeriodEndsOn, isSelected));
                        }

                        return new PlannerViewModel
                        {
                            View = view,
                            Energy = nineStarKiModel.PersonalHousesOccupiedEnergies.Year,
                            Lichun = lichun,
                            PeriodStarsOn = yearlyPeriod.PeriodStartsOn,
                            PeriodEndsOn = yearlyPeriod.PeriodEndsOn,
                            Energies = energies
                        };
                }

            }, TimeSpan.FromDays(30));
        }

        private NineStarKiEnergy GetInvertedEnergy(NineStarKiEnergy energy)
        {
            return new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(energy.EnergyNumber));
        }

        private NineStarKiDirections GetYearlyDirections(NineStarKiModel model)
        {
            var houseOfFive = model.GetHouseOfFive(model.PersonalHousesOccupiedEnergies.Year.EnergyNumber);

            return new NineStarKiDirections(houseOfFive.Direction,
                GetInvertedEnergy(houseOfFive).Direction,
                model.PersonalHousesOccupiedEnergies.Year.Direction,
                GetInvertedEnergy(model.PersonalHousesOccupiedEnergies.Year).Direction);
        }

        private NineStarKiDirections GetMonthlyDirections(NineStarKiModel model)
        {
            var houseOfFive = model.GetHouseOfFive(model.PersonalHousesOccupiedEnergies.Month.EnergyNumber);

            return new NineStarKiDirections(houseOfFive.Direction,
                GetInvertedEnergy(houseOfFive).Direction,
                model.PersonalHousesOccupiedEnergies.Month.Direction,
                GetInvertedEnergy(model.PersonalHousesOccupiedEnergies.Month).Direction);
        }
    }
}