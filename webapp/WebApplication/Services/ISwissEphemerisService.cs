using System;

namespace K9.WebApplication.Services
{
    public interface ISwissEphemerisService
    {
        int GetNineStarKiEightyOneYearKi(DateTime selectedDateTime, string timeZoneId);
        int GetNineStarKiNineYearKi(DateTime selectedDateTime, string timeZoneId);
        int GetNineStarKiYearlyKi(DateTime selectedDateTime, string timeZoneId);
        int GetNineStarKiMonthlyKi(DateTime selectedDateTime, string timeZoneId);
        /// <summary>
        /// Returns the Gregorian calendar month number, from 1 - 12
        /// </summary>
        /// <param name="selectedDateTime"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        int GetNineStarKiMonthNumber(DateTime selectedDateTime, string timeZoneId);

        (int ki, int? invertedKi) GetNineStarKiDailyKi(DateTime selectedDateTime, string timeZoneId);
        int GetNineStarKiHourlyKi(DateTime selectedDateTime, string timeZoneId);
    }
}