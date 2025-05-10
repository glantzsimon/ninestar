using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using System;

namespace K9.WebApplication.Models
{
    public class NumerologyPlannerModel
    {
        public ENumerologyCode NumerologyCode { get; set; }

        public int CodeNumber => (int)NumerologyCode;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Offset { get; set; }

        public DateTime Month => GetMonth();

        public bool IsCurrent => DateTime.Now.IsBetween(StartDate, EndDate) || DateTime.Today == StartDate && DateTime.Today == EndDate;

        public string DatesString =>
            StartDate.Year == EndDate.Year
                ? $"{StartDate.ToString("dd MMM")} - {EndDate.ToString("dd MMM yyyy")}"
                : $"{StartDate.ToString("dd MMM yy")} - {EndDate.ToString("dd MMM yy")}";

        public string Title => $"{StartDate.Year - EndDate.Year}";

        public string IsCurrentCssClass => IsCurrent ? "current" : "";

        public string IsSelectedCssClass => IsCurrent ? "selected" : "";

        private DateTime GetMonth()
        {
            var startDateDayCount = DateTime.DaysInMonth(StartDate.Year, StartDate.Month) - (StartDate.Day - 1);
            var endDateDayCount = EndDate.Day;
            var nextMonth = StartDate.AddMonths(1);
            var isMiddleMonth = EndDate.Month > nextMonth.Month;

            return isMiddleMonth ? nextMonth : startDateDayCount > endDateDayCount ? StartDate : EndDate;
        }
    }
}