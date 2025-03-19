using K9.WebApplication.Packages;
using SwissEphNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TimeZoneConverter;

namespace K9.WebApplication.Services
{
    public class SwissEphemerisService : BaseService, ISwissEphemerisService
    {
        private const int SE_GREG_CAL = 1;              // Gregorian calendar flag
        private const int SEFLG_SWIEPH = 2;             // Swiss Ephemeris flag for calculations
        private const double PRECISE_YEAR_LENGTH = 365.242190419;
        private const string BASE_DAY_TIMEZONE = "Europe/London";
        private const int BASE_DAY_KI_CYCLE_START = 71;
        private const int BASE_DAY_KI_SMALL_CYCLE_START = 12;
        private const int BASE_DAY_KI_SMALL_CYCLE = 60;
        private const int BASE_DAY_KI_CYCLE = 240;
        private const int BASE_DAY_KI = 2;
        private const int BASE_HOUR_KI = 5;
        private static DateTime BASE_KI_DATEUT;

        public SwissEphemerisService(INineStarKiBasePackage my)
            : base(my)
        {
            my.DefaultValuesConfiguration.ValidateSwephPath();
            BASE_KI_DATEUT = ConvertToUT(new DateTime(1900, 1, 1, 1, 0, 0), BASE_DAY_TIMEZONE);
        }

        public int GetNineStarKiYear(DateTime birthDate, string timeZoneId)
        {
            using (var sweph = new SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                DateTime birthDateTimeUT = ConvertToUT(birthDate, timeZoneId);
                int adjustedYear = AdjustBirthYear(sweph, birthDateTimeUT);
                return GetNineStarKiNumber(adjustedYear);
            }
        }

        public int GetNineStarKiMonth(DateTime birthDate, string timeZoneId)
        {
            using (var sweph = new SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                DateTime birthDateTimeUT = ConvertToUT(birthDate, timeZoneId);
                int adjustedYear = AdjustBirthYear(sweph, birthDateTimeUT);
                // Use the adjusted year to compute the energy directly instead of calling GetNineStarKiYear again.
                int yearEnergy = GetNineStarKiNumber(adjustedYear);
                int firstMonth = GetFirstMonthForYearEnergy(yearEnergy);
                double jdBirth = GetJulianDate(sweph, birthDateTimeUT);
                double[] solarTerms = GetSolarTerms(sweph, adjustedYear);

                for (int i = 0; i < 11; i++)
                {
                    if (jdBirth >= solarTerms[i] && jdBirth < solarTerms[i + 1])
                    {
                        // Downward cycle for Nine Star Ki month
                        return ((firstMonth - 1 - i + 9) % 9) + 1;
                    }
                }
                return ((firstMonth - 1 - 8 + 9) % 9) + 1;
            }
        }

        public int GetNineStarKiMonthNumber(DateTime birthDate, string timeZoneId)
        {
            using (var sweph = new SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                DateTime birthDateTimeUT = ConvertToUT(birthDate, timeZoneId);
                int adjustedYear = AdjustBirthYear(sweph, birthDateTimeUT);
                double jdBirth = GetJulianDate(sweph, birthDateTimeUT);
                double[] solarTerms = GetSolarTerms(sweph, adjustedYear);

                for (int i = 0; i < 11; i++)
                {
                    if (jdBirth >= solarTerms[i] && jdBirth < solarTerms[i + 1])
                    {
                        return i + 2;
                    }
                }
                return 12;
            }
        }

