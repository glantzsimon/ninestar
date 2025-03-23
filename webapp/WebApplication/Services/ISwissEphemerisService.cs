using System;

namespace K9.WebApplication.Services
{
    public interface ISwissEphemerisService
    {
        int GetNineStarKiEightyOneYearKi(DateTime selectedDateTime, string timeZoneId);
        int GetNineStarKiNineYearKi(DateTime selectedDateTime, string timeZoneId);
        int GetNineStarKiYearlyKi(DateTime selectedDateTime, string timeZoneId);
        int GetNineStarKiMonthlyKi(DateTime selectedDateTime, string timeZoneId, bool invert = false);
        /// <summary>
        /// Returns the Gregorian calendar month number, from 1 - 12
        /// </summary>
        /// <param name="selectedDateTime"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        int GetNineStarKiMonthNumber(DateTime selectedDateTime, string timeZoneId);

        (int DailyKi, int? InvertedDailyKi) GetNineStarKiDailyKi(DateTime selectedDateTime, string timeZoneId);
        int GetNineStarKiHourlyKi(DateTime selectedDateTime, string timeZoneId);

        (DateTime PeriodStartOn, DateTime PeriodEndsOn) GetNineStarKiMonthlyPeriodBoundaries(DateTime selectedDateTime,
            string timeZoneId);

        (DateTime PeriodStartOn, DateTime PeriodEndsOn, int MonthlyKi)[] GetNineStarKiMonthlyPeriods(
            DateTime selectedDateTime, string timeZoneId);

        (DateTime YearStart, DateTime YearEnd, int YearlyKi)[] GetNineStarKiYearlyPeriods(DateTime selectedDateTime,
            string timeZoneId);
    }
}