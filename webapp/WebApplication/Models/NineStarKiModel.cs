﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Ajax.Utilities;

namespace K9.WebApplication.Models
{

    public class NineStarKiModel
    {
        public NineStarKiModel(PersonModel personModel)
        {
            PersonModel = personModel;
            Init();
        }

        public PersonModel PersonModel { get; }

        [UIHint("Energy")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.MainEnergyLabel)]
        public NineStarKiEnergy MainEnergy { get; set; }

        [UIHint("Energy")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergy CharacterEnergy { get; set; }

        [UIHint("Energy")]
        [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.RisingEnergyLabel)]
        public NineStarKiEnergy RisingEnergy { get; set; }

        private void Init()
        {
            MainEnergy = GetMainEnergy();
            CharacterEnergy = GetCharacterEnergy();
        }

        private bool IsYin(EGender gender)
        {
            return new List<EGender>
            {
                EGender.Female,
                EGender.TransFemale,
                EGender.Hermaphrodite
            }.Contains(gender);
        }

        private NineStarKiEnergy GetMainEnergy()
        {
            var month = PersonModel.DateOfBirth.Month;
            var day = PersonModel.DateOfBirth.Day;
            var year = PersonModel.DateOfBirth.Year;

            year = (month == 2 && day <= 3) || month == 1 ? year + 1 : year;
            var energyNumber = 3 - ((year - 1979) % 9);
            energyNumber = energyNumber < 1 ? (9 + energyNumber) : energyNumber;

            return ProcessEnergy(energyNumber);
        }

        private NineStarKiEnergy GetCharacterEnergy()
        {
            var energyNumber = 0;
            var month = PersonModel.DateOfBirth.Month;
            var day = PersonModel.DateOfBirth.Day;

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
            
            switch (MainEnergy.Energy)
            {
                case ENineStarEnergy.Thunder:
                case ENineStarEnergy.Heaven:
                case ENineStarEnergy.Fire:
                    energyNumber = 5;
                    break;

                case ENineStarEnergy.Water:
                case ENineStarEnergy.Wind:
                case ENineStarEnergy.Lake:
                    energyNumber = 8;
                    break;

                case ENineStarEnergy.Soil:
                case ENineStarEnergy.CoreEarth:
                case ENineStarEnergy.Mountain:
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

            return ProcessEnergy(energyNumber);
        }

        private NineStarKiEnergy ProcessEnergy(int energyNumber)
        {
            energyNumber = energyNumber < 1 ? (9 + energyNumber) : energyNumber;

            if (IsYin(PersonModel.Gender))
            {
                energyNumber = InvertEnergy(energyNumber);
            }

            return new NineStarKiEnergy((ENineStarEnergy) energyNumber);
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