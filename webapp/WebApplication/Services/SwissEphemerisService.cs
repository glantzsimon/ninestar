using K9.WebApplication.Packages;
using SwissEphNet;
using System;
using System.Collections.Generic;
using TimeZoneConverter;

namespace K9.WebApplication.Services
{
    public class SwissEphemerisService : BaseService, ISwissEphemerisService
    {
        public SwissEphemerisService(INineStarKiBasePackage my)
            : base(my)
        {
            my.DefaultValuesConfiguration.ValidateSwephPath();

            BASE_KI_DATEUT = ConvertToUT(new DateTime(1900, 1, 1, 1, 0, 0), BASE_DAY_TIMEZONE);
        }

        private const int SE_GREG_CAL = 1; // Gregorian calendar
        private const int SEFLG_SWIEPH = 2; // Swiss Ephemeris flag
        private const double PRECISE_YEAR_LENGTH = 365.242190419;
        private const string BASE_DAY_TIMEZONE = "Europe/London";
        private const int BASE_DAY_KI = 2;
        private const int BASE_HOUR_KI = 5;
        private static DateTime BASE_KI_DATEUT;

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

        /// <summary>
        /// Returns the Gregorian calendar month number, from 1 - 12
        /// </summary>
        /// <param name="birthDate"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public int GetNineStarKiMonthNumber(DateTime birthDate, string timeZoneId)
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

                // **Get Solar Terms for the 12 months after Lìchūn**
                double[] solarTerms = GetSolarTerms(sweph, birthYear);

                // **Find the correct month by comparing birthdate with solar terms**
                for (int i = 0; i < 11; i++) // Avoid out-of-bounds
                {
                    if (jdBirth >= solarTerms[i] && jdBirth < solarTerms[i + 1])
                    {
                        return i + 2;
                    }
                }

