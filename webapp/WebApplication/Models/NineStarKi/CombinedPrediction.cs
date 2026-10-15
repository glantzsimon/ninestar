using System;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Models
{
    // Immutable period context: safe to share when copying a cached planner.
    public sealed class CombinedPrediction
    {
        public CombinedPrediction(int year, int month, int? day = null, int? afternoonDay = null, bool isGlobal = false)
        {
            if (year < 1 || year > 9 || month < 1 || month > 9 ||
                (day.HasValue && (day < 1 || day > 9)) ||
                (afternoonDay.HasValue && (!day.HasValue || afternoonDay < 1 || afternoonDay > 9)))
                throw new ArgumentOutOfRangeException(nameof(year), "Combined energies must be between one and nine.");
            IsGlobal = isGlobal;
            Year = year;
            Month = month;
            Day = day;
            AfternoonDay = afternoonDay != day ? afternoonDay : null;
        }

        public bool IsGlobal { get; }
        public int Year { get; }
        public int Month { get; }
        public int? Day { get; }
        public int? AfternoonDay { get; }
        public string Html => CombinedPredictionHelper.GetHtml(Year, Month, Day, AfternoonDay, IsGlobal);
    }
}
