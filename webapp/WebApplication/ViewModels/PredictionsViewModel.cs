using System.Web.Script.Serialization;
using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class PredictionsViewModel
    {
        [ScriptIgnore]
        public NineStarKiModel NineStarKiModel { get; }
        
        public NineStarKiPredictionsSummaryModel PredictionsSummaryModel { get; }

        [ScriptIgnore]
        public NineStarKiSummaryViewModel NineStarKiSummaryViewModel { get; }

        public PredictionsViewModel(NineStarKiModel nineStarKiModel, NineStarKiSummaryViewModel nineStarKiSummaryViewModel)
        {
            NineStarKiModel = nineStarKiModel;
            PredictionsSummaryModel = new NineStarKiPredictionsSummaryModel(nineStarKiModel);
            NineStarKiSummaryViewModel = nineStarKiSummaryViewModel;
        }
    }
}