        public (int ki, int? invertedKi) GetNineStarKiDailyKi(DateTime selectedDateTime, string timeZoneId)
        {
            using (var sweph = new SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);

                int dayKi = BASE_DAY_KI;
                int? invertedKi = null;
                DateTime selectedDateTimeUT = ConvertToUT(selectedDateTime, timeZoneId);
                int dayKiCount = BASE_DAY_KI_CYCLE_START;
                int dayKiCycleLength = BASE_DAY_KI_CYCLE;
                int dayKiSmallCycle = BASE_DAY_KI_SMALL_CYCLE_START;

                // Loop through each year from the base Ki date up to the selected year.
                for (int year = BASE_KI_DATEUT.Year; year <= selectedDateTimeUT.Year; year++)
                {
                    DateTime startOfYear = new DateTime(year, 1, 1);
                    DateTime finishOfYear = (year == selectedDateTimeUT.Year) ? selectedDateTimeUT.Date : new DateTime(year, 12, 31);
                    // Start on the day after January 1, as the first day already has the correct ki.
                    DateTime day = startOfYear.AddDays(1);

                    // Compute solstice dates for the current year.
                    DateTime juneSolstice = FindJuneSolstice(sweph, year);
                    DateTime decemberSolstice = FindDecemberSolstice(sweph, year);
                    
                    while (day <= finishOfYear)
                    {
                        invertedKi = null;
                        
                        if (dayKiCount == dayKiCycleLength)
                        {
                            // Every BASE_DAY_KI_CYCLE days, advance dayKi by 3.
                            dayKi = IncrementKi(dayKi, 3);
                            dayKiCount = 1;

                            switch (dayKiCycleLength)
                            {
                                case BASE_DAY_KI_CYCLE:
                                    dayKiCycleLength += 60;
                                    break;

                                case BASE_DAY_KI_CYCLE + 60:
                                    dayKiCycleLength += 120;
                                    break;

                                case BASE_DAY_KI_CYCLE + 120:
                                    dayKiCycleLength += 180;
                                    break;
                            }

                            if (dayKiCycleLength == BASE_DAY_KI_CYCLE + 180)
                            {
                                dayKiCycleLength = BASE_DAY_KI_CYCLE;
                            }
                        }

                        if (day < juneSolstice)
                        {
                            if (dayKiCount == 6 && dayKiSmallCycle == 5 && dayKi == 5)
                            {
                                dayKi = 3;
                            }
                            else
                            {
                                dayKi = IncrementKi(dayKi);
                            }
                        }
                        else if (day == juneSolstice)
                        {
                            dayKi = IncrementKi(dayKi);
                            invertedKi = dayKi;
                            dayKi = InvertEnergy(dayKi);
                        }
                        else if (day > juneSolstice && day < decemberSolstice)
                        {
                            dayKi = DecrementKi(dayKi);
                        }
                        else if (day == decemberSolstice)
                        {
                            dayKi = DecrementKi(dayKi);
                            invertedKi = dayKi;
                            dayKi = InvertEnergy(dayKi);
                        }
                        else // day > decemberSolstice
                        {
                            dayKi = IncrementKi(dayKi);
                        }

                        // Reste 60 day cycle
                        if (dayKiSmallCycle == BASE_DAY_KI_SMALL_CYCLE)
                        {
                            dayKiSmallCycle = 0;
                        }

                        day = day.AddDays(1);
                        dayKiCount++;
                        dayKiSmallCycle++;
                    }
                }

                return (dayKi, invertedKi);
            }
        }

        private DateTime FindSolstice(SwissEph sweph, int year, bool isJuneSoltice)
        {
            double targetLongitude = isJuneSoltice ? 90.0 : 270.0;
            DateTime approxDate = isJuneSoltice ? new DateTime(year, 6, 21) : new DateTime(year, 12, 21);
            double jd = GetJulianDate(sweph, approxDate);
            double epsilon = 1e-6;
            double delta = 1e-5;
            int maxIter = 100;
            string serr = "";
            double[] x = new double[6], xPlus = new double[6], xMinus = new double[6];

            for (int iter = 0; iter < maxIter; iter++)
            {
                int ret = sweph.swe_calc_ut(jd, SwissEph.SE_SUN, SwissEph.SEFLG_SPEED, x, ref serr);
                if (ret < 0)
                    throw new Exception("Error computing Sun position: " + serr);

                double longitude = x[0];
                double diff = ((longitude - targetLongitude + 180) % 360) - 180;
                if (Math.Abs(diff) < epsilon)
                    break;

                sweph.swe_calc_ut(jd + delta, SwissEph.SE_SUN, SwissEph.SEFLG_SPEED, xPlus, ref serr);
                sweph.swe_calc_ut(jd - delta, SwissEph.SE_SUN, SwissEph.SEFLG_SPEED, xMinus, ref serr);
                double fPlus = ((xPlus[0] - targetLongitude + 180) % 360) - 180;
                double fMinus = ((xMinus[0] - targetLongitude + 180) % 360) - 180;
                double derivative = (fPlus - fMinus) / (2 * delta);
                jd -= diff / derivative;
            }

            return JulianDayToDateTime(sweph, jd);
        }

        private DateTime FindJuneSolstice(SwissEph sweph, int year) => FindSolstice(sweph, year, true);

