using K9.Globalisation;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{

    public class NineStarKiSummaryModel
    {
        private readonly NineStarKiModel _nineStarKiModel;

        public NineStarKiSummaryModel(NineStarKiModel nineStarKiModel)
        {
            _nineStarKiModel = nineStarKiModel;
            MainEnergy = new NineStarKiEnergySummary(nineStarKiModel.MainEnergy);
            CharacterEnergy = new NineStarKiEnergySummary(nineStarKiModel.CharacterEnergy);
            SurfaceEnergy = new NineStarKiEnergySummary(nineStarKiModel.SurfaceEnergy);
        }

        public PersonModel PersonModel => _nineStarKiModel.PersonModel;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MainEnergyLabel)]
        public NineStarKiEnergySummary MainEnergy { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergySummary CharacterEnergy { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SurfaceEnergyLabel)]
        public NineStarKiEnergySummary SurfaceEnergy { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SummaryLabel)]
        public string Summary => _nineStarKiModel.Summary;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.OverviewLabel)]
        public string Overview => _nineStarKiModel.Overview;
    }
}