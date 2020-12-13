using K9.WebApplication.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

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

            MainEnergy = GetMainEnergy(PersonModel.DateOfBirth);
            EmotionalEnergy = GetEmotionalEnergy(PersonModel.DateOfBirth, MainEnergy.Energy);
            SurfaceEnergy = GetSurfaceEnergy();
            LifeCycleYearEnergy = GetLifeCycleYearEnergy();
            LifeCycleMonthEnergy = GetLifeCycleMonthEnergy();

            MainEnergy.RelatedEnergy = EmotionalEnergy.Energy;
        }

        public PersonModel PersonModel { get; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.MainEnergyLabel)]
        public NineStarKiEnergy MainEnergy { get; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EmotionalEnergyLabel)]
        public NineStarKiEnergy EmotionalEnergy { get; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.SurfaceEnergyLabel)]
        public NineStarKiEnergy SurfaceEnergy { get; }

        /// <summary>
        /// For testing purposes only
        /// </summary>
        public DateTime Today { get; set; }

        /// <summary>
        /// Determines the nine star ki energy of the current year
        /// </summary>
        public ENineStarKiEnergy LifeCycleYearEnergy { get; }

        /// <summary>
        /// Determines the nine star ki energy of the current month
        /// </summary>
        public ENineStarKiEnergy LifeCycleMonthEnergy { get; }
        
        private NineStarKiEnergy GetMainEnergy(DateTime date)
        {
            var month = date.Month;
            var day = date.Day;
            var year = date.Year;

            year = (month == 2 && day <= 3) || month == 1 ? year - 1 : year;
            var energyNumber = 3 - ((year - 1979) % 9);

            var nineStarKiEnergy = ProcessEnergy(energyNumber, true, ENineStarKiEnergyType.MainEnergy);
            nineStarKiEnergy.Gender = PersonModel.Gender;

            return nineStarKiEnergy;
        }

        private NineStarKiEnergy GetEmotionalEnergy(DateTime date, ENineStarKiEnergy energy, bool invertIfYin = true)
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

            return ProcessEnergy(energyNumber, invertIfYin, ENineStarKiEnergyType.EmotionalEnergy);
        }

        private NineStarKiEnergy GetSurfaceEnergy()
        {
            return ProcessEnergy(5 - (EmotionalEnergy.EnergyNumber - MainEnergy.EnergyNumber), false, ENineStarKiEnergyType.SurfaceEnergy);
        }

        private ENineStarKiEnergy GetLifeCycleYearEnergy()
        {
            var todayYearEnergy = (int)GetMainEnergy(Today).Energy;
            var personalYearEnergy = (int)MainEnergy.Energy;
            var offset = todayYearEnergy - personalYearEnergy;
            var lifeCycleYearEnergy = LoopEnergyNumber(5 + offset);

            return (ENineStarKiEnergy)lifeCycleYearEnergy;
        }

        private ENineStarKiEnergy GetLifeCycleMonthEnergy()
        {
            var yearEnergy = GetLifeCycleYearEnergy();
            return GetEmotionalEnergy(Today, yearEnergy, false).Energy;
        }

        private NineStarKiEnergy ProcessEnergy(int energyNumber, bool invertIfYin = true, ENineStarKiEnergyType type = ENineStarKiEnergyType.MainEnergy)
        {
            energyNumber = LoopEnergyNumber(energyNumber);
            if (invertIfYin && PersonModel.Gender.IsYin())
            {
                energyNumber = InvertEnergy(energyNumber);
            }

            return new NineStarKiEnergy((ENineStarKiEnergy)energyNumber, type);
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

    }
}