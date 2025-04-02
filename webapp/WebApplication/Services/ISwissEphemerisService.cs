using System;
using K9.WebApplication.Models;

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

        (int DailyKi, int? InvertedDailyKi)[] GetNineStarKiDailyKis(DateTime selectedDateTime, string timeZoneId);

        (DateTime PeriodStartsOn, DateTime PeriodEndsOn, int YearlyKi) GetNineStarKiYearlyPeriod(DateTime selectedDateTime,
            string timeZoneId);

        (DateTime PeriodStartsOn, DateTime PeriodEndsOn) GetNineStarKiMonthlyPeriodBoundaries(DateTime selectedDateTime,
            string timeZoneId);

        (DateTime PeriodStartsOn, DateTime PeriodEndsOn, int MonthlyKi)[] GetNineStarKiMonthlyPeriods(
            DateTime selectedDateTime, string timeZoneId);

        (DateTime PeriodStartsOn, DateTime PeriodEndsOn, int YearlyKi)[] GetNineStarKiYearlyPeriods(DateTime selectedDateTime,
            string timeZoneId);

        (DateTime PeriodStartsOn, DateTime PeriodEndsOn, int NineYearKi) GetNineStarKiNineYearPeriod(
            DateTime selectedDateTime, string timeZoneId);

        (DateTime PeriodStartsOn, DateTime PeriodEndsOn, int NineYearKi)[]
            GetNineStarKiNineYearPeriodsWithinEightyOneYearPeriod(DateTime selectedDateTime, string timeZoneId);

        (DateTime PeriodStartsOn, DateTime PeriodEndsOn, int EightyOneYearKi) GetNineStarKiEightyOneYearPeriod(
            DateTime selectedDateTime, string timeZoneId);

        (DateTime PeriodStartsOn, DateTime PeriodEndsOn, int YearlyKi)[] GetNineStarKiYearlyPeriodsForNineYearPeriod(
            DateTime selectedDateTime, string timeZoneId);

        (DateTime Day, int MorningKi, int? InvertedMorningKi, int? AfternoonKi, int? InvertedAfternoonKi)[]
            GetNineStarKiDailyEnergiesForMonth(DateTime selectedDateTime, string timeZoneId);

        (DateTime SegmentStartsOn, DateTime SegmentEndsOn, int HourlyKi)[] GetNineStarKiHourlyPeriodsForDay(
            DateTime selectedDateTime, string timeZoneId);

        DateTime GetLichun(DateTime selectedDateTime, string timeZoneId);

        MoonPhase GetMoonIlluminationPercentage(DateTime selectedDateTime,
            string timeZoneId);
    }
}