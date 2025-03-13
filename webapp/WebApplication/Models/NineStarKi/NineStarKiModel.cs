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
        private const bool invertCycleYinEnergies = true;

        public NineStarKiModel()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = Methods.GetRandomGender()
            };

            PersonModel = personModel;
            BiorhythmResultSet = new BioRhythmsResultSet();
        }

        public NineStarKiModel(PersonModel personModel, DateTime? selectedDate = null)
        {
            SelectedDate = selectedDate ?? DateTime.Today;

            PersonModel = personModel;

            MainEnergy = GetOrAddToCache($"MainEnergy_{PersonModel.DateOfBirth}_{PersonModel.Gender}",
                () => GetMainEnergy(PersonModel.DateOfBirth, PersonModel.Gender), TimeSpan.FromDays(30));

            CharacterEnergy = GetOrAddToCache($"CharacterEnergy_{PersonModel.DateOfBirth}_{PersonModel.Gender}",
                () => GetCharacterEnergy(PersonModel.DateOfBirth, PersonModel.Gender), TimeSpan.FromDays(30));

            SurfaceEnergy = GetOrAddToCache($"SurfaceEnergy_{MainEnergy.EnergyNumber}_{CharacterEnergy.EnergyNumber}",
                GetSurfaceEnergy, TimeSpan.FromDays(30));

            YearlyCycleEnergy = GetOrAddToCache($"YearlyCycleEnergy_{PersonModel.DateOfBirth}_{PersonModel.Gender}_{SelectedDate.Value.ToString()}",
                GetYearlyCycleEnergy, TimeSpan.FromDays(30));

            MonthlyCycleEnergy = GetOrAddToCache($"MonthlyCycleEnergy_{PersonModel.DateOfBirth}_{PersonModel.Gender}_{SelectedDate.Value.ToString()}",
                GetMonthlyCycleEnergy, TimeSpan.FromDays(30));

            MainEnergy.RelatedEnergy = CharacterEnergy.Energy;
            CharacterEnergy.RelatedEnergy = MainEnergy.Energy;
            SurfaceEnergy.RelatedEnergy = MainEnergy.Energy;

            BiorhythmResultSet = new BioRhythmsResultSet(this, selectedDate);
        }

        public PersonModel PersonModel { get; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MainEnergyLabel)]
        public NineStarKiEnergy MainEnergy { get; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergy CharacterEnergy { get; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SurfaceEnergyLabel)]
        public NineStarKiEnergy SurfaceEnergy { get; }

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
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.BiorhythmsLabel)]
        public BioRhythmsResultSet BiorhythmResultSet { get; set; }

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

        /// <summary>
        /// Determines the 9 Star Ki energy of the current year
        /// </summary>
        public NineStarKiEnergy YearlyCycleEnergy { get; }

        /// <summary>
        /// Where Core Earth is for this year - important for directionality
        /// </summary>
        public NineStarKiEnergy YearlyCycleCoreEarthEnergy { get; set; }

        /// <summary>
        /// Determines the 9 Star Ki energy of the current month
        /// </summary>
        public NineStarKiEnergy MonthlyCycleEnergy { get; }

        public NineStarKiDirections YearlyDirections { get; set; }
        public NineStarKiDirections MonthlyDirections { get; set; }
        
        /// <summary>
        /// Where Core Earth is for this month - important for directionality
        /// </summary>
        public NineStarKiEnergy MonthlyCycleCoreEarthEnergy { get; set; }

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

        public List<Tuple<int, NineStarKiEnergy>> GetYearlyPlanner()
        {
            return GetOrAddToCache($"YearlyPlanner_{DateTime.Today.Year}", () =>
            {
                var cycles = new List<Tuple<int, NineStarKiEnergy>>();
                var today = new DateTime(DateTime.Today.Year, 2, 15);

                for (int i = -20; i <= 20; i++)
                {
                    SelectedDate = today.AddYears(i);
                    cycles.Add(new Tuple<int, NineStarKiEnergy>(SelectedDate.Value.Year, GetYearlyCycleEnergy()));
                }

                SelectedDate = null;

                return cycles;
            }, TimeSpan.FromDays(30));
        }

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
                        cycles.Add(new Tuple<int, int, string, NineStarKiEnergy>(SelectedDate.Value.Year, SelectedDate.Value.Month, SelectedDate.Value.ToString("MMMM"), GetMonthlyCycleEnergy()));
                    }
                }

                SelectedDate = null;

                return cycles;

            }, TimeSpan.FromDays(30));
        }

        public static int LoopEnergyNumber(int energyNumber) => (energyNumber + 8) % 9 + 1;

        public NineStarKiDates GetMonthlyPeriod(DateTime date, EGender gender)
        {
            var month = GetMonth(date);
            var yearlyEnergy = GetMainEnergy(date, gender);
            var energyNumber = GetEnergyNumberFromYearlyEnergy(yearlyEnergy.Energy, month);
            var monthlyEnergy = ProcessEnergy(energyNumber, gender, ENineStarKiEnergyType.CharacterEnergy);

            var startDate = new DateTime(date.Year, month, GetMonthStartDay(month));
            var nextMonthDate = startDate.AddMonths(1);
            nextMonthDate = new DateTime(nextMonthDate.Year, nextMonthDate.Month, 15);

            var nextMonth = GetMonth(nextMonthDate);
            var nextMonthStartDay = GetMonthStartDay(nextMonth);
            var endDate = new DateTime(nextMonthDate.Year, nextMonthDate.Month, nextMonthStartDay - 1);

            return new NineStarKiDates
            {
                YearlyEnergy = yearlyEnergy.Energy,
                MonthlyEnergy = monthlyEnergy.Energy,
                MonthlyPeriodStartsOn = startDate,
                MonthlyPeriodEndsOn = endDate
            };
        }

        private NineStarKiEnergy GetMainEnergy(DateTime date, EGender gender)
        {
            var month = date.Month;
            var day = date.Day;
            var year = date.Year;

            year = (month == 2 && day <= 3) || month == 1 ? year - 1 : year;
            var energyNumber = 3 - ((year - 1979) % 9);

            var nineStarKiEnergy = ProcessEnergy(energyNumber, gender);
            nineStarKiEnergy.Gender = gender;

            return nineStarKiEnergy;
        }

        private int GetMonth(DateTime date)
        {
            var month = date.Month;
            var day = date.Day;

            switch (month)
            {
                case 2:
                    month = day >= 4 ? month : month - 1;
                    break;

                case 3:
                case 6:
                    month = day >= 6 ? month : month - 1;
                    break;

                case 4:
                case 5:
                    month = day >= 5 ? month : month - 1;
                    break;

                case 1:
                    month = day >= 5 ? month : 12;
                    break;

                case 7:
                case 8:
                case 11:
                case 12:
                    month = day >= 7 ? month : month - 1;
                    break;

                case 9:
                case 10:
                    month = day >= 8 ? month : month - 1;
                    break;
            }

            return month;
        }

        private int GetMonthStartDay(int month)
        {
            switch (month)
            {
                case 2:
                    return 4;

                case 3:
                case 6:
                    return 6;

                case 4:
                case 5:
                case 1:
                    return 5;

                case 7:
                case 8:
                case 11:
                case 12:
                    return 7;

                case 9:
                case 10:
                    return 8;
            }

            throw new ArgumentOutOfRangeException();
        }

        private int GetEnergyNumberFromYearlyEnergy(ENineStarKiEnergy yearlyEnergy, int month)
        {
            var energyNumber = 0;

            switch (yearlyEnergy)
            {
                case ENineStarKiEnergy.Thunder:
                case ENineStarKiEnergy.Heaven:
                case ENineStarKiEnergy.Fire:
                    energyNumber = 5;
                    break;

                case ENineStarKiEnergy.Water:
                case ENineStarKiEnergy.Wind:
                case ENineStarKiEnergy.Lake:
                    energyNumber = 8;
                    break;

                case ENineStarKiEnergy.Soil:
                case ENineStarKiEnergy.CoreEarth:
                case ENineStarKiEnergy.Mountain:
                    energyNumber = 2;
                    break;
            }

            switch (month)
            {
                case 2:
                case 11:
                    // no need to change
                    break;

                case 3:
                case 12:
                    energyNumber -= 1;
                    break;

                case 4:
                case 1:
                    energyNumber -= 2;
                    break;

                case 5:
                    energyNumber -= 3;
                    break;

                case 6:
                    energyNumber -= 4;
                    break;

                case 7:
                    energyNumber -= 5;
                    break;

                case 8:
                    energyNumber -= 6;
                    break;

                case 9:
                    energyNumber -= 7;
                    break;

                case 10:
                    energyNumber -= 8;
                    break;
            }

            return energyNumber;
        }

        private NineStarKiEnergy GetCharacterEnergy(DateTime date, EGender gender = EGender.Male)
        {
            var month = GetMonth(date);
            var yearlyEnergy = GetMainEnergy(date, gender).Energy;
            var energyNumber = GetEnergyNumberFromYearlyEnergy(yearlyEnergy, month);

            return ProcessEnergy(energyNumber, gender, ENineStarKiEnergyType.CharacterEnergy);
        }

        private NineStarKiEnergy GetSurfaceEnergy()
        {
            return ProcessEnergy(5 - (CharacterEnergy.EnergyNumber - MainEnergy.EnergyNumber), EGender.Male, ENineStarKiEnergyType.SurfaceEnergy);
        }

        private NineStarKiEnergy GetYearlyCycleEnergy()
        {
            var todayYearEnergy = (int)GetMainEnergy(SelectedDate ?? DateTime.Today, EGender.Male).Energy;
            /// Get the male energy for this calculation
            var personalYearEnergy = PersonModel.Gender.IsYin() ? InvertEnergy(MainEnergy.EnergyNumber) : MainEnergy.EnergyNumber;
            var offset = todayYearEnergy - personalYearEnergy;
            var lifeCycleYearEnergy = LoopEnergyNumber(5 - offset);

            var energy = (ENineStarKiEnergy)(PersonModel.Gender.IsYin() && invertCycleYinEnergies ? InvertEnergy(lifeCycleYearEnergy) : lifeCycleYearEnergy);

            return new NineStarKiEnergy(energy, ENineStarKiEnergyType.MainEnergy, PersonModel.IsAdult(), ENineStarKiEnergyCycleType.YearlyCycleEnergy);
        }

        private NineStarKiEnergy GetMonthlyCycleEnergy()
        {
            var month = GetMonth(SelectedDate.Value);
            var yearlyCycleEnergy = GetYearlyCycleEnergy().Energy;
            var energyNumber = GetEnergyNumberFromYearlyEnergy(yearlyCycleEnergy, month);
            var monthlyEnergy = ProcessEnergy(energyNumber, PersonModel.Gender, ENineStarKiEnergyType.CharacterEnergy);
            monthlyEnergy.EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy;
            return monthlyEnergy;
        }

        private NineStarKiEnergy ProcessEnergy(int energyNumber, EGender gender, ENineStarKiEnergyType type = ENineStarKiEnergyType.MainEnergy)
        {
            energyNumber = LoopEnergyNumber(energyNumber);
            if (gender.IsYin())
            {
                energyNumber = InvertEnergy(energyNumber);
            }

            return new NineStarKiEnergy((ENineStarKiEnergy)energyNumber, type, PersonModel.IsAdult());
        }

        private static readonly Dictionary<int, int> _invertedEnergies = new Dictionary<int, int>
        {
            { 1, 5 }, { 2, 4 }, { 4, 2 }, { 5, 1 },
            { 6, 9 }, { 7, 8 }, { 8, 7 }, { 9, 6 }
        };

        private int InvertEnergy(int energyNumber) =>
            _invertedEnergies.TryGetValue(energyNumber, out var inverted) ? inverted : energyNumber;
        
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