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
            NineYearlyPrediction = nineStarKiModel.PersonalHousesOccupiedEnergies?.Generation;
            YearlyPrediction = nineStarKiModel.PersonalHousesOccupiedEnergies?.Year;
            MonthlyPrediction = nineStarKiModel.PersonalHousesOccupiedEnergies?.Month;
            DailyPrediction = nineStarKiModel.PersonalHousesOccupiedEnergies?.Day;
            LunarDayDescription = nineStarKiModel.MoonPhase.LunarDayDescription;
        }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.GenerationalCycleEnergy)]
        public NineStarKiEnergy NineYearlyPrediction { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.YearlyCycleEnergy)]
        public NineStarKiEnergy YearlyPrediction { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.MonthlyCycleEnergy)]
        public NineStarKiEnergy MonthlyPrediction { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.DailyCycleEnergy)]
        public NineStarKiEnergy DailyPrediction { get; set; }

        public string LunarDayDescription { get; set; }
    }
}