using System;

namespace K9.WebApplication.Services
{
    public interface ISwissEphemerisService
    {
        int GetNineStarKiYear(DateTime birthDate, string timeZoneId);
        int GetNineStarKiMonth(DateTime birthDate, string timeZoneId);
        int GetNineStarKiMonthNumber(DateTime birthDate, string timeZoneId);
    }
}