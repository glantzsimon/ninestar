using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using K9.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace K9.WebApplication.Models
{

    public class NineStarKiModel : CachableBase
    {
        #region Options and Flags

        private static DateTime CYCLE_SWITCH_DATE = new DateTime(2105, 2, 4);

        [ScriptIgnore]
        [UIHint("CalculationMethod")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CalculationMethodLabel)]
        public ECalculationMethod CalculationMethod { get; set; } = (ECalculationMethod)SessionHelper.GetCurrentUserCalculationMethod();

        [ScriptIgnore]
        public bool InvertPersonalYinEnergies { get; set; } = true;

        [ScriptIgnore]
        public bool InvertCycleYinEnergies { get; set; } = true;

        [ScriptIgnore]
        public bool EnableCycleSwitch { get; set; } = true;

        [ScriptIgnore]
        public bool IsCycleSwitchActive => EnableCycleSwitch && SelectedDate >= CYCLE_SWITCH_DATE;

        #endregion

        public NineStarKiModel()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = Methods.GetRandomGender(),
                TimeZoneId = Current.UserTimeZoneId
            };

            PersonModel = personModel;
        }

        public NineStarKiModel(PersonModel personModel)
        {
            PersonModel = personModel;
        }

        public NineStarKiModel(PersonModel personModel, int precisePersonEpochEnergy, int precisePersonGenerationalEnergy, int preciseMainEnergy, int preciseEmotionalEnergy, int preciseEmotionalEnergyForInvertedYear, int precisePersonalDayStarEnergy, int precisePersonalHourlyEnergy,

            int preciseEpochCycleEnergy, int preciseGenerationalCycleEnergy, int preciseYearlyCycleEnergy, int preciseMonthlyCycleEnergy, int preciseDailyCycleEnergy, int preciseHourlyCycleEnergy, int? preciseDailyCycleInvertedEnergy, DateTime? selectedDate = null)
        {
            SelectedDate = selectedDate ?? DateTime.UtcNow;

            PersonModel = personModel;
            PersonalChartEnergies = new NineStarKiEnergiesModel();
            GlobalCycleEnergies = new NineStarKiEnergiesModel();
            PersonalHousesOccupiedEnergies = new NineStarKiEnergiesModel();

            preciseEmotionalEnergy = PersonModel.Gender.IsYin() && InvertPersonalYinEnergies
                ? preciseEmotionalEnergyForInvertedYear
                : preciseEmotionalEnergy;

            var personalInfoString =
                $"{PersonModel.DateOfBirth}_{PersonModel.TimeOfBirth}_{PersonModel.TimeZoneId}_{PersonModel.Gender}";

            #region Personal Chart

            PersonalChartEnergies.Epoch = GetOrAddToCache($"Epoch_p_{personalInfoString}_{precisePersonEpochEnergy}",
                () => GetPersonalEnergy(precisePersonEpochEnergy, ENineStarKiEnergyType.EpochEnergy), TimeSpan.FromDays(30));

            PersonalChartEnergies.Generation = GetOrAddToCache($"Generation_p_{personalInfoString}_{precisePersonGenerationalEnergy}",
                () => GetPersonalEnergy(precisePersonGenerationalEnergy, ENineStarKiEnergyType.GenerationalEnergy), TimeSpan.FromDays(30));

            PersonalChartEnergies.Year = GetOrAddToCache($"Year_p_{personalInfoString}_{preciseMainEnergy}",
                () => GetPersonalEnergy(preciseMainEnergy, ENineStarKiEnergyType.MainEnergy), TimeSpan.FromDays(30));

            PersonalChartEnergies.Month = GetOrAddToCache($"Month_p_{personalInfoString}_{preciseEmotionalEnergy}",
                () => GetPersonalEnergy(preciseEmotionalEnergy, ENineStarKiEnergyType.CharacterEnergy), TimeSpan.FromDays(30));

            PersonalChartEnergies.Surface = GetOrAddToCache($"Surface_p_{personalInfoString}",
                GetSurfaceEnergy, TimeSpan.FromDays(30));

            PersonalChartEnergies.Day = GetOrAddToCache($"Day_p_{personalInfoString}_{precisePersonalDayStarEnergy}",
                () => GetPersonalEnergy(precisePersonalDayStarEnergy, ENineStarKiEnergyType.DailyEnergy), TimeSpan.FromDays(30));

            PersonalChartEnergies.Hour = GetOrAddToCache($"Hour_p_{personalInfoString}_{precisePersonalHourlyEnergy}",
                () => GetPersonalEnergy(precisePersonalDayStarEnergy, ENineStarKiEnergyType.DailyEnergy), TimeSpan.FromDays(30));

            #endregion region


            #region Global Cycles

            GlobalCycleEnergies.Epoch = GetOrAddToCache($"Epoch_g_{SelectedDate}_{preciseEpochCycleEnergy}",
                () => GetGlobalCycleEnergy(preciseEpochCycleEnergy, ENineStarKiEnergyCycleType.EpochEnergy), TimeSpan.FromDays(30));

            GlobalCycleEnergies.Generation = GetOrAddToCache($"Generation_g_{SelectedDate}_{preciseGenerationalCycleEnergy}",
                () => GetGlobalCycleEnergy(preciseGenerationalCycleEnergy, ENineStarKiEnergyCycleType.GenerationalEnergy), TimeSpan.FromDays(30));

            GlobalCycleEnergies.Year = GetOrAddToCache($"Year_g_{SelectedDate}_{preciseYearlyCycleEnergy}",
                () => GetGlobalCycleEnergy(preciseYearlyCycleEnergy, ENineStarKiEnergyCycleType.YearlyCycleEnergy), TimeSpan.FromDays(30));

            GlobalCycleEnergies.Month = GetOrAddToCache($"Month_g_{SelectedDate}_{preciseMonthlyCycleEnergy}",
                () => GetGlobalCycleEnergy(preciseMonthlyCycleEnergy, ENineStarKiEnergyCycleType.MonthlyCycleEnergy), TimeSpan.FromDays(30));

            GlobalCycleEnergies.Day = GetOrAddToCache($"Day_g_{SelectedDate}_{preciseDailyCycleEnergy}_{preciseDailyCycleInvertedEnergy}",
                () => GetGlobalCycleEnergy(preciseDailyCycleEnergy, ENineStarKiEnergyCycleType.DailyEnergy), TimeSpan.FromDays(30));

            GlobalCycleEnergies.Hour = GetOrAddToCache($"Hour_g_{SelectedDate}_{preciseHourlyCycleEnergy}",
                () => GetGlobalCycleEnergy(preciseHourlyCycleEnergy, ENineStarKiEnergyCycleType.HourlyEnergy), TimeSpan.FromDays(30));

            #endregion



            #region Personal Cycle Energies / Houses Occupied

            PersonalHousesOccupiedEnergies.Epoch = GetOrAddToCache($"Epoch_h_{SelectedDate}_{personalInfoString}_{preciseEpochCycleEnergy}",
                () => GetPersonalCycleEnergy(preciseEpochCycleEnergy, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.EpochEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Generation = GetOrAddToCache($"Generation_h_{SelectedDate}_{preciseGenerationalCycleEnergy}",
                () => GetPersonalCycleEnergy(preciseGenerationalCycleEnergy, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.GenerationalEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Year = GetOrAddToCache($"Year_h_{SelectedDate}_{personalInfoString}_{preciseYearlyCycleEnergy}",
                () => GetPersonalCycleEnergy(preciseYearlyCycleEnergy, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.YearlyCycleEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Month = GetOrAddToCache($"Month_h_{SelectedDate}_{personalInfoString}_{preciseMonthlyCycleEnergy}",
                () => GetPersonalCycleEnergy(preciseMonthlyCycleEnergy, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.MonthlyCycleEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Day = GetOrAddToCache($"Day_h_{SelectedDate}_{personalInfoString}_{preciseDailyCycleEnergy}_{preciseDailyCycleInvertedEnergy}",
                () => GetPersonalCycleEnergy(preciseDailyCycleEnergy, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Hour = GetOrAddToCache($"Hour_h_{SelectedDate}_{personalInfoString}_{preciseHourlyCycleEnergy}",
                () => GetPersonalCycleEnergy(preciseHourlyCycleEnergy, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.HourlyEnergy), TimeSpan.FromDays(30));

            if (preciseDailyCycleInvertedEnergy.HasValue)
            {
                PersonalHousesOccupiedEnergies.DayInverted = GetOrAddToCache($"DayInverted_h_{SelectedDate}_{personalInfoString}_{preciseDailyCycleEnergy}_{preciseDailyCycleInvertedEnergy}",
                    () => GetPersonalCycleEnergy(preciseDailyCycleInvertedEnergy.Value, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy), TimeSpan.FromDays(30));
            }

            #endregion

            MainEnergy.RelatedEnergy = CharacterEnergy.Energy;
            CharacterEnergy.RelatedEnergy = MainEnergy.Energy;
            SurfaceEnergy.RelatedEnergy = MainEnergy.Energy;
        }

        public PersonModel PersonModel { get; }

        public NineStarKiEnergiesModel PersonalChartEnergies { get; }

        public NineStarKiEnergiesModel GlobalCycleEnergies { get; }

        public NineStarKiEnergiesModel PersonalHousesOccupiedEnergies { get; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MainEnergyLabel)]
        public NineStarKiEnergy MainEnergy => PersonalChartEnergies?.Year;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergy CharacterEnergy => PersonalChartEnergies?.Month;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergy SurfaceEnergy => PersonalChartEnergies?.Surface;

        public NineStarKiEnergy GetHouseOfFive(int energy) => new NineStarKiEnergy((ENineStarKiEnergy)GetNineStarKiNumber(energy + (5 - MainEnergy?.EnergyNumber ?? 0)));

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SummaryLabel)]
        public string Summary { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.OverviewLabel)]
        public string Overview { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.HealthLabel)]
        public string Health { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.OccupationsLabel)]
        public string Occupations { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PersonalDevelopemntLabel)]
        public string PersonalDevelopemnt { get; set; }

        [ScriptIgnore]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SelectedDateLabel)]
        public DateTime? SelectedDate { get; set; }

        [ScriptIgnore]
        public bool IsScrollToCyclesOverview { get; set; }

        [ScriptIgnore]
        public string ActiveCycleTabId { get; set; }

        [ScriptIgnore]
        public bool IsShowSummary { get; set; } = true;

        [ScriptIgnore]
        public bool IsCompatibility { get; set; } = false;

        public string DailyCycleEnergyDisplayText =>
            PersonalHousesOccupiedEnergies.DayInverted == null ? PersonalHousesOccupiedEnergies.Day.EnergyNumber.ToString() : $"{PersonalHousesOccupiedEnergies.Day.EnergyNumber.ToString()}/{PersonalHousesOccupiedEnergies.DayInverted.EnergyNumber.ToString()}";

        public NineStarKiDirections YearlyDirections { get; set; }

        public NineStarKiDirections MonthlyDirections { get; set; }

        [ScriptIgnore]
        public ESexualityRelationType SexualityRelationType => GetSexualityRelationType();

        [ScriptIgnore]
        public string OverviewLabel => MainEnergy != null ? $"{MainEnergy.EnergyNameAndNumber} Overview" : "";

        [ScriptIgnore]
        public string EnergySexualityLabel => MainEnergy != null ? $"{MainEnergy.EnergyNameAndNumber} {Dictionary.SexualityLabel}" : "";

        [ScriptIgnore]
        public string SexualityRelationTypeDetailsStraight => GetSexualityGenderDescription();

        [ScriptIgnore]
        public string SexualityRelationTypeDetailsGay => GetSexualityGenderDescription(true);

        [ScriptIgnore]
        public string MainEnergySexualityDetails => GetMainEnergySexualityDetails();

        [ScriptIgnore]
        public string GayLabel => PersonModel?.Gender == EGender.Female ? Dictionary.Lesbian : Dictionary.Gay;

        [ScriptIgnore]
        public bool IsProcessed { get; set; } = false;

        [ScriptIgnore]
        public bool IsMyProfile { get; set; } = false;

        [UIHint("Organ")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.StrongYinOrgans)]
        public EOrgan? StrongYinOrgans => MainEnergy != null ? MainEnergy.MetaData?.StrongYinOrgans : null;

        [UIHint("Organ")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.StrongYangOrgans)]
        public EOrgan? StrongYangOrgans => MainEnergy != null ? MainEnergy.MetaData?.StrongYangOrgans : null;

        [UIHint("Organs")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.WeakYinOrgans)]
        public EOrgan[] WeakYinOrgans => MainEnergy != null ? MainEnergy.MetaData?.WeakYinOrgans : null;

        [UIHint("Organs")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.WeakYangOrgans)]
        public EOrgan[] WeakYangOrgans => MainEnergy != null ? MainEnergy.MetaData?.WeakYangOrgans : null;

        /// <summary>
        /// To do: move to SwissEphem / NineStarService
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, NineStarKiEnergy>> GetYearlyPlanner()
        {
            return GetOrAddToCache($"YearlyPlanner_{DateTime.Today.Year}", () =>
            {
                var cycles = new List<Tuple<int, NineStarKiEnergy>>();
                var today = new DateTime(DateTime.Today.Year, 2, 15);

                for (int i = -20; i <= 20; i++)
                {
                    SelectedDate = today.AddYears(i);
                    cycles.Add(new Tuple<int, NineStarKiEnergy>(SelectedDate.Value.Year, new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyCycleType.DailyEnergy)));
                }

                SelectedDate = null;

                return cycles;
            }, TimeSpan.FromDays(30));
        }

        /// <summary>
        /// TODO: Move this to SwissEphem
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, int, string, NineStarKiEnergy>> GetMonthlyPlanner()
        {
            return GetOrAddToCache($"MonthlyPlanner_{DateTime.Today.Year}", () =>
            {
                var cycles = new List<Tuple<int, int, string, NineStarKiEnergy>>();
                var today = new DateTime(DateTime.Today.Year, 2, 15);

                for (int i = -1; i <= 10; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        var year = today.AddYears(i).Year;
                        SelectedDate = new DateTime(year, j + 1, 15);
                        cycles.Add(new Tuple<int, int, string, NineStarKiEnergy>(SelectedDate.Value.Year, SelectedDate.Value.Month, SelectedDate.Value.ToString("MMMM"), GetPersonalCycleEnergy(2, 1, ENineStarKiEnergyCycleType.MonthlyCycleEnergy)));
                    }
                }

                SelectedDate = null;

                return cycles;

            }, TimeSpan.FromDays(30));
        }

        /// <summary>
        /// Ensures number is always in the range 1-9
        /// </summary>
        /// <param name="energyNumber"></param>
        /// <returns></returns>
        public static int GetNineStarKiNumber(int energyNumber) => (energyNumber + 8) % 9 + 1;

        private NineStarKiEnergy GetPersonalEnergy(int energyNumber, ENineStarKiEnergyType energyType)
        {
            energyNumber = GetNineStarKiNumber(energyNumber);
            if (PersonModel.Gender.IsYin() && InvertPersonalYinEnergies)
            {
                energyNumber = InvertEnergy(energyNumber);
            }

            return new NineStarKiEnergy((ENineStarKiEnergy)energyNumber, energyType, PersonModel.IsAdult());
        }

        private NineStarKiEnergy GetSurfaceEnergy()
        {
            var surfaceEnergyNumber = GetNineStarKiNumber(5 - (CharacterEnergy.EnergyNumber - MainEnergy.EnergyNumber));
            return GetPersonalEnergy(surfaceEnergyNumber, ENineStarKiEnergyType.SurfaceEnergy);
        }

        private NineStarKiEnergy GetGlobalCycleEnergy(int cycleEnergy, ENineStarKiEnergyCycleType cycleType)
        {
            var selectedDate = SelectedDate ?? DateTime.Today;
            cycleEnergy = IsCycleSwitchActive ? GetOppositeEnergyInMagicSquare(cycleEnergy) : cycleEnergy;

            var energy = (ENineStarKiEnergy)cycleEnergy;

            return new NineStarKiEnergy(energy, cycleType);
        }

        private NineStarKiEnergy GetPersonalCycleEnergy(int cycleEnergy, int energyNumber, ENineStarKiEnergyCycleType cycleType)
        {
            var selectedDate = SelectedDate ?? DateTime.Today;
            var invertEnergy = (PersonModel.Gender.IsYin() && InvertCycleYinEnergies) || IsCycleSwitchActive;
            energyNumber = invertEnergy ? InvertEnergy(energyNumber) : energyNumber;
            var houseOccupied = GetHouseOccupiedByNumber(cycleEnergy, energyNumber);
            houseOccupied = invertEnergy ? GetOppositeEnergyInMagicSquare(houseOccupied) : houseOccupied;

            var energy = (ENineStarKiEnergy)(houseOccupied);

            return new NineStarKiEnergy(energy, cycleType);
        }

        public static int GetHouseOccupiedByNumber(int cycleEnergy, int personalPrimaryEnergy)
        {
            var offset = (5 - cycleEnergy);
            return GetNineStarKiNumber(personalPrimaryEnergy + offset);
        }

        private static readonly Dictionary<int, int> _invertedEnergies = new Dictionary<int, int>
        {
            { 1, 5 }, { 2, 4 }, { 4, 2 }, { 5, 1 },
            { 6, 9 }, { 7, 8 }, { 8, 7 }, { 9, 6 }
        };

        public static int InvertEnergy(int energyNumber) =>
            _invertedEnergies.TryGetValue(energyNumber, out var inverted) ? inverted : energyNumber;

        private static readonly Dictionary<int, int> _invertedDirectionEnergies = new Dictionary<int, int>
        {
            { 1, 9 }, { 2, 8 }, { 3, 7 }, { 4, 6 },
            { 5, 5 }, { 6, 4 }, { 7, 3 }, { 8, 2 }, { 9, 1 }
        };

        public static int GetOppositeEnergyInMagicSquare(int energyNumber) =>
            _invertedDirectionEnergies.TryGetValue(energyNumber, out var inverted) ? inverted : energyNumber;

        private static readonly Dictionary<bool, Dictionary<(ENineStarKiYinYang, ENineStarKiYinYang), ESexualityRelationType>> _sexualityRelations
            = new Dictionary<bool, Dictionary<(ENineStarKiYinYang, ENineStarKiYinYang), ESexualityRelationType>>
            {
                // If PersonModel.Gender.IsYin() == true
                [true] = new Dictionary<(ENineStarKiYinYang, ENineStarKiYinYang), ESexualityRelationType>
                {
                    { (ENineStarKiYinYang.Yin, ENineStarKiYinYang.Yin), ESexualityRelationType.MatchMatch },
                    { (ENineStarKiYinYang.Yang, ENineStarKiYinYang.Yang), ESexualityRelationType.OppositeOpposite },
                    { (ENineStarKiYinYang.Yin, ENineStarKiYinYang.Yang), ESexualityRelationType.MatchOpposite },
                    { (ENineStarKiYinYang.Yang, ENineStarKiYinYang.Yin), ESexualityRelationType.OppositeMatch }
                },

                // If PersonModel.Gender.IsYin() == false
                [false] = new Dictionary<(ENineStarKiYinYang, ENineStarKiYinYang), ESexualityRelationType>
                {
                    { (ENineStarKiYinYang.Yin, ENineStarKiYinYang.Yin), ESexualityRelationType.OppositeOpposite },
                    { (ENineStarKiYinYang.Yang, ENineStarKiYinYang.Yang), ESexualityRelationType.MatchMatch },
                    { (ENineStarKiYinYang.Yin, ENineStarKiYinYang.Yang), ESexualityRelationType.OppositeMatch },
                    { (ENineStarKiYinYang.Yang, ENineStarKiYinYang.Yin), ESexualityRelationType.MatchOpposite }
                }
            };

        private ESexualityRelationType GetSexualityRelationType()
        {
            if (PersonModel == null || MainEnergy == null || CharacterEnergy == null)
            {
                return ESexualityRelationType.Unspecified;
            }

            bool isPersonYin = PersonModel.Gender.IsYin();

            return _sexualityRelations[isPersonYin].TryGetValue((MainEnergy.YinYang, CharacterEnergy.YinYang), out var relation)
                ? relation
                : ESexualityRelationType.Unspecified;
        }


        private string GetSexualityGenderDescription(bool isGay = false)
        {
            if (PersonModel.Gender == EGender.Other)
            {
                return Dictionary.sexuality_gender_other;
            }

            var text = string.Empty;
            var potentialMatesText = isGay ? "potential mates" : "members of the opposite sex";
            var sexualPartnersText = isGay ? "their sexual partners" : "members of the opposite sex";

            switch (SexualityRelationType)
            {
                case ESexualityRelationType.MatchMatch:
                    text = PersonModel.Gender == EGender.Male
                        ? Dictionary.sexuality_match_match_male
                        : Dictionary.sexuality_match_match_female;
                    break;

                case ESexualityRelationType.MatchOpposite:
                    text = PersonModel.Gender == EGender.Male
                        ? Dictionary.sexuality_match_opposite_male
                        : Dictionary.sexuality_match_opposite_female;
                    break;

                case ESexualityRelationType.OppositeMatch:
                    text = PersonModel.Gender == EGender.Male
                        ? Dictionary.sexuality_opposite_match_male
                        : Dictionary.sexuality_opposite_match_female;
                    break;

                case ESexualityRelationType.OppositeOpposite:
                    text = PersonModel.Gender == EGender.Male
                        ? Dictionary.sexuality_opposite_opposite_male
                        : Dictionary.sexuality_opposite_opposite_female;
                    break;

                default:
                    return string.Empty;
            }

            // Apply sexuality specific vocabulary
            text = TemplateParser.Parse(text,
                new
                {
                    PotentialMatesText = potentialMatesText,
                    SexualPartnersText = sexualPartnersText
                });

            var gayNotes = isGay ? Dictionary.sexuality_gay_notes : string.Empty;

            return $"{text} {gayNotes}".Trim();
        }

        private static readonly Dictionary<ENineStarKiEnergy, Func<EGender, string>> _sexualityDetails
            = new Dictionary<ENineStarKiEnergy, Func<EGender, string>>
            {
                { ENineStarKiEnergy.Water, _ => Dictionary.water_sexuality },
                { ENineStarKiEnergy.Soil, g => g == EGender.Female ? Dictionary.soil_sexuality_female : Dictionary.soil_sexuality_male },
                { ENineStarKiEnergy.Thunder, g => g == EGender.Female ? Dictionary.thunder_sexuality_female : Dictionary.thunder_sexuality_male },
                { ENineStarKiEnergy.Wind, _ => Dictionary.wind_sexuality },
                { ENineStarKiEnergy.CoreEarth, _ => Dictionary.coreearth_sexuality },
                { ENineStarKiEnergy.Heaven, g => g == EGender.Female ? Dictionary.heaven_sexuality_female : Dictionary.heaven_sexuality_male },
                { ENineStarKiEnergy.Lake, _ => Dictionary.lake_sexuality },
                { ENineStarKiEnergy.Mountain, _ => Dictionary.mountain_sexuality },
                { ENineStarKiEnergy.Fire, _ => Dictionary.fire_sexuality }
            };

        private string GetMainEnergySexualityDetails()
        {
            return MainEnergy != null ? _sexualityDetails.TryGetValue(MainEnergy.Energy, out var getSexuality)
                ? getSexuality(PersonModel.Gender)
                : string.Empty : string.Empty;
        }
    }
}