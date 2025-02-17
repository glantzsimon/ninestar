using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class PredictionsViewModel
    {
        public NineStarKiModel NineStarKiModel { get; }

        public NineStarKiSummaryViewModel NineStarKiSummaryViewModel { get; }

        public PredictionsViewModel(NineStarKiModel nineStarKiModel, NineStarKiSummaryViewModel nineStarKiSummaryViewModel)
        {
            NineStarKiModel = nineStarKiModel;
            NineStarKiSummaryViewModel = nineStarKiSummaryViewModel;
        }
    }
}