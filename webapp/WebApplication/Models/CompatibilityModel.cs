using K9.WebApplication.Enums;

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
            ElementChemistryLevel = GetChemistryLevel();
        }

        public NineStarKiModel NineStarKiModel1 { get; }
        public NineStarKiModel NineStarKiModel2 { get; }

        public EElementChemistryLevel ElementChemistryLevel { get; }

        private EElementChemistryLevel GetChemistryLevel()
        {
            return EElementChemistryLevel.High;
        }
    }
}