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

        public EElementChemistryLevel MainElementChemistryLevel { get; }
        public EElementLearningPotential MainElementLearningPotential { get; }

        private EElementChemistryLevel GetMainElementChemistryLevel()
        {
            return GetChemistryLevel(NineStarKiModel1.MainEnergy, NineStarKiModel2.MainEnergy);
        }

        private EElementChemistryLevel GetChemistryLevel(NineStarKiEnergy energy1, NineStarKiEnergy energy2)
        {
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);
            var isSameGender = energy1.YinYang == energy2.YinYang;
            var isSameModality = energy1.Modality == energy2.Modality;

            switch (transformationType)
            {
                case ETransformationType.Challenges:
                case ETransformationType.IsChallenged:
                    if (isSameGender && isSameModality)
                    {
                        return EElementChemistryLevel.Extreme;
                    }
                    else if (isSameGender)
                    {
                        return EElementChemistryLevel.VeryHigh;
                    }
                    else if (isSameModality)
                    {
                        return EElementChemistryLevel.VeryHigh;
                    }
                    else
                    {
                        return EElementChemistryLevel.VeryHigh;
                    }

                case ETransformationType.IsSupported:
                case ETransformationType.Supports:
                    return isSameGender ? EElementChemistryLevel.Medium : EElementChemistryLevel.MediumToHigh;

                case ETransformationType.Same:
                    return isSameGender ? EElementChemistryLevel.VeryLow : EElementChemistryLevel.Low;
            }

            return EElementChemistryLevel.Unspecified;
        }

        private EElementLearningPotential GetMainElementLearningPotential()
        {
            var transformationType = NineStarKiModel1.MainEnergy.Energy.GetTransformationType(NineStarKiModel2.MainEnergy.Energy);
            var isSameGender = NineStarKiModel1.MainEnergy.YinYang == NineStarKiModel2.MainEnergy.YinYang;

            switch (transformationType)
            {
                case ETransformationType.IsChallenged:
                    return isSameGender ? EElementLearningPotential.High : EElementLearningPotential.VeryHigh;

                case ETransformationType.Challenges:
                    return isSameGender ? EElementLearningPotential.MediumToHigh : EElementLearningPotential.High;

                case ETransformationType.Supports:
                    return isSameGender ? EElementLearningPotential.Medium : EElementLearningPotential.MediumToHigh;

                case ETransformationType.IsSupported:
                    return isSameGender ? EElementLearningPotential.Low : EElementLearningPotential.Medium;

                case ETransformationType.Same:
                    return isSameGender ? EElementLearningPotential.VeryLow : EElementLearningPotential.Low;
            }

            return EElementLearningPotential.Unspecified;
        }
    }
}