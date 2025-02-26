using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using System;

namespace K9.WebApplication.Models
{
    public class ChakraCodePlannerModel
    {
        public EChakraCode ChakraCode { get; set; }

        public int ChakraCodeNumber => (int)ChakraCode;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Offset { get; set; }

        public DateTime Month => GetMonth();

        public bool IsCurrent => DateTime.Now.IsBetween(StartDate, EndDate) || DateTime.Today == StartDate && DateTime.Today == EndDate;

        public string Title => $"{StartDate.Year - EndDate.Year}";

        public string IsCurrentCssClass => IsCurrent ? "current" : "";

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