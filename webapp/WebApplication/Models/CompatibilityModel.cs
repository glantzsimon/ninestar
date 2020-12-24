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
        }

        public NineStarKiModel NineStarKiModel1 { get; }
        public NineStarKiModel NineStarKiModel2 { get; }

        public EElementChemistryLevel MainElementChemistryLevel { get; }

        private EElementChemistryLevel GetMainElementChemistryLevel()
        {
            var transformationType = NineStarKiModel1.MainEnergy.Energy.GetTransformationType(NineStarKiModel2.MainEnergy.Energy);
            var isSameGender = NineStarKiModel1.MainEnergy.YinYang == NineStarKiModel2.MainEnergy.YinYang;

            switch (transformationType)
            {
                case ETransformationType.Challenges:
                case ETransformationType.IsChallenged:
                    return isSameGender ? EElementChemistryLevel.High : EElementChemistryLevel.VeryHigh;

                case ETransformationType.IsSupported:
                case ETransformationType.Supports:
                    return isSameGender ? EElementChemistryLevel.Medium : EElementChemistryLevel.MediumToHigh;

                case ETransformationType.Same:
                    return isSameGender ? EElementChemistryLevel.VeryLow : EElementChemistryLevel.Low;
            }

            return EElementChemistryLevel.Unspecified;
        }
    }
}