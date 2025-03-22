using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
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
            bool isMyProfile = false, DateTime? today = null)
        {
            var cacheKey = $"CalculateNineStarKiProfileFromModel_{personModel.DateOfBirth.ToString()}_{personModel.Name}_{personModel.Gender}_{isCompatibility}_{isMyProfile}_{today.ToString()}";
            return GetOrAddToCache(cacheKey, () =>
            {
                var tzInfo = TZConvert.GetTimeZoneInfo(personModel.TimeZoneId);
                var selectedDateTime = today == null
                    ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzInfo)
                    : today.Value;

                var preciseEpochEnergy = _swissEphemerisService.GetNineStarKiEightyOneYearKi(personModel.DateOfBirth, personModel.TimeZoneId);
                var preciseGenerationalEnergy = _swissEphemerisService.GetNineStarKiNineYearKi(personModel.DateOfBirth, personModel.TimeZoneId);
                var preciseMainEnergy = _swissEphemerisService.GetNineStarKiYearlyKi(personModel.DateOfBirth, personModel.TimeZoneId);
                var preciseEmotionalEnergy = _swissEphemerisService.GetNineStarKiMonthlyKi(personModel.DateOfBirth, personModel.TimeZoneId);
                var preciseDayStarEnergy = _swissEphemerisService.GetNineStarKiDailyKi(personModel.DateOfBirth, personModel.TimeZoneId);
                var preciseHourlyEnergy = _swissEphemerisService.GetNineStarKiHourlyKi(personModel.DateOfBirth, personModel.TimeZoneId);

                var preciseEightyOneYearEnergy = _swissEphemerisService.GetNineStarKiEightyOneYearKi(selectedDateTime, personModel.TimeZoneId);
                var preciseNineYearEnergy = _swissEphemerisService.GetNineStarKiNineYearKi(selectedDateTime, personModel.TimeZoneId);
                var preciseYearEnergy = _swissEphemerisService.GetNineStarKiYearlyKi(selectedDateTime, personModel.TimeZoneId);
                var preciseMonthEnergy = _swissEphemerisService.GetNineStarKiMonthlyKi(selectedDateTime, personModel.TimeZoneId);
                var preciseDailyEnergy = _swissEphemerisService.GetNineStarKiDailyKi(selectedDateTime, personModel.TimeZoneId);
                var preciseHourlyCycleEnergy = _swissEphemerisService.GetNineStarKiHourlyKi(selectedDateTime, personModel.TimeZoneId);

                var model = new NineStarKiModel(personModel, preciseEpochEnergy, preciseGenerationalEnergy, preciseMainEnergy, preciseEmotionalEnergy, preciseDayStarEnergy.DailyKi, preciseHourlyEnergy,
                    preciseEightyOneYearEnergy, preciseNineYearEnergy, preciseYearEnergy, preciseMonthEnergy, preciseDailyEnergy.DailyKi, preciseHourlyEnergy, preciseDailyEnergy.InvertedDailyKi, selectedDateTime);

                model.MainEnergy.EnergyDescription = GetMainEnergyDescription(model.MainEnergy.Energy);
                model.CharacterEnergy.EnergyDescription = GetCharacterEnergyDescription(model.CharacterEnergy.Energy);
                model.SurfaceEnergy.EnergyDescription = GetSurfaceEnergyDescription(model.SurfaceEnergy.Energy);
                model.Health = GetHealth(model.MainEnergy.Energy);
                model.Occupations = GetOccupations(model.MainEnergy.Energy);
                model.PersonalDevelopemnt = GetPersonalDevelopemnt(model.MainEnergy.Energy);
                model.Summary = GetSummary(model);
                model.Overview = GetOverview(model.MainEnergy.Energy);
                
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

                var mainEnergies = new List<NineStarKiModel>
            {
                CalculateNineStarKiProfile(new DateTime(1981, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1980, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1979, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1978, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1977, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1976, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1984, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1974, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1973, 3, 3)),
            };
                var characterEnergies = new List<NineStarKiModel>
            {
                CalculateNineStarKiProfile(new DateTime(1980, 2, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 3, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 4, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 5, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 6, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 7, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 8, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 9, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 10, 10)),
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
                var flexibleEnergies = new List<NineStarKiEnergy>
            {
                new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.MainEnergy),
                new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.MainEnergy),
                new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.MainEnergy)
            };

                return new NineStarKiSummaryViewModel(mainEnergies, characterEnergies, dynamicEnergies, staticEnergies,
                    flexibleEnergies);
            }, TimeSpan.FromDays(30));
        }

        private string GetOverview(ENineStarKiEnergy energy)
        {
            var cacheKey = $"GetOverview_{energy}";
            return GetOrAddToCache(cacheKey, () =>
            {
                switch (energy)
                {
                    case ENineStarKiEnergy.Water:
                        return Dictionary.water_overview;

                    case ENineStarKiEnergy.Soil:
                        return Dictionary.soil_overview;

                    case ENineStarKiEnergy.Thunder:
                        return Dictionary.thunder_overview;

                    case ENineStarKiEnergy.Wind:
                        return Dictionary.wind_overview;

                    case ENineStarKiEnergy.CoreEarth:
                        return Dictionary.coreearth_overview;

                    case ENineStarKiEnergy.Heaven:
                        return Dictionary.heaven_overview;

                    case ENineStarKiEnergy.Lake:
                        return Dictionary.lake_overview;

                    case ENineStarKiEnergy.Mountain:
                        return Dictionary.mountain_overview;

                    case ENineStarKiEnergy.Fire:
                        return Dictionary.fire_overview;
                }
                return string.Empty;

            }, TimeSpan.FromDays(30));
        }

        private string GetSummary(NineStarKiModel model)
        {
            var cacheKey = $"GetSummary_{model.PersonModel.IsAdult()}_{model.MainEnergy.Energy}_{model.CharacterEnergy.Energy}";
            return GetOrAddToCache(cacheKey, () =>
            {
                if (!model.PersonModel.IsAdult())
                {
                    switch (model.CharacterEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Dictionary.water_child;

                        case ENineStarKiEnergy.Soil:
                            return Dictionary.soil_child;

                        case ENineStarKiEnergy.Thunder:
                            return Dictionary.thunder_child;

                        case ENineStarKiEnergy.Wind:
                            return Dictionary.wind_child;

                        case ENineStarKiEnergy.CoreEarth:
                            return Dictionary.coreearth_child;

                        case ENineStarKiEnergy.Heaven:
                            return Dictionary.heaven_child;

                        case ENineStarKiEnergy.Lake:
                            return Dictionary.lake_child;

                        case ENineStarKiEnergy.Mountain:
                            return Dictionary.mountain_child;

                        case ENineStarKiEnergy.Fire:
                            return Dictionary.fire_child;
                    }
                }
                else
                {
                    switch (model.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._115;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._124;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._133;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._142;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._151;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._169;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._178;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._187;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._196;
                            }

                            break;

                        case ENineStarKiEnergy.Soil:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._216;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._225;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._234;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._243;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._252;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._261;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._279;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._288;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._297;
                            }

                            break;

                        case ENineStarKiEnergy.Thunder:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._317;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._326;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._335;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._344;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._353;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._362;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._371;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._389;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._398;
                            }

                            break;

                        case ENineStarKiEnergy.Wind:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._418;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._427;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._436;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._445;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._454;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._463;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._472;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._481;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._499;
                            }

                            return string.Empty;

                        case ENineStarKiEnergy.CoreEarth:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._519;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._528;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._537;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._546;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._555;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._564;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._573;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._582;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._591;
                            }

                            return string.Empty;

                        case ENineStarKiEnergy.Heaven:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._611;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._629;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._638;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._647;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._656;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._665;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._674;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._683;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._692;
                            }

                            return string.Empty;

                        case ENineStarKiEnergy.Lake:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._712;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._721;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._739;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._748;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._757;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._766;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._775;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._784;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._793;
                            }

                            return string.Empty;

                        case ENineStarKiEnergy.Mountain:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._813;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._822;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._831;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._849;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._858;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._867;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._876;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._885;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._894;
                            }

                            return string.Empty;

                        case ENineStarKiEnergy.Fire:
                            switch (model.CharacterEnergy.Energy)
                            {
                                case ENineStarKiEnergy.Water:
                                    return Dictionary._914;

                                case ENineStarKiEnergy.Soil:
                                    return Dictionary._923;

                                case ENineStarKiEnergy.Thunder:
                                    return Dictionary._932;

                                case ENineStarKiEnergy.Wind:
                                    return Dictionary._941;

                                case ENineStarKiEnergy.CoreEarth:
                                    return Dictionary._959;

                                case ENineStarKiEnergy.Heaven:
                                    return Dictionary._968;

                                case ENineStarKiEnergy.Lake:
                                    return Dictionary._977;

                                case ENineStarKiEnergy.Mountain:
                                    return Dictionary._986;

                                case ENineStarKiEnergy.Fire:
                                    return Dictionary._995;
                            }

                            return string.Empty;
                    }
                }

                return string.Empty;
            }, TimeSpan.FromDays(30));
        }

        private string GetMainEnergyDescription(ENineStarKiEnergy energy)
        {
            var cacheKey = $"GetMainEnergyDescription_{energy}";
            return GetOrAddToCache(cacheKey, () =>
            {
                switch (energy)
                {
                    case ENineStarKiEnergy.Water:
                        return Dictionary.water_description;

                    case ENineStarKiEnergy.Soil:
                        return Dictionary.soil_description;

                    case ENineStarKiEnergy.Thunder:
                        return Dictionary.thunder_description;

                    case ENineStarKiEnergy.Wind:
                        return Dictionary.wind_description;

                    case ENineStarKiEnergy.CoreEarth:
                        return Dictionary.coreearth_description;

                    case ENineStarKiEnergy.Heaven:
                        return Dictionary.heaven_description;

                    case ENineStarKiEnergy.Lake:
                        return Dictionary.lake_description;

                    case ENineStarKiEnergy.Mountain:
                        return Dictionary.mountain_description;

                    case ENineStarKiEnergy.Fire:
                        return Dictionary.fire_description;
                }

                return string.Empty;

            }, TimeSpan.FromDays(30));
        }

        private string GetCharacterEnergyDescription(ENineStarKiEnergy energy)
        {
            var cacheKey = $"GetCharacterEnergyDescription_{energy}";
            return GetOrAddToCache(cacheKey, () =>
            {
                switch (energy)
                {
                    case ENineStarKiEnergy.Water:
                        return Dictionary.water_emotional_description;

                    case ENineStarKiEnergy.Soil:
                        return Dictionary.soil_emotional_description;

                    case ENineStarKiEnergy.Thunder:
                        return Dictionary.thunder_emotional_description;

                    case ENineStarKiEnergy.Wind:
                        return Dictionary.wind_emotional_description;

                    case ENineStarKiEnergy.CoreEarth:
                        return Dictionary.coreearth_emotional_description;

                    case ENineStarKiEnergy.Heaven:
                        return Dictionary.heaven_emotional_description;

                    case ENineStarKiEnergy.Lake:
                        return Dictionary.lake_emotional_description;

                    case ENineStarKiEnergy.Mountain:
                        return Dictionary.mountain_emotional_description;

                    case ENineStarKiEnergy.Fire:
                        return Dictionary.fire_emotional_description;
                }

                return string.Empty;

            }, TimeSpan.FromDays(30));
        }

        private string GetSurfaceEnergyDescription(ENineStarKiEnergy energy)
        {
            var cacheKey = $"GetSurfaceEnergyDescription_{energy}";
            return GetOrAddToCache(cacheKey, () =>
            {
                switch (energy)
                {
                    case ENineStarKiEnergy.Water:
                        return Dictionary.water_surface_description;

                    case ENineStarKiEnergy.Soil:
                        return Dictionary.soil_surface_description;

                    case ENineStarKiEnergy.Thunder:
                        return Dictionary.thunder_surface_description;

                    case ENineStarKiEnergy.Wind:
                        return Dictionary.wind_surface_description;

                    case ENineStarKiEnergy.CoreEarth:
                        return Dictionary.coreearth_surface_description;

                    case ENineStarKiEnergy.Heaven:
                        return Dictionary.heaven_surface_description;

                    case ENineStarKiEnergy.Lake:
                        return Dictionary.lake_surface_description;

                    case ENineStarKiEnergy.Mountain:
                        return Dictionary.mountain_surface_description;

                    case ENineStarKiEnergy.Fire:
                        return Dictionary.fire_surface_description;
                }

                return string.Empty;

            }, TimeSpan.FromDays(30));
        }

        private string GetHealth(ENineStarKiEnergy energy)
        {
            var cacheKey = $"GetHealth_{energy}";
            return GetOrAddToCache(cacheKey, () =>
            {

                switch (energy)
                {
                    case ENineStarKiEnergy.Water:
                        return Dictionary.water_health;

                    case ENineStarKiEnergy.Soil:
                        return Dictionary.soil_health;

                    case ENineStarKiEnergy.Thunder:
                        return Dictionary.thunder_health;

                    case ENineStarKiEnergy.Wind:
                        return Dictionary.wind_health;

                    case ENineStarKiEnergy.CoreEarth:
                        return Dictionary.coreearth_health;

                    case ENineStarKiEnergy.Heaven:
                        return Dictionary.heaven_health;

                    case ENineStarKiEnergy.Lake:
                        return Dictionary.lake_health;

                    case ENineStarKiEnergy.Mountain:
                        return Dictionary.mountain_health;

                    case ENineStarKiEnergy.Fire:
                        return Dictionary.fire_health;
                }

                return string.Empty;

            }, TimeSpan.FromDays(30));
        }

        private string GetOccupations(ENineStarKiEnergy energy)
        {
            var cacheKey = $"GetOccupations_{energy}";
            return GetOrAddToCache(cacheKey, () =>
            {

                switch (energy)
                {
                    case ENineStarKiEnergy.Water:
                        return Dictionary.water_occupations;

                    case ENineStarKiEnergy.Soil:
                        return Dictionary.soil_occupations;

                    case ENineStarKiEnergy.Thunder:
                        return Dictionary.thunder_occupations;

                    case ENineStarKiEnergy.Wind:
                        return Dictionary.wind_occupations;

                    case ENineStarKiEnergy.CoreEarth:
                        return Dictionary.coreearth_occupations;

                    case ENineStarKiEnergy.Heaven:
                        return Dictionary.heaven_occupations;

                    case ENineStarKiEnergy.Lake:
                        return Dictionary.lake_occupations;

                    case ENineStarKiEnergy.Mountain:
                        return Dictionary.mountain_occupations;

                    case ENineStarKiEnergy.Fire:
                        return Dictionary.fire_occupations;
                }

                return string.Empty;

            }, TimeSpan.FromDays(30));
        }

        private string GetPersonalDevelopemnt(ENineStarKiEnergy energy)
        {
            var cacheKey = $"GetPersonalDevelopemnt_{energy}";
            return GetOrAddToCache(cacheKey, () =>
            {

                switch (energy)
                {
                    case ENineStarKiEnergy.Water:
                        return Dictionary.water_personal_development;

                    case ENineStarKiEnergy.Soil:
                        return Dictionary.soil_personal_development;

                    case ENineStarKiEnergy.Thunder:
                        return Dictionary.thunder_personal_development;

                    case ENineStarKiEnergy.Wind:
                        return Dictionary.wind_personal_development;

                    case ENineStarKiEnergy.CoreEarth:
                        return Dictionary.coreearth_personal_development;

                    case ENineStarKiEnergy.Heaven:
                        return Dictionary.heaven_personal_development;

                    case ENineStarKiEnergy.Lake:
                        return Dictionary.lake_personal_development;

                    case ENineStarKiEnergy.Mountain:
                        return Dictionary.mountain_personal_development;

                    case ENineStarKiEnergy.Fire:
                        return Dictionary.fire_personal_development;
                }

                return string.Empty;

            }, TimeSpan.FromDays(30));
        }

        private NineStarKiEnergy GetInvertedEnergy(NineStarKiEnergy energy, ENineStarKiEnergyType type)
        {
            return new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(energy.EnergyNumber), type);
        }

        private NineStarKiDirections GetYearlyDirections(NineStarKiModel model)
        {
            return new NineStarKiDirections(model.GlobalCycleEnergies.Year.Direction,
                GetInvertedEnergy(model.GlobalCycleEnergies.Year, ENineStarKiEnergyType.MainEnergy).Direction,
                model.PersonalHousesOccupiedEnergies.Year.Direction,
                GetInvertedEnergy(model.PersonalHousesOccupiedEnergies.Year, ENineStarKiEnergyType.MainEnergy).Direction);
        }

        private NineStarKiDirections GetMonthlyDirections(NineStarKiModel model)
        {
            return new NineStarKiDirections(model.GlobalCycleEnergies.Month.Direction,
                GetInvertedEnergy(model.GlobalCycleEnergies.Month, ENineStarKiEnergyType.MainEnergy).Direction,
                model.PersonalHousesOccupiedEnergies.Month.Direction,
                GetInvertedEnergy(model.PersonalHousesOccupiedEnergies.Month, ENineStarKiEnergyType.MainEnergy).Direction);
        }
    }
}