        private DateTime FindDecemberSolstice(SwissEph sweph, int year) => FindSolstice(sweph, year, false);

        private DateTime JulianDayToDateTime(SwissEph sweph, double julianDay)
        {
            int gregflag = 1;
            int year = 0, month = 0, day = 0, hour = 0, minute = 0;
            double second = 0.0;
            sweph.swe_jdet_to_utc(julianDay, gregflag, ref year, ref month, ref day, ref hour, ref minute, ref second);
            int wholeSeconds = (int)second;
            double fraction = second - wholeSeconds;
            return new DateTime(year, month, day, hour, minute, wholeSeconds, DateTimeKind.Utc)
                .AddMilliseconds(fraction * 1000);
        }

        private double GetSolarTerm(SwissEph sweph, int year, double targetLongitude)
        {
            double julianStart = sweph.swe_julday(year, 1, 1, 0.0, SE_GREG_CAL);
            double julianEnd = sweph.swe_julday(year, 12, 31, 23.999, SE_GREG_CAL);
            double tolerance = 0.01;
            int maxIterations = 100;
            int iteration = 0;
            string serr = "";
            double mid, sunLongitude;

            while (iteration < maxIterations)
            {
                iteration++;
                mid = (julianStart + julianEnd) / 2;
                double[] xx = new double[6];
                int ret = sweph.swe_calc(mid, SwissEph.SE_SUN, SEFLG_SWIEPH, xx, ref serr);
                if (ret < 0)
                    throw new Exception($"Error calculating Sun position: {serr}");
                sunLongitude = xx[0];

                // Handle wrap-around correctly
                if (sunLongitude < targetLongitude && (targetLongitude - sunLongitude > 180))
                    julianEnd = mid;
                else if (sunLongitude > targetLongitude && (sunLongitude - targetLongitude > 180))
                    julianStart = mid;
                else
                    if (sunLongitude < targetLongitude)
                    julianStart = mid;
                else
                    julianEnd = mid;

                if (Math.Abs(sunLongitude - targetLongitude) < tolerance)
                    return mid;
            }

            throw new Exception($"Could not find solar term at {targetLongitude}° for year {year} after {maxIterations} iterations.");
        }

        private double GetJulianDate(SwissEph sweph, DateTime dateTimeUT)
        {
            return sweph.swe_julday(
                dateTimeUT.Year, dateTimeUT.Month, dateTimeUT.Day,
                dateTimeUT.Hour + dateTimeUT.Minute / 60.0, SE_GREG_CAL);
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

        private double[] GetSolarTerms(SwissEph sweph, int year)
        {
            double[] solarTerms = new double[12];
            for (int i = 0; i < 12; i++)
            {
                double targetLongitude = (315.0 + (i * 30.0)) % 360;
                solarTerms[i] = GetSolarTerm(sweph, year, targetLongitude);
            }
            return solarTerms;
        }

        /// <summary>
        /// Adjusts the birth year based on Lìchūn (315° solar term) so that if the birth
        /// date is before Lìchūn in January–February, the previous year is used.
        /// </summary>
        private int AdjustBirthYear(SwissEph sweph, DateTime birthDateTimeUT)
        {
            int year = birthDateTimeUT.Year;
            double jdLichun = GetSolarTerm(sweph, year, 315.0);
            double jdBirth = GetJulianDate(sweph, birthDateTimeUT);
            return (jdBirth < jdLichun && birthDateTimeUT.Month <= 2) ? year - 1 : year;
        }

        /// <summary>
        /// Increments a ki number (1–9) by a given amount.
        /// </summary>
        private int IncrementKi(int ki, int amount = 1)
        {
            return (((ki - 1) + amount) % 9) + 1;
        }

        /// <summary>
        /// Decrements a ki number (1–9) by a given amount.
        /// </summary>
        private int DecrementKi(int ki, int amount = 1)
        {
            return ((((ki - 1) - amount) % 9 + 9) % 9) + 1;
        }

        private int InvertEnergy(int energyNumber) =>
            _invertedEnergies.TryGetValue(energyNumber, out var inverted) ? inverted : energyNumber;

        private static readonly Dictionary<int, int> _invertedEnergies = new Dictionary<int, int>
        {
            { 1, 5 }, { 2, 4 }, { 4, 2 }, { 5, 1 },
            { 6, 9 }, { 7, 8 }, { 8, 7 }, { 9, 6 }
        };
    }
}
