using K9.WebApplication.Packages;
using SwissEphNet;
using System;
using System.Diagnostics;
using TimeZoneConverter;
using Xunit.Abstractions;

namespace K9.WebApplication.Services
{
    public class SwissEphemerisService : BaseService, ISwissEphemerisService
    {
        private readonly ITestOutputHelper _output;
        private const int SE_GREG_CAL = 1;              // Gregorian calendar flag
        private const int SEFLG_SWIEPH = 2;             // Swiss Ephemeris flag for calculations
        private const double PRECISE_YEAR_LENGTH = 365.242190419;
        private const string BASE_DAY_TIMEZONE = "Europe/London";

        private const int SEXAGENARY_CYCLE_START_DAY = 12;
        private const int SEXAGENARY_CYCLE_LENGTH = 60;
        private const int SEXAGENARY_JUNE_JIA_ZI_DAY_KI = 9;
        private const int SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI = 1;

        private const int BASE_DAY_KI = 2;
        private const int BASE_HOUR_KI = 5;
        private static DateTime BASE_KI_DATEUT;
        
        public SwissEphemerisService(INineStarKiBasePackage my, ITestOutputHelper output = null)
            : base(my)
        {
            _output = output;
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

        public (int ki, int? invertedKi) GetNineStarKiDailyKi(DateTime selectedDateTime, string timeZoneId, bool isDebug = false)
        {
            using (var sweph = new SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);

                int dayKi = 0;
                int? invertedKi = null;

                DateTime selectedDateTimeUT = ConvertToUT(selectedDateTime, timeZoneId);
                DateTime startOfYear = new DateTime(selectedDateTimeUT.Year, 1, 1);
                DateTime finishOfYear = new DateTime(selectedDateTimeUT.Year, 12, 31);
                DateTime juneSolstice = FindJuneSolstice(sweph, selectedDateTimeUT.Year).Date;
                DateTime juneSolsticeAdjustmentDay =
                    FindFirstJiaZiDayBeforeSolstice(sweph, selectedDateTimeUT.Year, true).Date;
                DateTime decemberSolstice = FindDecemberSolstice(sweph, selectedDateTimeUT.Year).Date;
                DateTime decemberSolsticeAdjustmentDay = 
                    FindFirstJiaZiDayBeforeSolstice(sweph, selectedDateTimeUT.Year, false).Date;
                DateTime juneSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year, true).Date;
                DateTime decemberSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year, false).Date;
                DateTime previousDecemberSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year - 1, false).Date;
                DateTime day = selectedDateTimeUT.Date;

