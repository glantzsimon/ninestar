using System;

namespace K9.WebApplication.Services
{
    public interface ISwissEphemerisService
    {
        int GetNineStarKiYear(DateTime birthDate, string timeZoneId);
        int GetNineStarKiMonth(DateTime birthDate, string timeZoneId);
        /// <summary>
        /// Returns the Gregorian calendar month number, from 1 - 12
        /// </summary>
        /// <param name="birthDate"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        int GetNineStarKiMonthNumber(DateTime birthDate, string timeZoneId);

        (int ki, int? invertedKi) GetNineStarKiDailyKi(DateTime currentDate, string timeZoneId, bool isDebug = false);
    }
}