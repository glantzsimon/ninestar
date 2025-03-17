using K9.WebApplication.Packages;
using System;
using TimeZoneConverter;

namespace K9.WebApplication.Services
{
    public class SwissEphemerisService : BaseService, ISwissEphemerisService
    {
        public SwissEphemerisService(INineStarKiBasePackage my)
            : base(my)
        {
        }

        private const int SE_GREG_CAL = 1; // Gregorian calendar
        private const int SEFLG_SWIEPH = 2; // Swiss Ephemeris flag

        public int GetNineStarKiYear(DateTime birthDate, string timeZoneId)
        {
            using (var sweph = new SwissEphNet.SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);

                DateTime birthDateTimeUT = ConvertToUT(birthDate, timeZoneId);
                int birthYear = birthDateTimeUT.Year;

                // Calculate Julian Date for Lìchūn (立春)
                double jdLichun = GetSolarTerm(sweph, birthYear, 315.0); // 315° marks Lìchūn
                double jdBirth = GetJulianDate(sweph, birthDateTimeUT);

                // If birth date is before Lìchūn, use the previous year
                if (jdBirth < jdLichun && birthDateTimeUT.Month <= 2)
                {
                    birthYear -= 1;
                }

                return GetNineStarKiNumber(birthYear);
            }
        }

        public int GetNineStarKiMonth(DateTime birthDate, string timeZoneId)
        {
            using (var sweph = new SwissEphNet.SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);

                DateTime birthDateTimeUT = ConvertToUT(birthDate, timeZoneId);
                int birthYear = birthDateTimeUT.Year;

                // Calculate Julian Date for Lìchūn (立春)
                double jdLichun = GetSolarTerm(sweph, birthYear, 315.0); // 315° marks Lìchūn
                double jdBirth = GetJulianDate(sweph, birthDateTimeUT);

                // If birth date is before Lìchūn, use the previous year
                if (jdBirth < jdLichun && birthDateTimeUT.Month <= 2)
                {
                    birthYear -= 1;
                }

                // Get the Nine Star Ki year energy (which has already considered Lìchūn)
                int yearEnergy = GetNineStarKiYear(birthDate, timeZoneId);

                // **Determine First Month Based on Year Energy**
                int firstMonth = GetFirstMonthForYearEnergy(yearEnergy);

                // **Get Solar Terms for the 12 months after Lìchūn**
                double[] solarTerms = GetSolarTerms(sweph, birthYear);

                // **Find the correct month by comparing birthdate with solar terms**
                for (int i = 0; i < 11; i++) // Avoid out-of-bounds
                {
                    if (jdBirth >= solarTerms[i] && jdBirth < solarTerms[i + 1])
                    {
                        return ((firstMonth - 1 - i + 9) % 9) + 1; // **Nine Star Ki downward cycle**
                    }
                }

                // **If birth date is after last solar term, assign last month**
                return ((firstMonth - 1 - 8 + 9) % 9) + 1;
            }
        }

        private double GetSolarTerm(SwissEphNet.SwissEph sweph, int year, double targetLongitude)
        {
            double julianStart = sweph.swe_julday(year, 1, 1, 0.0, SE_GREG_CAL);
            double julianEnd = sweph.swe_julday(year, 12, 31, 23.999, SE_GREG_CAL);

            double tolerance = 0.01;  // Precision tolerance
            int maxIterations = 100;  // Maximum number of iterations
            double mid, sunLongitude;
            int iteration = 0;
            string serr = "";

            while (iteration < maxIterations)
            {
                iteration++;
                mid = (julianStart + julianEnd) / 2;
                double[] xx = new double[6];

                int ret = sweph.swe_calc(mid, SwissEphNet.SwissEph.SE_SUN, SEFLG_SWIEPH, xx, ref serr);
                if (ret < 0)
                {
                    throw new Exception($"Error calculating Sun position: {serr}");
                }

                sunLongitude = xx[0]; // Sun's longitude in degrees

                // Handle the longitude wrap-around correctly
                if (sunLongitude < targetLongitude && (targetLongitude - sunLongitude > 180))
                {
                    julianEnd = mid; // Move search range backward
                }
                else if (sunLongitude > targetLongitude && (sunLongitude - targetLongitude > 180))
                {
                    julianStart = mid; // Move search range forward
                }
                else
                {
                    // Regular binary search
                    if (sunLongitude < targetLongitude)
                        julianStart = mid;
                    else
                        julianEnd = mid;
                }

                // Check if we have converged within the tolerance
                if (Math.Abs(sunLongitude - targetLongitude) < tolerance)
                {
                    return mid;
                }
            }

            throw new Exception($"Could not find solar term at {targetLongitude}° for year {year} after {maxIterations} iterations.");
        }

        private double GetJulianDate(SwissEphNet.SwissEph sweph, DateTime birthDateTimeUT)
        {
            return sweph.swe_julday(
                birthDateTimeUT.Year, birthDateTimeUT.Month, birthDateTimeUT.Day,
                birthDateTimeUT.Hour + birthDateTimeUT.Minute / 60.0, SE_GREG_CAL);
        }

        private int GetNineStarKiNumber(int year)
        {
            int kiNumber = (11 - (year % 9)) % 9;
            return kiNumber == 0 ? 9 : kiNumber;
        }

        private DateTime ConvertToUT(DateTime localTime, string timeZoneId)
        {
            TimeZoneInfo tz = TZConvert.GetTimeZoneInfo(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(localTime, tz);
        }

        private int GetFirstMonthForYearEnergy(int yearEnergy)
        {
            if (yearEnergy == 1 || yearEnergy == 4 || yearEnergy == 7)
                return 8;
            else if (yearEnergy == 2 || yearEnergy == 5 || yearEnergy == 8)
                return 2;
            else
                return 5;
        }

        private double[] GetSolarTerms(SwissEphNet.SwissEph sweph, int birthYear)
        {
            double[] solarTerms = new double[12];
            for (int i = 0; i < 12; i++)
            {
                var targetLongitude = 315.0 + (i * 30.0);
                double normalizedTargetLongitude = targetLongitude % 360;
                solarTerms[i] = GetSolarTerm(sweph, birthYear, normalizedTargetLongitude);
            }
            return solarTerms;
        }
    }
}
