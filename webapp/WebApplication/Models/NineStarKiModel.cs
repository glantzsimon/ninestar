using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using K9.SharedLibrary.Helpers;

namespace K9.WebApplication.Models
{

    public class NineStarKiModel
    {
        public NineStarKiModel()
        {
            PersonModel = new PersonModel();
            Today = DateTime.Now;
        }

        public NineStarKiModel(PersonModel personModel)
        {
            PersonModel = personModel;
            Today = DateTime.Now;

            MainEnergy = GetMainEnergy(PersonModel.DateOfBirth, PersonModel.Gender);
            CharacterEnergy = GetCharacterEnergy(PersonModel.DateOfBirth, MainEnergy.Energy, personModel.Gender);
            SurfaceEnergy = GetSurfaceEnergy();
            LifeCycleYearEnergy = GetLifeCycleYearEnergy();
            LifeCycleMonthEnergy = GetLifeCycleMonthEnergy();

            MainEnergy.RelatedEnergy = CharacterEnergy.Energy;
            CharacterEnergy.RelatedEnergy = MainEnergy.Energy;
            SurfaceEnergy.RelatedEnergy = MainEnergy.Energy;
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

        /// <summary>
        /// For testing purposes only
        /// </summary>
        public DateTime Today { get; set; }

        /// <summary>
        /// Determines the 9 Star Ki energy of the current year
        /// </summary>
        public ENineStarKiEnergy LifeCycleYearEnergy { get; }

        /// <summary>
        /// Determines the 9 Star Ki energy of the current month
        /// </summary>
        public ENineStarKiEnergy LifeCycleMonthEnergy { get; }

        public EReadingType ReadingType { get; set; }

        public ESexualityRelationType SexualityRelationType => GetSexualityRelationType();

        public string OverviewLabel => $"Overview of {MainEnergy.EnergyNameAndNumber} Fundamental Nature";

        public string EnergySexualityLabel => $"{MainEnergy.EnergyName} {Dictionary.SexualityLabel}";

        public string SexualityRelationTypeLabel => "Sexuality relative to Gender and Personal Profile";

        public string SexualityRelationTypeDetailsStraight => GetSexualityGenderDescription();

        public string SexualityRelationTypeDetailsGay => GetSexualityGenderDescription(true);

        public string MainEnergySexualityDetails => GetMainEnergySexualityDetails();

        public string GayLabel => PersonModel?.Gender == EGender.Female ? Dictionary.Lesbian : Dictionary.Gay;

        public bool IsProcessed { get; set; } = false;

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

        private NineStarKiEnergy GetCharacterEnergy(DateTime date, ENineStarKiEnergy energy, EGender gender)
        {
            var energyNumber = 0;
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

            switch (energy)
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

            return ProcessEnergy(energyNumber, gender, ENineStarKiEnergyType.CharacterEnergy);
        }

        private NineStarKiEnergy GetSurfaceEnergy()
        {
            return ProcessEnergy(5 - (CharacterEnergy.EnergyNumber - MainEnergy.EnergyNumber), EGender.Male, ENineStarKiEnergyType.SurfaceEnergy);
        }

        private ENineStarKiEnergy GetLifeCycleYearEnergy()
        {
            var todayYearEnergy = (int)GetMainEnergy(Today, EGender.Male).Energy;
            var personalYearEnergy = (PersonModel.Gender.IsYin() ? InvertEnergy(MainEnergy.EnergyNumber) : MainEnergy.EnergyNumber);
            var offset = todayYearEnergy - personalYearEnergy;
            var lifeCycleYearEnergy = LoopEnergyNumber(5 - offset);

            return (ENineStarKiEnergy)(PersonModel.Gender.IsYin() ? InvertEnergy(lifeCycleYearEnergy) : lifeCycleYearEnergy);
        }

        private ENineStarKiEnergy GetLifeCycleMonthEnergy()
        {
            var yearEnergy = GetLifeCycleYearEnergy();
            return GetCharacterEnergy(Today, yearEnergy, MainEnergy.Gender).Energy;
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

        /// <summary>
        /// Takes care of numbers less than 1 or over 9 and loops them back into the nine star key energy numbers
        /// </summary>
        /// <param name="energyNumber"></param>
        /// <returns></returns>
        private static int LoopEnergyNumber(int energyNumber)
        {
            if (energyNumber < 1)
            {
                energyNumber = 9 + energyNumber;
            }
            else if (energyNumber > 9)
            {
                energyNumber = energyNumber - 9;
            }
            return energyNumber;
        }

        private int InvertEnergy(int energyNumber)
        {
            switch (energyNumber)
            {
                case 1:
                    return 5;

                case 2:
                    return 4;

                case 4:
                    return 2;

                case 5:
                    return 1;

                case 6:
                    return 9;

                case 7:
                    return 8;

                case 8:
                    return 7;

                case 9:
                    return 6;

                default:
                    return energyNumber;
            }
        }

        private ESexualityRelationType GetSexualityRelationType()
        {
            if (PersonModel != null && MainEnergy != null && CharacterEnergy != null)
            {
                if (PersonModel.Gender.IsYin())
                {
                    if (MainEnergy.YinYang == ENineStarKiYinYang.Yin &&
                        CharacterEnergy.YinYang == ENineStarKiYinYang.Yin)
                    {
                        return ESexualityRelationType.MatchMatch;
                    }

                    if (MainEnergy.YinYang == ENineStarKiYinYang.Yang &&
                        CharacterEnergy.YinYang == ENineStarKiYinYang.Yang)
                    {
                        return ESexualityRelationType.OppositeOpposite;
                    }

                    if (MainEnergy.YinYang == ENineStarKiYinYang.Yin &&
                        CharacterEnergy.YinYang == ENineStarKiYinYang.Yang)
                    {
                        return ESexualityRelationType.MatchOpposite;
                    }

                    if (MainEnergy.YinYang == ENineStarKiYinYang.Yang &&
                        CharacterEnergy.YinYang == ENineStarKiYinYang.Yin)
                    {
                        return ESexualityRelationType.OppositeMatch;
                    }
                }
                else
                {
                    if (MainEnergy.YinYang == ENineStarKiYinYang.Yin &&
                        CharacterEnergy.YinYang == ENineStarKiYinYang.Yin)
                    {
                        return ESexualityRelationType.OppositeOpposite;
                    }

                    if (MainEnergy.YinYang == ENineStarKiYinYang.Yang &&
                        CharacterEnergy.YinYang == ENineStarKiYinYang.Yang)
                    {
                        return ESexualityRelationType.MatchMatch;
                    }

                    if (MainEnergy.YinYang == ENineStarKiYinYang.Yin &&
                        CharacterEnergy.YinYang == ENineStarKiYinYang.Yang)
                    {
                        return ESexualityRelationType.OppositeMatch;
                    }

                    if (MainEnergy.YinYang == ENineStarKiYinYang.Yang &&
                        CharacterEnergy.YinYang == ENineStarKiYinYang.Yin)
                    {
                        return ESexualityRelationType.MatchOpposite;
                    }
                }
            }

            return ESexualityRelationType.Unspecified;
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
            text = TemplateProcessor.PopulateTemplate(text,
                new
                {
                    PotentialMatesText = potentialMatesText,
                    SexualPartnersText = sexualPartnersText
                });

            var gayNotes = isGay ? Dictionary.sexuality_gay_notes : string.Empty;

            return $"{text} {gayNotes}".Trim();
        }

        private string GetMainEnergySexualityDetails()
        {
            switch (MainEnergy.Energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_sexuality;

                case ENineStarKiEnergy.Soil:
                    return PersonModel.Gender == EGender.Female
                        ? Dictionary.soil_sexuality_female
                        : Dictionary.soil_sexuality_male;

                case ENineStarKiEnergy.Thunder:
                    return PersonModel.Gender == EGender.Female
                        ? Dictionary.thunder_sexuality_female
                        : Dictionary.thunder_sexuality_male;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_sexuality;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_sexuality;

                case ENineStarKiEnergy.Heaven:
                    return PersonModel.Gender == EGender.Female
                        ? Dictionary.heaven_sexuality_female
                        : Dictionary.heaven_sexuality_male;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_sexuality;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_sexuality;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_sexuality;
            }

            return string.Empty;
        }
    }
}