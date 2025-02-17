using K9.Globalisation;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{

    public class NineStarKiPredictionsSummaryModel
    {
        private readonly NineStarKiModel _nineStarKiModel;

        public NineStarKiPredictionsSummaryModel(NineStarKiModel nineStarKiModel)
        {
            _nineStarKiModel = nineStarKiModel;
            YearlyPrediction = nineStarKiModel.YearlyCycleEnergy;
            MonthlyPrediction = nineStarKiModel.MonthlyCycleEnergy;
        }
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MainEnergyLabel)]
        public NineStarKiEnergy YearlyPrediction { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergy MonthlyPrediction { get; set; }
    }
}