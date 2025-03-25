using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using K9.WebApplication.Enums;
using TimeZoneConverter;

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
            bool isMyProfile = false, DateTime? today = null, ECalculationMethod calculationMethod = ECalculationMethod.Chinese, bool includeCycles = true, bool useHolograhpicCycleCalculation = false, bool invertDailyAndHourlyKiForSouthernHemisphere = false, bool invertDailyAndHourlyCycleKiForSouthernHemisphere = false)
        {
            var cacheKey = $"CalculateNineStarKiProfileFromModel_{personModel.DateOfBirth:yyyyMMddHHmm}_{personModel.TimeOfBirth.ToString()}_{personModel.BirthTimeZoneId}_{personModel.Name}_{personModel.Gender}_{isCompatibility}_{isMyProfile}_{calculationMethod}_{includeCycles}_{useHolograhpicCycleCalculation}_{today:yyyyMMddHHmm}_{invertDailyAndHourlyKiForSouthernHemisphere}";
            return GetOrAddToCache(cacheKey, () =>
            {
                var tzInfo = TZConvert.GetTimeZoneInfo(personModel.BirthTimeZoneId);
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
                    var preciseEightyOneYearEnergy =
                        _swissEphemerisService.GetNineStarKiEightyOneYearKi(selectedDateTime, personModel.BirthTimeZoneId);
                    var preciseNineYearEnergy =
                        _swissEphemerisService.GetNineStarKiNineYearKi(selectedDateTime, personModel.BirthTimeZoneId);
                    var preciseYearEnergy =
                        _swissEphemerisService.GetNineStarKiYearlyKi(selectedDateTime, personModel.BirthTimeZoneId);
                    var preciseMonthEnergy =
                        _swissEphemerisService.GetNineStarKiMonthlyKi(selectedDateTime, personModel.BirthTimeZoneId);
                    var preciseDailyEnergy =
                        _swissEphemerisService.GetNineStarKiDailyKi(selectedDateTime, personModel.BirthTimeZoneId);
                    var preciseHourlyCycleEnergy =
                        _swissEphemerisService.GetNineStarKiHourlyKi(selectedDateTime, personModel.BirthTimeZoneId);

                    var yearlyPeriods =
                        _swissEphemerisService.GetNineStarKiYearlyPeriods(selectedDateTime, personModel.BirthTimeZoneId);

                    var monthlyPeriods = new List<(DateTime PeriodStartOn, DateTime PeriodEndsOn, int MonthlyKi)>();
                    foreach (var yearlyPeriod in yearlyPeriods)
                    {
                        monthlyPeriods.AddRange(_swissEphemerisService.GetNineStarKiMonthlyPeriods(yearlyPeriod.YearStart.AddDays(1), personModel.BirthTimeZoneId));
                    }

                    var dailyPeriods =
                        _swissEphemerisService.GetNineStarKiDailyEnergiesForMonth(selectedDateTime,
                            personModel.BirthTimeZoneId);

                    model = new NineStarKiModel(personModel, preciseEpochEnergy, preciseGenerationalEnergy,
                        preciseMainEnergy, preciseEmotionalEnergy, preciseEmotionalEnergyForInvertedYear,
                        preciseDayStarEnergy.DailyKi, preciseHourlyEnergy,
                        preciseEightyOneYearEnergy, preciseNineYearEnergy, preciseYearEnergy, preciseMonthEnergy,
                        preciseDailyEnergy.DailyKi, preciseHourlyCycleEnergy, preciseDailyEnergy.InvertedDailyKi,
                        selectedDateTime, calculationMethod, useHolograhpicCycleCalculation, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere)
                    {
                        YearlyPeriods = yearlyPeriods,
                        MonthlyPeriods = monthlyPeriods.ToArray(),
                        DailyPeriods = dailyPeriods
                    };
                }
                else
                {
                    model = new NineStarKiModel(personModel, preciseEpochEnergy, preciseGenerationalEnergy,
                        preciseMainEnergy, preciseEmotionalEnergy, preciseEmotionalEnergyForInvertedYear,
                        preciseDayStarEnergy.DailyKi, preciseHourlyEnergy,
                        5, 5, 5, 5, 5, 5, 5, selectedDateTime, calculationMethod, useHolograhpicCycleCalculation, invertDailyAndHourlyKiForSouthernHemisphere, invertDailyAndHourlyCycleKiForSouthernHemisphere)
                    {
                        YearlyPeriods = new (DateTime PeriodStartOn, DateTime PeriodEndsOn, int YearlyKi)[] { },
                        MonthlyPeriods = new (DateTime PeriodStartOn, DateTime PeriodEndsOn, int MonthlyKi)[] { },
                        DailyPeriods = new (DateTime Date, int DailyKi, int? InvertedDailyKi)[] { }
                    };
                }

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
            bool isHideSexuality)
        {
            var cacheKey = $"CalculateCompatibilityFromModel_{personModel1.DateOfBirth.ToString()}_{personModel1.Name}_{personModel1.Gender}_{personModel2.DateOfBirth.ToString()}_{personModel2.Name}_{personModel2.Gender}";
            return GetOrAddToCache(cacheKey, () =>
            {
                var nineStarKiModel1 = CalculateNineStarKiProfile(personModel1, true);
                var nineStarKiModel2 = CalculateNineStarKiProfile(personModel2, true);

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

                return new NineStarKiSummaryViewModel(yearEnergies, monthEnergies, dynamicEnergies, staticEnergies,
                    reflectiveEnergies);
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