                var predictedJuneSolticeDayKi = NineStarKiService.InvertDirectionEnergy(IncrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI,
                    (int)juneSolstice.Subtract(previousDecemberSolsticeJiaDay).TotalDays));
                var actualJuneSolsticeDayKi = IncrementKi(SEXAGENARY_JUNE_JIA_ZI_DAY_KI, (int)juneSolsticeJiaDay.Subtract(juneSolstice).TotalDays);
                var juneAdjustmentNeeded = predictedJuneSolticeDayKi != actualJuneSolsticeDayKi;
                var juneAdjustment = predictedJuneSolticeDayKi - actualJuneSolsticeDayKi;

                var predictedDecSolticeDayKi = NineStarKiService.InvertDirectionEnergy(DecrementKi(SEXAGENARY_JUNE_JIA_ZI_DAY_KI,
                    (int)decemberSolstice.Subtract(juneSolsticeJiaDay).TotalDays));
                var actualDecSolsticeDayKi = DecrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI, (int)decemberSolsticeJiaDay.Subtract(decemberSolstice).TotalDays);
                var decAdjustmentNeeded = predictedDecSolticeDayKi != actualDecSolsticeDayKi;
                var decAdjustment = predictedDecSolticeDayKi - actualDecSolsticeDayKi;

                if (isDebug)
                {
                    Debugger.Break();
                }

                if (day <= juneSolstice.Date)
                {
                    // Get day ki
                    if (day > previousDecemberSolsticeJiaDay)
                    {
                        var daysFromPreviousDecSolsticeJiaDay = (int)day.Subtract(previousDecemberSolsticeJiaDay).TotalDays;
                        dayKi = IncrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI, daysFromPreviousDecSolsticeJiaDay);
                    }
                    else
                    {
                        var daysToPreviousDecSolsticeJiaDay = (int)previousDecemberSolsticeJiaDay.Subtract(day).TotalDays;
                        dayKi = DecrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI, daysToPreviousDecSolsticeJiaDay);
                    }

                    // Adjust if needed
                    if (juneAdjustmentNeeded && day >= juneSolsticeAdjustmentDay)
                    {
                        dayKi = AdjustKi(dayKi, juneAdjustment);
                    }
                    if (day == juneSolstice)
                    {
                        invertedKi = dayKi;
                        dayKi = NineStarKiService.InvertDirectionEnergy(dayKi);
                    }
                }
                else if (day > juneSolstice && day <= decemberSolstice)
                {
                    // Get day ki
                    if (day < juneSolsticeJiaDay)
                    {
                        var daysToNextJuneSolsticeJiaDay = (int)juneSolsticeJiaDay.Subtract(day).TotalDays;
                        dayKi = IncrementKi(SEXAGENARY_JUNE_JIA_ZI_DAY_KI, daysToNextJuneSolsticeJiaDay);

                    }
                    else
                    {
                        var daysFromJuneSolsticeJiaDay = (int)day.Subtract(juneSolsticeJiaDay).TotalDays;
                        dayKi = DecrementKi(SEXAGENARY_JUNE_JIA_ZI_DAY_KI, daysFromJuneSolsticeJiaDay);
                    }
                  
                    // Adjust if needed
                    if (decAdjustmentNeeded && day >= decemberSolsticeAdjustmentDay)
                    {
                        dayKi = AdjustKi(dayKi, decAdjustment);
                    }
                    if (day == decemberSolstice)
                    {
                        invertedKi = dayKi;
                        dayKi = NineStarKiService.InvertDirectionEnergy(dayKi);
                    }
                }
                else // day > decemberSolstice
                {
                    dayKi = IncrementKi(actualDecSolsticeDayKi,
                        (int)day.Subtract(decemberSolstice).TotalDays);
                }

                return (dayKi, invertedKi);
            }
        }
        
        private DateTime FindFirstJiaZiDayAfterSolstice(SwissEph sweph, int year, bool isJuneSolstice)
        {
            // Get the solstice date using your existing method.
            DateTime solstice = FindSolstice(sweph, year, isJuneSolstice);

            // Get the cycle day number for the solstice day.
            int solsticeCycle = GetSexagenaryCycleDayNumber(solstice);

            int offset = solsticeCycle == 1 ? 0 : 61 - solsticeCycle;

            // Return the first day after the solstice with a cycle of 1.
            return solstice.AddDays(offset);
        }

        private DateTime FindFirstJiaZiDayBeforeSolstice(SwissEph sweph, int year, bool isJuneSolstice)
        {
            // Get the solstice date using your existing method.
            DateTime solstice = FindSolstice(sweph, year, isJuneSolstice);

            // Get the cycle day number for the solstice day.
            int solsticeCycle = GetSexagenaryCycleDayNumber(solstice);

            // If the solstice is a Jia Zi day (cycle==1), then the previous Jia Zi is 60 days before.
            // Otherwise, it's (solsticeCycle - 1) days before.
            int offset = (solsticeCycle == 1) ? 60 : (solsticeCycle - 1);

            // Return the day before the solstice that has a cycle value of 1.
            return solstice.AddDays(-offset);
        }

        private static int GetSexagenaryCycleDayNumber(DateTime targetDate)
        {
            // Base date and base value: Jan 1, 1900 has the cycle value of 12.
            DateTime baseDate = BASE_KI_DATEUT;
            const int baseValue = 12;
            const int cycleLength = 60;

            // Calculate the number of full days between targetDate and baseDate.
            int daysDifference = (targetDate.Date.Date - baseDate.Date).Days;

            // Adjust the base value (convert to 0-indexed by subtracting 1), add the offset,
            // then use modulus arithmetic ensuring a non-negative result, and convert back to 1-indexed.
            int cycleValue = ((((baseValue - 1) + daysDifference - 1) % cycleLength) + cycleLength) % cycleLength + 1;

            return cycleValue;
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

        private DateTime ConvertToLocale(DateTime dateTimeUtc, string timeZoneId)
        {
            TimeZoneInfo tz = TZConvert.GetTimeZoneInfo(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, tz);
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

        private int AdjustKi(int ki, int amount = 1)
        {
            if (amount > 0)
            {
                return IncrementKi(ki, amount);
            }
            else if (amount < 0)
            {
                return DecrementKi(ki, amount);
            }
            else
            {
                return amount;
            }
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
    }
}
