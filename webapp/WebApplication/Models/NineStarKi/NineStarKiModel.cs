using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using K9.WebApplication.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Script.Serialization;
using K9.SharedLibrary.Extensions;

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
        public bool EnableCycleSwitch { get; set; } = true;

        [ScriptIgnore]
        public bool UseHolograhpicCycleCalculation { get; set; } = SessionHelper.GetCurrentUserUseHolograhpicCycles();

        [ScriptIgnore]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.InvertDailyAndHourlyKiForSouthernHemisphereLabel)]
        public bool InvertDailyAndHourlyKiForSouthernHemisphere { get; set; } = SessionHelper.GetInvertDailyAndHourlyKiForSouthernHemisphere();

        [ScriptIgnore]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.InvertDailyAndHourlyKiForSouthernHemisphereLabel)]
        public bool InvertDailyAndHourlyCycleKiForSouthernHemisphere { get; set; } = SessionHelper.GetInvertDailyAndHourlyCycleKiForSouthernHemisphere();

        [ScriptIgnore]
        public bool IsPredictionsScreen { get; set; }

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
                BirthTimeZoneId = Current.UserTimeZoneId
            };

            PersonModel = personModel;
            UserTimeZoneId = Current.UserTimeZoneId;
            SelectedTime = new TimeSpan?();
        }

        public NineStarKiModel(PersonModel personModel)
        {
            PersonModel = personModel;
            UserTimeZoneId = Current.UserTimeZoneId;
            SelectedTime = new TimeSpan?();
        }

        public NineStarKiModel(PersonModel personModel, int precisePersonEpochEnergy, int precisePersonGenerationalEnergy, int preciseMainEnergy, int preciseEmotionalEnergy, int preciseEmotionalEnergyForInvertedYear, int precisePersonalDayStarEnergy, int precisePersonalHourlyEnergy,

            int preciseEpochCycleEnergy, int preciseGenerationalCycleEnergy, int preciseYearlyCycleEnergy, int preciseMonthlyCycleEnergy, (int DailyKi, int? InvertedDailyKi)[] preciseDailyCycleEnergies, int preciseHourlyCycleEnergy, DateTime? selectedDate = null,

            ECalculationMethod calculationMethod = ECalculationMethod.Chinese, bool useHolograhpicCycleCalculation = false, bool invertDailyAndHourlyKiForSouthernHemisphere = false, bool invertDailyAndHourlyCycleKiForSouthernHemisphere = false, string displayDataForTimeZoneId = "")
        {
            UserTimeZoneId = string.IsNullOrEmpty(displayDataForTimeZoneId) ? Current.UserTimeZoneId : displayDataForTimeZoneId;
            SelectedDate = selectedDate ?? DateTime.UtcNow;
            CalculationMethod = calculationMethod;
            UseHolograhpicCycleCalculation = useHolograhpicCycleCalculation;
            InvertDailyAndHourlyKiForSouthernHemisphere = invertDailyAndHourlyKiForSouthernHemisphere;
            InvertDailyAndHourlyCycleKiForSouthernHemisphere = invertDailyAndHourlyCycleKiForSouthernHemisphere;

            PersonModel = personModel;
            PersonalChartEnergies = new NineStarKiEnergiesModel();
            GlobalCycleEnergies = new NineStarKiEnergiesModel();
            PersonalHousesOccupiedEnergies = new NineStarKiEnergiesModel();

            preciseEmotionalEnergy = PersonModel.Gender.IsYin() && CalculationMethod == ECalculationMethod.Chinese
                ? preciseEmotionalEnergyForInvertedYear
                : preciseEmotionalEnergy;

            precisePersonalDayStarEnergy = InvertDailyAndHourlyKiForSouthernHemisphere
                ? GetOppositeEnergyInMagicSquare(precisePersonalDayStarEnergy)
                : precisePersonalDayStarEnergy;

            var preciseDailyCycleMorningEnergy = preciseDailyCycleEnergies[0].DailyKi;
            var preciseDailyCycleAfternoonEnergy = preciseDailyCycleEnergies[1].DailyKi;
            var preciseInvertedDailyCycleMorningEnergy = preciseDailyCycleEnergies[0].InvertedDailyKi;
            var preciseInvertedDailyCycleAfternoonEnergy = preciseDailyCycleEnergies[1].InvertedDailyKi;

            preciseDailyCycleMorningEnergy = InvertDailyAndHourlyCycleKiForSouthernHemisphere
                ? GetOppositeEnergyInMagicSquare(preciseDailyCycleMorningEnergy)
                : preciseDailyCycleMorningEnergy;

            preciseInvertedDailyCycleMorningEnergy = preciseInvertedDailyCycleMorningEnergy.HasValue && InvertDailyAndHourlyCycleKiForSouthernHemisphere
                ? GetOppositeEnergyInMagicSquare(preciseInvertedDailyCycleMorningEnergy.Value)
                : preciseInvertedDailyCycleMorningEnergy;

            preciseDailyCycleAfternoonEnergy = InvertDailyAndHourlyCycleKiForSouthernHemisphere
                ? GetOppositeEnergyInMagicSquare(preciseDailyCycleAfternoonEnergy)
                : preciseDailyCycleAfternoonEnergy;

            preciseInvertedDailyCycleAfternoonEnergy = preciseInvertedDailyCycleAfternoonEnergy.HasValue && InvertDailyAndHourlyCycleKiForSouthernHemisphere
                ? GetOppositeEnergyInMagicSquare(preciseInvertedDailyCycleAfternoonEnergy.Value)
                : preciseInvertedDailyCycleAfternoonEnergy;

            precisePersonalHourlyEnergy = InvertDailyAndHourlyKiForSouthernHemisphere
                ? GetOppositeEnergyInMagicSquare(precisePersonalHourlyEnergy)
                : precisePersonalHourlyEnergy;

            preciseHourlyCycleEnergy = InvertDailyAndHourlyCycleKiForSouthernHemisphere
                ? GetOppositeEnergyInMagicSquare(preciseHourlyCycleEnergy)
                : preciseHourlyCycleEnergy;

            var personalInfoString =
                $"{PersonModel.DateOfBirth}_{PersonModel.TimeOfBirth}_{PersonModel.BirthTimeZoneId}_{PersonModel.Gender}";


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

            PersonalChartEnergies.Day = GetOrAddToCache($"Day_p_{personalInfoString}_{precisePersonalDayStarEnergy}_{InvertDailyAndHourlyKiForSouthernHemisphere}",
                () => GetPersonalEnergy(precisePersonalDayStarEnergy, ENineStarKiEnergyType.DailyEnergy), TimeSpan.FromDays(30));

            PersonalChartEnergies.Hour = GetOrAddToCache($"Hour_p_{personalInfoString}_{precisePersonalHourlyEnergy}_{InvertDailyAndHourlyKiForSouthernHemisphere}",
                () => GetPersonalEnergy(precisePersonalHourlyEnergy, ENineStarKiEnergyType.HourlyEnergy), TimeSpan.FromDays(30));

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

            GlobalCycleEnergies.Day = GetOrAddToCache($"Day_g_{SelectedDate:yyyyMMddHH}_{preciseDailyCycleMorningEnergy}_{preciseInvertedDailyCycleMorningEnergy}",
                () => GetGlobalCycleEnergy(preciseDailyCycleMorningEnergy, ENineStarKiEnergyCycleType.DailyEnergy), TimeSpan.FromDays(30));

            GlobalCycleEnergies.Hour = GetOrAddToCache($"Hour_g_{SelectedDate:yyyyMMddHH}_{preciseHourlyCycleEnergy}",
                () => GetGlobalCycleEnergy(preciseHourlyCycleEnergy, ENineStarKiEnergyCycleType.HourlyEnergy), TimeSpan.FromDays(30));

            #endregion


            #region Personal Cycle Energies / Houses Occupied

            PersonalHousesOccupiedEnergies.Epoch = GetOrAddToCache($"Epoch_h_{SelectedDate}_{personalInfoString}_{preciseEpochCycleEnergy}_{UseHolograhpicCycleCalculation}",
                () => GetPersonalCycleEnergy(preciseEpochCycleEnergy, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Epoch.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.EpochEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Generation = GetOrAddToCache($"Generation_h_{SelectedDate}_{preciseGenerationalCycleEnergy}_{UseHolograhpicCycleCalculation}",
                () => GetPersonalCycleEnergy(preciseGenerationalCycleEnergy, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Generation.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.GenerationalEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Year = GetOrAddToCache($"Year_h_{SelectedDate}_{personalInfoString}_{preciseYearlyCycleEnergy}_{UseHolograhpicCycleCalculation}",
                () => GetPersonalCycleEnergy(preciseYearlyCycleEnergy, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.YearlyCycleEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Month = GetOrAddToCache($"Month_h_{SelectedDate}_{personalInfoString}_{preciseMonthlyCycleEnergy}_{UseHolograhpicCycleCalculation}",
                () => GetPersonalCycleEnergy(preciseMonthlyCycleEnergy, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Month.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.MonthlyCycleEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Day = GetOrAddToCache($"Day_h_{SelectedDate:yyyyMMddHH}_{personalInfoString}_{preciseDailyCycleMorningEnergy}_{preciseInvertedDailyCycleMorningEnergy}_{UseHolograhpicCycleCalculation}_{InvertDailyAndHourlyCycleKiForSouthernHemisphere}",
                () => GetPersonalCycleEnergy(preciseDailyCycleMorningEnergy, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Day.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Day2 = GetOrAddToCache($"DayTwo_h_{SelectedDate:yyyyMMddHH}_{personalInfoString}_{preciseDailyCycleAfternoonEnergy}_{preciseInvertedDailyCycleAfternoonEnergy}_{UseHolograhpicCycleCalculation}_{InvertDailyAndHourlyCycleKiForSouthernHemisphere}",
                () => GetPersonalCycleEnergy(preciseDailyCycleAfternoonEnergy, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Day.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy), TimeSpan.FromDays(30));

            PersonalHousesOccupiedEnergies.Hour = GetOrAddToCache($"Hour_h_{SelectedDate:yyyyMMddHHmm}_{personalInfoString}_{preciseHourlyCycleEnergy}_{UseHolograhpicCycleCalculation}_{InvertDailyAndHourlyCycleKiForSouthernHemisphere}",
                () => GetPersonalCycleEnergy(preciseHourlyCycleEnergy, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Hour.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.HourlyEnergy), TimeSpan.FromDays(30));

            if (preciseInvertedDailyCycleMorningEnergy.HasValue)
            {
                PersonalHousesOccupiedEnergies.DayInverted = GetOrAddToCache($"DayInverted_h_{SelectedDate:yyyyMMddHH}_{personalInfoString}_{preciseInvertedDailyCycleMorningEnergy}_{preciseInvertedDailyCycleMorningEnergy}_{UseHolograhpicCycleCalculation}_{InvertDailyAndHourlyCycleKiForSouthernHemisphere}",
                    () => GetPersonalCycleEnergy(preciseInvertedDailyCycleMorningEnergy.Value, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Hour.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy), TimeSpan.FromDays(30));

                PersonalHousesOccupiedEnergies.Day2Inverted = GetOrAddToCache($"DayTwoInverted_h_{SelectedDate:yyyyMMddHH}_{personalInfoString}_{preciseInvertedDailyCycleAfternoonEnergy}_{preciseInvertedDailyCycleAfternoonEnergy}_{UseHolograhpicCycleCalculation}_{InvertDailyAndHourlyCycleKiForSouthernHemisphere}",
                    () => GetPersonalCycleEnergy(preciseInvertedDailyCycleAfternoonEnergy.Value, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Hour.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy), TimeSpan.FromDays(30));
            }

            #endregion


            MainEnergy.RelatedEnergy = CharacterEnergy.Energy;
            CharacterEnergy.RelatedEnergy = MainEnergy.Energy;
            SurfaceEnergy.RelatedEnergy = MainEnergy.Energy;
        }

        public PersonModel PersonModel { get; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LichunLabel)]
        public DateTime? Lichun { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SelectedDateLabel)]
        public DateTime? SelectedDate { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SelectedTimeLabel)]
        public TimeSpan? SelectedTime { get; set; }

        [UIHint("DisplayDataForPeriod")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DisplayDataForLabel)]
        public EDisplayDataForPeriod DisplayDataForPeriod { get; set; }

        [UIHint("TimeZone")]
        [Required(ErrorMessageResourceType = typeof(Base.Globalisation.Dictionary),
            ErrorMessageResourceName = Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.UserTimeZone)]
        public string UserTimeZoneId { get; set; }

        public NineStarKiEnergiesModel PersonalChartEnergies { get; }

        public NineStarKiEnergiesModel GlobalCycleEnergies { get; }

        public NineStarKiEnergiesModel PersonalHousesOccupiedEnergies { get; }

        public NineStarKiEnergiesModel PersonalHousesOccupiedIsometricEnergies { get; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MainEnergyLabel)]
        public NineStarKiEnergy MainEnergy => PersonalChartEnergies?.Year;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergy CharacterEnergy => PersonalChartEnergies?.Month;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergy SurfaceEnergy => PersonalChartEnergies?.Surface;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.YearlyEnergyLabel)]
        public NineStarKiEnergy YearlyCycleEnergy => PersonalHousesOccupiedEnergies?.Year;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MonthlyEnergyLabel)]
        public NineStarKiEnergy MonthlyCycleEnergy => PersonalHousesOccupiedEnergies?.Month;

        public NineStarKiEnergy GetHouseOfFive(int energy) => new NineStarKiEnergy((ENineStarKiEnergy)GetNineStarKiNumber(energy + (5 - MainEnergy?.EnergyNumber ?? 0)));

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SummaryLabel)]
        public string Summary => GetSummary();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.OverviewLabel)]
        public string Overview => MainEnergy?.OverviewDescription;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.HealthLabel)]
        public string Health => MainEnergy?.HealthDescription;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.OccupationsLabel)]
        public string Career => MainEnergy?.CareerDescription;

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
        public string EnergySexualityLabel => MainEnergy != null ? $"{MainEnergy.EnergyNameAndNumber} {Dictionary.Relationships}" : "";

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

        public (DateTime PeriodStartOn, DateTime PeriodEndsOn, int YearlyKi)[] YearlyPeriods { get; set; }

        public (DateTime PeriodStartOn, DateTime PeriodEndsOn, int MonthlyKi)[] MonthlyPeriods { get; set; }

        public (DateTime Date, int DailyKi, int? InvertedDailyKi, int? AfternoonKi, int? InvertedAfternoonKi)[] DailyPeriods { get; set; }

        public List<Tuple<int, DateTime, DateTime, NineStarKiEnergy, List<Tuple<int, int, DateTime, DateTime, string, NineStarKiEnergy>>>> GetGlobalPlanner()
        {
            return GetOrAddToCache($"GlobalPlanner_{SelectedDate.ToString()}", () =>
            {
                var yearlyCycles = new List<Tuple<int, DateTime, DateTime, NineStarKiEnergy, List<Tuple<int, int, DateTime, DateTime, string, NineStarKiEnergy>>>>();

                foreach (var yearlyPeriod in YearlyPeriods)
                {
                    var monthlyCycles = new List<Tuple<int, int, DateTime, DateTime, string, NineStarKiEnergy>>();
                    var monthlyPeriodsForYear =
                        MonthlyPeriods.Where(e => e.PeriodStartOn.IsBetween(yearlyPeriod.PeriodStartOn, yearlyPeriod.PeriodEndsOn)
                                                  || e.PeriodEndsOn.IsBetween(yearlyPeriod.PeriodStartOn, yearlyPeriod.PeriodEndsOn)).OrderBy(e => e.PeriodStartOn).ToList();
                    foreach (var monthlyPeriod in monthlyPeriodsForYear)
                    {
                        monthlyCycles.Add(new Tuple<int, int, DateTime, DateTime, string, NineStarKiEnergy>(
                            monthlyPeriod.PeriodStartOn.Year,
                            monthlyPeriod.PeriodStartOn.Month,
                            monthlyPeriod.PeriodStartOn,
                            monthlyPeriod.PeriodEndsOn,

                            monthlyPeriod.PeriodStartOn.ToString("MMM"), GetPersonalCycleEnergy(monthlyPeriod.MonthlyKi, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Month.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.MonthlyCycleEnergy)));
                    }

                    yearlyCycles.Add(new Tuple<int, DateTime, DateTime, NineStarKiEnergy, List<Tuple<int, int, DateTime, DateTime, string, NineStarKiEnergy>>>(
                        yearlyPeriod.PeriodStartOn.Year,
                        yearlyPeriod.PeriodStartOn,
                        yearlyPeriod.PeriodEndsOn,

                        GetPersonalCycleEnergy(yearlyPeriod.YearlyKi, MainEnergy.EnergyNumber,
                            ENineStarKiEnergyCycleType.YearlyCycleEnergy),

                        monthlyCycles));
                }

                return yearlyCycles;
            }, TimeSpan.FromDays(30));
        }

        public List<Tuple<int, NineStarKiEnergy>> GetYearlyPlanner()
        {
            return GetOrAddToCache($"YearlyPlanner_{SelectedDate.ToString()}", () =>
            {
                var cycles = new List<Tuple<int, NineStarKiEnergy>>();

                foreach (var yearlyPeriod in YearlyPeriods)
                {
                    cycles.Add(new Tuple<int, NineStarKiEnergy>(yearlyPeriod.PeriodStartOn.Year, GetPersonalCycleEnergy(yearlyPeriod.YearlyKi, MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.YearlyCycleEnergy)));
                }

                return cycles;
            }, TimeSpan.FromDays(30));
        }

        public List<Tuple<int, int, string, NineStarKiEnergy>> GetMonthlyPlanner()
        {
            return GetOrAddToCache($"MonthlyPlanner_{SelectedDate.ToString()}", () =>
            {
                var cycles = new List<Tuple<int, int, string, NineStarKiEnergy>>();

                foreach (var monthlyPeriod in MonthlyPeriods)
                {
                    cycles.Add(new Tuple<int, int, string, NineStarKiEnergy>(monthlyPeriod.PeriodStartOn.Year, monthlyPeriod.PeriodStartOn.Month, monthlyPeriod.PeriodStartOn.ToString("MMMM"), GetPersonalCycleEnergy(monthlyPeriod.MonthlyKi, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Month.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.MonthlyCycleEnergy)));
                }

                return cycles;

            }, TimeSpan.FromDays(30));
        }

        public List<Tuple<int, int, int, string, string, NineStarKiEnergy, NineStarKiEnergy>> GetDailyPlanner()
        {
            return GetOrAddToCache($"DailyPlanner_{SelectedDate.ToString()}", () =>
            {
                var cycles = new List<Tuple<int, int, int, string, string, NineStarKiEnergy, NineStarKiEnergy>>();

                foreach (var day in DailyPeriods)
                {
                    cycles.Add(new Tuple<int, int, int, string, string, NineStarKiEnergy, NineStarKiEnergy>(day.Date.Year, day.Date.Month, day.Date.Day, day.Date.ToString("MMM"), day.Date.ToString("ddd"),

                        GetPersonalCycleEnergy(InvertDailyAndHourlyCycleKiForSouthernHemisphere ? GetOppositeEnergyInMagicSquare(day.DailyKi) : day.DailyKi, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Day.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy),

                        day.AfternoonKi == null ? null : GetPersonalCycleEnergy(InvertDailyAndHourlyCycleKiForSouthernHemisphere ? GetOppositeEnergyInMagicSquare(day.AfternoonKi.Value) : day.AfternoonKi.Value, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Day.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy)
                        ));
                }

                return cycles;

            }, TimeSpan.FromDays(30));
        }

        public List<Tuple<int, int, int, string, string, NineStarKiEnergy>> GetHourlyPlanner()
        {
            return GetOrAddToCache($"HourlyPlanner_{SelectedDate.ToString()}", () =>
            {
                var cycles = new List<Tuple<int, int, int, string, string, NineStarKiEnergy>>();

                foreach (var hour in DailyPeriods)
                {
                    cycles.Add(new Tuple<int, int, int, string, string, NineStarKiEnergy>(hour.Date.Year, hour.Date.Month, hour.Date.Day, hour.Date.ToString("MMM"), hour.Date.ToString("ddd"), GetPersonalCycleEnergy(hour.DailyKi, UseHolograhpicCycleCalculation ? PersonalChartEnergies.Day.EnergyNumber : MainEnergy.EnergyNumber, ENineStarKiEnergyCycleType.DailyEnergy)));
                }

                return cycles;

            }, TimeSpan.FromDays(30));
        }

        /// <summary>
        /// Ensures number is always in the range 1-9
        /// </summary>
        /// <param name="energyNumber"></param>
        /// <returns></returns>
        public static int GetNineStarKiNumber(int energyNumber) => (energyNumber + 8) % 9 + 1;

        public DateTime GetLocalNow()
        {
            return string.IsNullOrEmpty(PersonModel?.BirthTimeZoneId)
                ? DateTime.UtcNow
                : DateTimeHelper.ConvertToLocaleDateTime(DateTime.UtcNow, PersonModel.BirthTimeZoneId);
        }

        private NineStarKiEnergy GetPersonalEnergy(int energyNumber, ENineStarKiEnergyType energyType)
        {
            energyNumber = GetNineStarKiNumber(energyNumber);
            if (PersonModel.Gender.IsYin() && CalculationMethod == ECalculationMethod.Chinese)
            {
                energyNumber = InvertEnergy(energyNumber);
            }

            return new NineStarKiEnergy((ENineStarKiEnergy)energyNumber, energyType, PersonModel.IsAdult());
        }

        private NineStarKiEnergy GetSurfaceEnergy()
        {
            var surfaceEnergyNumber = GetNineStarKiNumber(5 - (CharacterEnergy.EnergyNumber - MainEnergy.EnergyNumber));
            return new NineStarKiEnergy((ENineStarKiEnergy)surfaceEnergyNumber, ENineStarKiEnergyType.SurfaceEnergy);
        }

        private NineStarKiEnergy GetGlobalCycleEnergy(int cycleEnergy, ENineStarKiEnergyCycleType cycleType)
        {
            cycleEnergy = IsCycleSwitchActive ? InvertEnergy(cycleEnergy) : cycleEnergy;

            var energy = (ENineStarKiEnergy)cycleEnergy;

            return new NineStarKiEnergy(energy, cycleType);
        }

        private NineStarKiEnergy GetPersonalCycleEnergy(int cycleEnergy, int energyNumber, ENineStarKiEnergyCycleType cycleType)
        {
            var invertCycle = (CalculationMethod == ECalculationMethod.Chinese && PersonModel.Gender.IsYin()) || IsCycleSwitchActive;
            cycleEnergy = invertCycle ? InvertEnergy(cycleEnergy) : cycleEnergy;
            var houseOccupied = GetHouseOccupiedByNumber(cycleEnergy, energyNumber);

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

        private static readonly Dictionary<ENineStarKiEnergy, Func<EGender, string>> _relationshipDetails
            = new Dictionary<ENineStarKiEnergy, Func<EGender, string>>
            {
                { ENineStarKiEnergy.Water, _ => Dictionary.water_relationships },
                { ENineStarKiEnergy.Soil, g => g == EGender.Female ? Dictionary.soil_relationships_female : Dictionary.soil_relationships_male },
                { ENineStarKiEnergy.Thunder, g => g == EGender.Female ? Dictionary.thunder_relationships_female : Dictionary.thunder_relationships_male },
                { ENineStarKiEnergy.Wind, _ => Dictionary.wind_relationships },
                { ENineStarKiEnergy.CoreEarth, _ => Dictionary.coreearth_relationships },
                { ENineStarKiEnergy.Heaven, g => g == EGender.Female ? Dictionary.heaven_relationships_female : Dictionary.heaven_relationships_male },
                { ENineStarKiEnergy.Lake, _ => Dictionary.lake_relationships },
                { ENineStarKiEnergy.Mountain, _ => Dictionary.mountain_relationships },
                { ENineStarKiEnergy.Fire, _ => Dictionary.fire_relationships }
            };

        private string GetMainEnergySexualityDetails()
        {
            return MainEnergy != null ? _relationshipDetails.TryGetValue(MainEnergy.Energy, out var getSexuality)
                ? getSexuality(PersonModel.Gender)
                : string.Empty : string.Empty;
        }

        private string GetSummary()
        {
            var cacheKey = $"GetSummary_{PersonModel.IsAdult()}_{MainEnergy.Energy}_{CharacterEnergy.Energy}";
            return GetOrAddToCache(cacheKey, () =>
            {
                if (!PersonModel.IsAdult())
                {
                    switch (CharacterEnergy.Energy)
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
                    switch (MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            switch (CharacterEnergy.Energy)
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
                            switch (CharacterEnergy.Energy)
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
                            switch (CharacterEnergy.Energy)
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
                            switch (CharacterEnergy.Energy)
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
                            switch (CharacterEnergy.Energy)
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
                            switch (CharacterEnergy.Energy)
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
                            switch (CharacterEnergy.Energy)
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
                            switch (CharacterEnergy.Energy)
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
                            switch (CharacterEnergy.Energy)
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

    }
}