using System.Web.Script.Serialization;
using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class PredictionsViewModel
    {
        [ScriptIgnore]
        public NineStarKiModel NineStarKiModel { get; }
        
        public NineStarKiSummaryModel PersonalChartModel { get; }

        public NineStarKiPredictionsSummaryModel PredictionsSummaryModel { get; }

        [ScriptIgnore]
        public NineStarKiSummaryViewModel NineStarKiSummaryViewModel { get; }

        public PredictionsViewModel(NineStarKiModel nineStarKiModel, NineStarKiSummaryViewModel nineStarKiSummaryViewModel)
        {
            NineStarKiModel = nineStarKiModel;
            PersonalChartModel = new NineStarKiSummaryModel(nineStarKiModel);
            PredictionsSummaryModel = new NineStarKiPredictionsSummaryModel(nineStarKiModel);
            NineStarKiSummaryViewModel = nineStarKiSummaryViewModel;
        }
    }
}