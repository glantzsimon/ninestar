using System;

namespace K9.WebApplication.Models
{
    public class CalendarEntry
    {
        public DateTime Date { get; set; }
        public int YearHouse { get; set; }
        public int MonthHouse { get; set; }
        public int DayHouse { get; set; }
        public int? AfternoonDayHouse { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
    }
}
