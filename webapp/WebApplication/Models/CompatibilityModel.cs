using System;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;

namespace K9.WebApplication.Models
{

    public class CompatibilityModel
    {
        public CompatibilityModel()
        {
            NineStarKiModel1 = new NineStarKiModel();
            NineStarKiModel2 = new NineStarKiModel();
        }

        public CompatibilityModel(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2)
        {
            NineStarKiModel1 = nineStarKiModel1;
            NineStarKiModel2 = nineStarKiModel2;
            MainElementChemistryLevel = GetMainElementChemistryLevel();
            MainElementLearningPotential = GetMainElementLearningPotential();
        }

        public NineStarKiModel NineStarKiModel1 { get; }
        public NineStarKiModel NineStarKiModel2 { get; }

        public ECompatibilityLevel MainElementChemistryLevel { get; }
        public ECompatibilityLevel MainElementLearningPotential { get; }

        private ECompatibilityLevel GetMainElementChemistryLevel()
        {
            return GetChemistryLevel(NineStarKiModel1.MainEnergy, NineStarKiModel2.MainEnergy);
        }

        private ECompatibilityLevel GetChemistryLevel(NineStarKiEnergy energy1, NineStarKiEnergy energy2)
        {
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);

            switch (transformationType)
            {
                case ETransformationType.Challenges:
                case ETransformationType.IsChallenged:
                    return ProcessGenderAndModality(ECompatibilityLevel.High, energy1, energy2, true);

                case ETransformationType.IsSupported:
                case ETransformationType.Supports:
                    return ProcessGenderAndModality(ECompatibilityLevel.Medium, energy1, energy2, true);

                case ETransformationType.Same:
                    return ProcessGenderAndModality(ECompatibilityLevel.Low, energy1, energy2, true);
            }

            return ECompatibilityLevel.Unspecified;
        }

        private ECompatibilityLevel GetMainElementLearningPotential()
        {
            return GetElementLearningPotential(NineStarKiModel1.MainEnergy, NineStarKiModel2.MainEnergy);
        }

        private ECompatibilityLevel GetElementLearningPotential(NineStarKiEnergy energy1, NineStarKiEnergy energy2)
        {
            var transformationType = NineStarKiModel1.MainEnergy.Energy.GetTransformationType(NineStarKiModel2.MainEnergy.Energy);

            switch (transformationType)
            {
                case ETransformationType.IsChallenged:
                case ETransformationType.Challenges:
                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                    return ProcessGenderAndModality(ECompatibilityLevel.High, energy1, energy2, true);

                case ETransformationType.Same:
                    return ProcessGenderAndModality(ECompatibilityLevel.Low, energy1, energy2, true);
            }

            return ECompatibilityLevel.Unspecified;
        }

        private ECompatibilityLevel ProcessGenderAndModality(ECompatibilityLevel value, NineStarKiEnergy energy1, NineStarKiEnergy energy2, bool invertCalculation = false)
        {
            var isSameGender = energy1.YinYang == energy2.YinYang;
            var isSameModality = energy1.Modality == energy2.Modality;
            var score = (isSameGender ? 1 : 0) + (isSameModality ? 1 : 0);
            return value + score;
        }
    }
}