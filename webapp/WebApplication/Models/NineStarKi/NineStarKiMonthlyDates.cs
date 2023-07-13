using System;

namespace K9.WebApplication.Models
{
    public class NineStarKiDates
    {
        public DateTime MonthlyPeriodStartsOn { get; set; }
        public DateTime MonthlyPeriodEndsOn { get; set; }
        public DateTime YearlyPeriodStartsOn { get; set; }
        public DateTime YearlyPeriodEndsOn { get; set; }

        public ENineStarKiEnergy YearlyEnergy { get; set; }
        public ENineStarKiEnergy MonthlyEnergy { get; set; }

        public int GetTotalDaysInMonthlyPeriod() => (int)MonthlyPeriodEndsOn.Subtract(MonthlyPeriodStartsOn).TotalDays;

        public string GetPeriodTitle() =>
            $"{MonthlyPeriodStartsOn.ToLongDateString()} - {MonthlyPeriodEndsOn.ToLongDateString()}";
    }
}