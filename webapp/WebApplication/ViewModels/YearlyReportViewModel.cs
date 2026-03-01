using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class YearlyReportViewModel
    {
        public NineStarKiModel NineStarKiModel { get; set; }
        public PlannerViewModel YearlyPlannerModel { get; set; }

        public string PeriodDateString => $"{YearlyPlannerModel.PeriodStartsOn.ToShortDateString()} - {YearlyPlannerModel.PeriodEndsOn.ToShortDateString()}";
    }
}