                // **If birth date is after last solar term, assign last month**
                return 12;
            }
        }

        // Method to calculate the ascending and descending days from baseKiDate to selected date
        public (double ascendingDays, double descendingDays) CalculateAscendingDescendingDays(DateTime selectedDate, string timeZoneId)
        {
            using (var sweph = new SwissEphNet.SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);

                double ascendingDays = 0;
                double descendingDays = 0;

                // The number of days per degree of solar longitude
                double daysPerDegree = PRECISE_YEAR_LENGTH / 360;

                // Loop through each year from baseKiDate to selected date
                for (int year = BASE_KI_DATEUT.Year; year <= selectedDate.Year; year++)
                {
                    DateTime startOfYear = new DateTime(year, 1, 1);
                    DateTime endOfYear = new DateTime(year, 12, 31);

                    // Adjust endOfYear if the selected date is before the end of the year
                    if (year == selectedDate.Year)
                    {
                        endOfYear = selectedDate;
                    }

                    // Get solar longitude for the start and end of the year
                    double startLongitude = GetSolarLongitude(sweph, startOfYear);
                    double endLongitude = GetSolarLongitude(sweph, endOfYear);
                    
                    // Handle the first phase (ascending or descending)
                    if (startLongitude < 90 || (startLongitude >= 270 && startLongitude < 360))
                    {
                        // Ascending phase: 0° to 90° or 270° to 360°
                        if (endLongitude <= 90 || (endLongitude >= 270 && endLongitude < 360))
                        {
                            // Both start and end are within the ascending phase
                            if (endLongitude < startLongitude)
                            {
                                ascendingDays += (endLongitude + 360) - startLongitude;
                            }
                            else
                            {
                                ascendingDays += endLongitude - startLongitude;
                            }
                        }
                        else
                        {
                            // Start in ascending phase, end in descending (90° to 270°)
                            ascendingDays += 90 - startLongitude;  // First ascending part
                            descendingDays += endLongitude - 90;  // Descending part
                        }
                    }
                    else if (startLongitude >= 90 && startLongitude < 270)
                    {
                        // Descending phase: 90° to 270°
                        if (endLongitude >= 90 && endLongitude < 270)
                        {
                            // Both start and end are within the descending phase
                            descendingDays += endLongitude - startLongitude;
                        }
                        else
                        {
                            // Start in descending, end in ascending (0° to 90° or 270° to 360°)
                            descendingDays += 270 - startLongitude;  // First descending part
                            ascendingDays += endLongitude - 270;  // Ascending part
                        }
                    }
                }

                // Apply the scaling factor for each degree of longitude
                ascendingDays *= daysPerDegree;
                descendingDays *= daysPerDegree;

                return (ascendingDays, descendingDays);
            }
        }

        public int CalculateKi(DateTime selectedDate, string timeZoneId)
        {
            using (var sweph = new SwissEphNet.SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);

                (double ascendingDays, double descendingDays) = CalculateAscendingDescendingDays(selectedDate, timeZoneId);

                // Use ascending and descending days to determine the Ki for the selected date
                int baseKi = BASE_DAY_KI;
                if (ascendingDays > descendingDays)
                {
                    baseKi = (baseKi + ((int)ascendingDays - 1)) % 9 + 1;
                }
                else
                {
                    baseKi = (baseKi + ((int)descendingDays - 1)) % 9 + 1;
                }

                return baseKi;
            }
        }

        public int GetNineStarKiDailyKi(DateTime currentDate, string timeZoneId)
        {
            return CalculateKi(currentDate, timeZoneId);

            using (var sweph = new SwissEphNet.SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);

                // Convert the current date to UTC based on the provided time zone
                DateTime currentDateTimeInUtc = ConvertToUT(currentDate, timeZoneId);

                // Initialize arrays to hold planetary data
                double[] sunPosition = new double[6]; // Array to hold the Sun's position and related data

                // Search for the Summer and December Solstice by calculating the Sun's position
                DateTime juneSolstice = FindSolstice(sweph, currentDateTimeInUtc, 180); // 180 degrees for the June Solstice
                DateTime decemberSolstice = FindSolstice(sweph, currentDateTimeInUtc, 0); // 0 degrees for the December Solstice
                DateTime previousDecemberSolstice = FindSolstice(sweph, currentDateTimeInUtc.AddYears(-1), 0); // 0 degrees for the December Solstice

                // Calculate the day of the Nine Star Ki year (from January 1st)
                int dayOfNineStarKiYear = (currentDateTimeInUtc - new DateTime(currentDateTimeInUtc.Year, 1, 1)).Days + 1;

                // Normalize the starting point: 1st Jan 1900 is day 2
                int baseKi = 2; // Starting point for January 1, 1900

                int dailyKi;

                // Ascending Cycle (before June Solstice)
                if (currentDateTimeInUtc > previousDecemberSolstice && currentDateTimeInUtc <= juneSolstice)
                {
                    // Ascending energy period: Ki number starts increasing
                    dailyKi = (baseKi + (dayOfNineStarKiYear - 1)) % 9 + 1;
                }
                // Descending Cycle (after June Solstice)
                else if (currentDateTimeInUtc > juneSolstice && currentDateTimeInUtc <= decemberSolstice)
                {
                    // Descending energy period: Ki number starts decreasing
                    dailyKi = (baseKi - (dayOfNineStarKiYear - juneSolstice.DayOfYear) - 1) % 9;

                    // Ensure the Ki number stays within the 1-9 range, using wrapping
                    if (dailyKi <= 0)
                    {
                        dailyKi += 9; // Correct for negative values, such as -2 becoming 8
                    }
                }
                else
                {
                    // Handle edge cases if the date is not properly fitting within the expected range
                    var details =
                        $"{Environment.NewLine}" +
                        $"JuneSolticeDate: {juneSolstice.ToString()} {Environment.NewLine}" +
                        $"DecemberSolsticeDate: {decemberSolstice.ToString()} {Environment.NewLine}" +
                        $"PreviousDecemberSolsticeDate: {previousDecemberSolstice.ToString()} {Environment.NewLine}" +
                        $"CurrentDate: {currentDateTimeInUtc.ToString()}";

                    throw new InvalidOperationException($"The calculated date does not fall within the expected solstice cycle. Details: {details}");
                }

                return dailyKi;
            }
        }

        // Method to calculate solar longitude for a given date using sweph
        private double GetSolarLongitude(SwissEph sweph, DateTime date)
        {
            double jd = GetJulianDate(sweph, date);

            // Array to hold the Sun's position
            double[] sunPosition = new double[6]; // [0] = ecliptic longitude, [1] = ecliptic latitude, [2] = distance, etc.
            string error = "";

            // Calculate Sun's position (ecliptic longitude)
            sweph.swe_calc(jd, SwissEph.SE_SUN, SwissEph.SEFLG_SWIEPH, sunPosition, ref error);

            // The solar longitude is the ecliptic longitude (in degrees) of the Sun
            return sunPosition[0]; // The ecliptic longitude is the first element in the array
        }

        private DateTime FindSolstice(SwissEph sweph, DateTime currentDateTimeInUtc, double targetLongitude)
        {
            // Convert current date to Julian Date
            double jdCurrent = GetJulianDate(sweph, currentDateTimeInUtc);

            // Get the current year
            int currentYear = currentDateTimeInUtc.Year;

            // Calculate the Julian Date for the start of the current calendar year (January 1st)
            double jdStartOfYear = GetJulianDate(sweph, new DateTime(currentYear, 1, 1));

            // Find the solstice date using the target longitude (0° for Winter, 180° for Summer)
            double jdSolstice = FindSolsticeJulianDay(sweph, jdStartOfYear, targetLongitude);

            // Ensure the solstice date is within the calendar year range
            double jdEndOfYear = GetJulianDate(sweph, new DateTime(currentYear, 12, 31, 23, 59, 59));

            if (jdSolstice >= jdStartOfYear && jdSolstice <= jdEndOfYear)
            {
                // Return the solstice date as a DateTime
                return JulianDayToDateTime(sweph, jdSolstice);
            }

            // If solstice is not found within the calendar year, throw an error
            var details = $"{Environment.NewLine}" +
                          $"currentDateTimeInUtc: {currentDateTimeInUtc.ToString()} {Environment.NewLine}" +
                          $"jdStartOfYear: {jdStartOfYear} {Environment.NewLine}" +
                          $"jdEndOfYear: {jdEndOfYear} {Environment.NewLine}" +
                          $"jdSolstice: {jdSolstice}";

            throw new InvalidOperationException($"The calculated solstice date does not fall within the expected calendar year. Details: {details}");
        }

        private double FindSolsticeJulianDay(SwissEph sweph, double startJulianDay, double targetLongitude)
        {
            double[] sunPosition = new double[6]; // Array to hold the Sun's position
            double currentJulianDay = startJulianDay;
            string serr = "";

            // Loop to find the exact solar longitude crossing the target (0° or 180°)
            while (true)
            {
                // Calculate the Sun's position for the current Julian day
                int ret = sweph.swe_calc(currentJulianDay, SwissEph.SE_SUN, SwissEph.SEFLG_SWIEPH, sunPosition, ref serr);

                // Check if the Sun's longitude is close to the target (0° for December Solstice or 180° for June Solstice)
                if (Math.Abs(sunPosition[0] - targetLongitude) < 0.1)
                {
                    // Return the solstice Julian Day
                    return currentJulianDay;
                }

                // Increment Julian day to continue searching
                currentJulianDay += 0.1; // Small step size to ensure precision
            }
        }

        private DateTime JulianDayToDateTime(SwissEph sweph, double julianDay)
        {
            // Initialize variables to hold the results of the conversion
            int gregflag = 0;   // Calendar flag (not used in the conversion)
            int year = 0, month = 0, day = 0, hour = 0, minute = 0;
            double second = 0.0;

            // Convert Julian Day to UTC using swe_jdet_to_utc method
            sweph.swe_jdet_to_utc(julianDay, gregflag, ref year, ref month, ref day, ref hour, ref minute, ref second);

            DateTime utcDateTime = new DateTime(
                year, month, day, hour, minute, (int)second, DateTimeKind.Utc); // Setting the DateTime to UTC

            return utcDateTime;
        }

        private DateTime GetSolarTermDate(int year, double targetLongitude)
        {
            using (var sweph = new SwissEphNet.SwissEph())
            {
                sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);

                // Use the GetSolarTerm method to find the exact date of Lìchūn (315° longitude)
                double julianDay = GetSolarTerm(sweph, year, targetLongitude); // Get the Julian day for the solar term

                // Convert the Julian Day to DateTime using swe_revjul
                int yearOut = 0, monthOut = 0, dayOut = 0;
                double hourOut = 0;

                // Convert Julian Day to DateTime (using swe_revjul)
                sweph.swe_revjul(julianDay, SwissEph.SE_GREG_CAL, ref yearOut, ref monthOut, ref dayOut, ref hourOut);

                // Return the corresponding DateTime for Lìchūn
                return new DateTime(yearOut, monthOut, dayOut);
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

        private static readonly Dictionary<int, int> _invertedEnergies = new Dictionary<int, int>
        {
            { 1, 5 }, { 2, 4 }, { 4, 2 }, { 5, 1 },
            { 6, 9 }, { 7, 8 }, { 8, 7 }, { 9, 6 }
        };

        private int InvertEnergy(int energyNumber) =>
            _invertedEnergies.TryGetValue(energyNumber, out var inverted) ? inverted : energyNumber;

    }
}
