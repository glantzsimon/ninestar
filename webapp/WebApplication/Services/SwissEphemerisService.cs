using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using SwissEphNet;
using System;
using System.Collections.Generic;
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

        public int GetNineStarKiEightyOneYearKi(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiEightyOneYearKi)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedDateTimeUT = ConvertToUT(selectedDateTime, timeZoneId);
                    int adjustedYear = AdjustYearForLichun(sweph, selectedDateTimeUT);
                    int periodIndex = (int)Math.Floor((adjustedYear - 1955) / 81.0);
                    return ((((9 - periodIndex) - 1) % 9) + 9) % 9 + 1;
                }
            }, TimeSpan.FromDays(30));
        }

        public int GetNineStarKiNineYearKi(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiNineYearKi)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedDateTimeUT = ConvertToUT(selectedDateTime, timeZoneId);
                    int adjustedYear = AdjustYearForLichun(sweph, selectedDateTimeUT);
                    int periodIndex = (int)Math.Floor((adjustedYear - 1991) / 9.0);
                    return ((((5 - periodIndex) - 1) % 9) + 9) % 9 + 1;
                }
            }, TimeSpan.FromDays(30));
        }

        public int GetNineStarKiYearlyKi(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiYearlyKi)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedDateTimeUt = ConvertToUT(selectedDateTime, timeZoneId);
                    int adjustedYear = AdjustYearForLichun(sweph, selectedDateTimeUt);
                    return GetNineStarKiNumber(adjustedYear);
                }
            }, TimeSpan.FromDays(30));
        }

        public int GetNineStarKiMonthlyKi(DateTime selectedDateTime, string timeZoneId, bool invert = false)
        {
            string cacheKey = $"{nameof(GetNineStarKiMonthlyKi)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}_{invert}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedDateTimeUT = ConvertToUT(selectedDateTime, timeZoneId);
                    int adjustedYear = AdjustYearForLichun(sweph, selectedDateTimeUT);
                    int yearEnergy = GetNineStarKiNumber(adjustedYear, invert);
                    int firstMonth = GetFirstMonthForYearEnergy(yearEnergy);
                    double jd = GetJulianDate(sweph, selectedDateTimeUT);
                    double[] solarTerms = GetSolarTerms(sweph, adjustedYear);

                    // There are 12 boundaries defining 12 intervals.
                    for (int i = 0; i < solarTerms.Length - 1; i++)
                    {
                        if (jd >= solarTerms[i] && jd < solarTerms[i + 1])
                        {
                            // Descending cycle: subtract i
                            return ((firstMonth - 1 - i + 9) % 9) + 1;
                        }
                    }
                    return ((firstMonth - 1 - (solarTerms.Length - 1) + 9) % 9) + 1;
                }
            }, TimeSpan.FromDays(30));
        }

        public int GetNineStarKiMonthNumber(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiMonthNumber)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedDateTimeUT = ConvertToUT(selectedDateTime, timeZoneId);
                    int adjustedYear = AdjustYearForLichun(sweph, selectedDateTimeUT);
                    double jd = GetJulianDate(sweph, selectedDateTimeUT);
                    double[] solarTerms = GetSolarTerms(sweph, adjustedYear);

                    for (int i = 0; i < 11; i++)
                    {
                        if (jd >= solarTerms[i] && jd < solarTerms[i + 1])
                        {
                            return i + 2;
                        }
                    }
                    return 12;
                }
            }, TimeSpan.FromDays(30));
        }

        public (int DailyKi, int? InvertedDailyKi) GetNineStarKiDailyKi(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiDailyKi)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
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
                    DateTime juneSolsticeAdjustmentDay = FindFirstJiaZiDayBeforeSolstice(sweph, selectedDateTimeUT.Year, true).Date;
                    DateTime decemberSolstice = FindDecemberSolstice(sweph, selectedDateTimeUT.Year).Date;
                    DateTime decemberSolsticeAdjustmentDay = FindFirstJiaZiDayBeforeSolstice(sweph, selectedDateTimeUT.Year, false).Date;
                    DateTime juneSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year, true).Date;
                    DateTime decemberSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year, false).Date;
                    DateTime previousDecemberSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year - 1, false).Date;
                    DateTime day = selectedDateTimeUT.Date;

                    var predictedJuneSolticeDayKi = NineStarKiModel.GetOppositeEnergyInMagicSquare(IncrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI,
                        (int)juneSolstice.Subtract(previousDecemberSolsticeJiaDay).TotalDays));
                    var actualJuneSolsticeDayKi = IncrementKi(SEXAGENARY_JUNE_JIA_ZI_DAY_KI, (int)juneSolsticeJiaDay.Subtract(juneSolstice).TotalDays);
                    var juneAdjustmentNeeded = predictedJuneSolticeDayKi != actualJuneSolsticeDayKi;
                    var juneAdjustment = predictedJuneSolticeDayKi - actualJuneSolsticeDayKi;

                    var predictedDecSolticeDayKi = NineStarKiModel.GetOppositeEnergyInMagicSquare(DecrementKi(SEXAGENARY_JUNE_JIA_ZI_DAY_KI,
                        (int)decemberSolstice.Subtract(juneSolsticeJiaDay).TotalDays));
                    var actualDecSolsticeDayKi = DecrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI, (int)decemberSolsticeJiaDay.Subtract(decemberSolstice).TotalDays);
                    var decAdjustmentNeeded = predictedDecSolticeDayKi != actualDecSolsticeDayKi;
                    var decAdjustment = predictedDecSolticeDayKi - actualDecSolsticeDayKi;

                    if (day <= juneSolstice.Date)
                    {
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

                        if (juneAdjustmentNeeded && day >= juneSolsticeAdjustmentDay)
                        {
                            dayKi = AdjustKi(dayKi, juneAdjustment);
                        }
                        if (day == juneSolstice)
                        {
                            invertedKi = dayKi;
                            dayKi = NineStarKiModel.GetOppositeEnergyInMagicSquare(dayKi);
                        }
                    }
                    else if (day > juneSolstice && day <= decemberSolstice)
                    {
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

                        if (decAdjustmentNeeded && day >= decemberSolsticeAdjustmentDay)
                        {
                            dayKi = AdjustKi(dayKi, decAdjustment);
                        }
                        if (day == decemberSolstice)
                        {
                            invertedKi = dayKi;
                            dayKi = NineStarKiModel.GetOppositeEnergyInMagicSquare(dayKi);
                        }
                    }
                    else
                    {
                        dayKi = IncrementKi(actualDecSolsticeDayKi,
                            (int)day.Subtract(decemberSolstice).TotalDays);
                    }

                    return (dayKi, invertedKi);
                }
            }, TimeSpan.FromDays(30));
        }

        public int GetNineStarKiHourlyKi(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiHourlyKi)}_{selectedDateTime:yyyyMMddHHHHmm}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedDateTimeUT = ConvertToUT(selectedDateTime, timeZoneId);
                    DateTime juneSolstice = FindJuneSolstice(sweph, selectedDateTimeUT.Year).Date;
                    DateTime decemberSolstice = FindDecemberSolstice(sweph, selectedDateTimeUT.Year).Date;
                    DateTime juneSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year, true).Date.AddHours(1);
                    DateTime decemberSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year, false).Date.AddHours(1);
                    DateTime previousDecemberSolsticeJiaDay = FindFirstJiaZiDayAfterSolstice(sweph, selectedDateTimeUT.Year - 1, false).Date.AddHours(1);
                    DateTime day = selectedDateTimeUT.Date;
                    int hourKi = 0;

                    if (day <= juneSolstice)
                    {
                        if (day > previousDecemberSolsticeJiaDay.Date)
                        {
                            var hoursFromPreviousDecSolsticeJiaDay = (int)selectedDateTimeUT.Subtract(previousDecemberSolsticeJiaDay).TotalHours / 2 + 1;
                            hourKi = IncrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI, hoursFromPreviousDecSolsticeJiaDay);
                        }
                        else
                        {
                            var hoursToPreviousDecSolsticeJiaDay = (int)previousDecemberSolsticeJiaDay.Subtract(selectedDateTimeUT).TotalHours / 2 + 1;
                            hourKi = DecrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI, hoursToPreviousDecSolsticeJiaDay);
                        }
                    }
                    else if (day > juneSolstice && day <= decemberSolstice)
                    {
                        if (day < juneSolsticeJiaDay.Date)
                        {
                            var hoursToNextJuneSolsticeJiaDay = (int)juneSolsticeJiaDay.Subtract(selectedDateTimeUT).TotalHours / 2 + 1;
                            hourKi = IncrementKi(SEXAGENARY_JUNE_JIA_ZI_DAY_KI, hoursToNextJuneSolsticeJiaDay);
                        }
                        else
                        {
                            var hoursFromJuneSolsticeJiaDay = (int)selectedDateTimeUT.Subtract(juneSolsticeJiaDay).TotalHours / 2 + 1;
                            hourKi = DecrementKi(SEXAGENARY_JUNE_JIA_ZI_DAY_KI, hoursFromJuneSolsticeJiaDay);
                        }
                    }
                    else
                    {
                        if (day < decemberSolsticeJiaDay.Date)
                        {
                            var hoursToNextSolsticeJiaDay = (int)decemberSolsticeJiaDay.Subtract(selectedDateTimeUT).TotalHours / 2 + 1;
                            hourKi = IncrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI, hoursToNextSolsticeJiaDay);
                        }
                        else
                        {
                            var hoursFromSolsticeJiaDay = (int)selectedDateTimeUT.Subtract(decemberSolsticeJiaDay).TotalHours / 2 + 1;
                            hourKi = DecrementKi(SEXAGENARY_DECEMBER_JIA_ZI_DAY_KI, hoursFromSolsticeJiaDay);
                        }
                    }

                    return hourKi;
                }
            }, TimeSpan.FromDays(30));
        }

        public (DateTime PeriodStartOn, DateTime PeriodEndsOn) GetNineStarKiMonthlyPeriodBoundaries(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiMonthlyPeriodBoundaries)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedUT = ConvertToUT(selectedDateTime, timeZoneId);
                    int adjustedYear = AdjustYearForLichun(sweph, selectedUT);
                    double[] solarTerms = GetSolarTerms(sweph, adjustedYear);
                    DateTime[] solarTermDates = new DateTime[solarTerms.Length];
                    for (int i = 0; i < solarTerms.Length; i++)
                    {
                        solarTermDates[i] = JulianDayToDateTime(sweph, solarTerms[i]).Date;
                    }

                    DateTime periodStart = DateTime.MinValue;
                    DateTime periodEnd = DateTime.MinValue;
                    bool found = false;
                    for (int i = 0; i < solarTermDates.Length - 1; i++)
                    {
                        if (selectedUT.Date >= solarTermDates[i] && selectedUT.Date < solarTermDates[i + 1])
                        {
                            periodStart = solarTermDates[i];
                            periodEnd = solarTermDates[i + 1].AddDays(-1);
                            found = true;
                            break;
                        }
                    }
                    if (!found && selectedUT.Date >= solarTermDates[solarTermDates.Length - 1])
                    {
                        periodStart = solarTermDates[solarTermDates.Length - 1];
                        double jdNextLichun = GetSolarTerm(sweph, adjustedYear + 1, 315.0);
                        DateTime nextLichunDate = JulianDayToDateTime(sweph, jdNextLichun).Date;
                        periodEnd = nextLichunDate.AddDays(-1);
                    }

                    return (periodStart, periodEnd);
                }
            }, TimeSpan.FromDays(30));
        }

        public (DateTime PeriodStartOn, DateTime PeriodEndsOn, int MonthlyKi)[] GetNineStarKiMonthlyPeriods(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiMonthlyPeriods)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedUT = ConvertToUT(selectedDateTime, timeZoneId);
                    int adjustedYear = AdjustYearForLichun(sweph, selectedUT);
                    double[] solarTerms = GetSolarTerms(sweph, adjustedYear);
                    DateTime[] solarTermDates = new DateTime[solarTerms.Length];
                    for (int i = 0; i < solarTerms.Length; i++)
                    {
                        var solarTerm = solarTerms[i];
                        if (solarTerm == 0)
                        {
                            Debugger.Break();
                            continue;
                        }
                        solarTermDates[i] = JulianDayToDateTime(sweph, solarTerm).Date;
                    }

                    var periods = new (DateTime PeriodStartOn, DateTime PeriodEndsOn, int MonthlyKi)[12];
                    for (int i = 0; i < solarTermDates.Length - 1; i++)
                    {
                        var solarTermDate = solarTermDates[i];
                        var monthlyKi = GetNineStarKiMonthlyKi(solarTermDate.AddDays(1), timeZoneId);
                        periods[i] = (solarTermDate, solarTermDates[i + 1].AddDays(-1), monthlyKi);
                    }

                    double jdNextLichun = GetSolarTerm(sweph, adjustedYear + 1, 315.0);
                    DateTime nextLichunDate = JulianDayToDateTime(sweph, jdNextLichun).Date;
                    var lastSolarTermDate = solarTermDates[11];
                    var lastMonthlyKi = GetNineStarKiMonthlyKi(lastSolarTermDate.AddDays(1), timeZoneId);
                    periods[11] = (lastSolarTermDate, nextLichunDate.AddDays(-1), lastMonthlyKi);

                    return periods;
                }
            }, TimeSpan.FromDays(30));
        }

        public (DateTime YearStart, DateTime YearEnd, int YearlyKi)[] GetNineStarKiYearlyPeriods(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiYearlyPeriods)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                using (var sweph = new SwissEph())
                {
                    sweph.swe_set_ephe_path(My.DefaultValuesConfiguration.SwephPath);
                    DateTime selectedUT = ConvertToUT(selectedDateTime, timeZoneId);
                    int adjustedYear = AdjustYearForLichun(sweph, selectedUT);
                    int startYear = adjustedYear - 9;
                    int endYear = adjustedYear + 9;
                    int numBoundaries = endYear - startYear + 1;  // 19 boundaries
                    DateTime[] boundaries = new DateTime[numBoundaries];
                    for (int i = 0; i < numBoundaries; i++)
                    {
                        int year = startYear + i;
                        double jd = GetSolarTerm(sweph, year, 315.0);
                        boundaries[i] = JulianDayToDateTime(sweph, jd).Date;
                    }

                    int numPeriods = numBoundaries - 1; // 18 periods
                    var periods = new (DateTime YearStart, DateTime YearEnd, int YearlyKi)[numPeriods];

                    for (int i = 0; i < numPeriods; i++)
                    {
                        DateTime yearStart = boundaries[i];
                        DateTime yearEnd = boundaries[i + 1].AddDays(-1);
                        int yearlyKi = GetNineStarKiYearlyKi(yearStart.AddDays(1), timeZoneId);
                        periods[i] = (yearStart, yearEnd, yearlyKi);
                    }

                    return periods;
                }
            }, TimeSpan.FromDays(30));
        }

        public (DateTime Day, int DailyKi, int? InvertedDailyKi)[] GetNineStarKiDailyEnergiesForMonth(DateTime selectedDateTime, string timeZoneId)
        {
            string cacheKey = $"{nameof(GetNineStarKiDailyEnergiesForMonth)}_{selectedDateTime:yyyyMMddHH}_{timeZoneId}";
            return GetOrAddToCache(cacheKey, () =>
            {
                var (PeriodStartOn, PeriodEndsOn) = GetNineStarKiMonthlyPeriodBoundaries(selectedDateTime, timeZoneId);
                var dailyEnergies = new List<(DateTime Day, int DailyKi, int? InvertedDailyKi)>();

                for (DateTime day = PeriodStartOn; day <= PeriodEndsOn; day = day.AddDays(1))
                {
                    var dayWithTime = day.Add(selectedDateTime.TimeOfDay);
                    var dailyKi = GetNineStarKiDailyKi(dayWithTime, timeZoneId);
                    dailyEnergies.Add((dayWithTime, dailyKi.DailyKi, dailyKi.InvertedDailyKi));
                }

                return dailyEnergies.ToArray();
            }, TimeSpan.FromDays(30));
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

        private int GetNineStarKiNumber(int year, bool invert = false)
        {
            int kiNumber = (11 - (year % 9)) % 9;
            kiNumber = kiNumber == 0 ? 9 : kiNumber;
            return invert ? NineStarKiModel.InvertEnergy(kiNumber) : kiNumber;
        }

        private DateTime ConvertToUT(DateTime rawDateTime, string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId))
            {
                return rawDateTime; // Presumed to be UTC
            }

            DateTime localTime = DateTime.SpecifyKind(rawDateTime, DateTimeKind.Unspecified);
            var tz = TZConvert.GetTimeZoneInfo(timeZoneId);
            if (tz.IsInvalidTime(localTime))
            {
                // Adjust localTime to a valid time. For example, add an hour:
                localTime = localTime.AddHours(1);
            }
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, tz);
            return utcTime;
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

        /// <summary>
        /// Returns the 12 solar term boundaries (as Julian Day numbers) for the Nine Star Ki year,
        /// i.e. from Lìchūn (315°) of the adjusted year until Lìchūn of the next year.
        /// The boundaries are computed in an "unwrapped" fashion so that they are strictly increasing.
        /// </summary>
        private double[] GetSolarTerms(SwissEph sweph, int adjustedYear)
        {
            // Compute the starting and ending boundaries of the Nine Star Ki year.
            double jdStart = GetSolarTerm(sweph, adjustedYear, 315.0);         // Lìchūn of adjustedYear
            double jdEnd = GetSolarTerm(sweph, adjustedYear + 1, 315.0) + 1.0;       // Lìchūn of next year plus one day

            double[] solarTerms = new double[12];
            for (int i = 0; i < 12; i++)
            {
                // Compute the raw target angle.
                double rawTarget = 315.0 + (i * 30.0);
                double target = rawTarget % 360; // This gives a value in 0–359.
                                                 // Unwrap the target: if it's less than 315, add 360.
                double unwrappedTarget = (target < 315) ? target + 360 : target;

                // Compute the boundary within the fixed interval [jdStart, jdEnd] using the unwrapped target.
                var solarTermWithinInterval = GetSolarTermWithinInterval(sweph, unwrappedTarget, jdStart, jdEnd);
                solarTerms[i] = solarTermWithinInterval;
            }
            return solarTerms;
        }

        /// <summary>
        /// Finds the time (in JD) when the sun's unwrapped longitude reaches the unwrappedTarget,
        /// searching within the interval [jdStart, jdEnd].
        /// </summary>
        private double GetSolarTermWithinInterval(SwissEph sweph, double target, double jdStart, double jdEnd)
        {
            double tolerance = 0.01; // degrees tolerance
            int maxIterations = 100;
            int iteration = 0;
            string serr = "";

            // For the 315° target, adjust jdStart by subtracting a small delta so that the computed value falls below 315.
            if (Math.Abs(target - 315) < 1e-6)
            {
                jdStart -= 0.001;  // Adjust by 0.01 days (you may tweak this value if needed)
            }

            double jdLow = jdStart;
            double jdHigh = jdEnd;
            double mid = 0;

            while (iteration < maxIterations)
            {
                iteration++;
                mid = (jdLow + jdHigh) / 2;
                double[] xx = new double[6];
                int ret = sweph.swe_calc_ut(mid, SwissEph.SE_SUN, SwissEph.SEFLG_SWIEPH, xx, ref serr);
                if (ret < 0)
                    throw new Exception("Error calculating Sun position: " + serr);

                // Get the computed sun longitude.
                double computed = xx[0];
                // Unwrap the computed longitude: if it's less than 315, add 360.
                double unwrappedComputed = (computed < 315) ? computed + 360 : computed;

                // Use binary search.
                if (unwrappedComputed < target)
                    jdLow = mid;
                else
                    jdHigh = mid;

                if (Math.Abs(unwrappedComputed - target) < tolerance)
                    return mid;
            }
            throw new Exception($"Could not find solar term at {target}° between JD {jdStart} and {jdEnd} after {maxIterations} iterations.");
        }

        /// <summary>
        /// Adjusts the year based on Lìchūn (315° solar term) so that if the 
        /// date is before Lìchūn in January–February, the previous year is used.
        /// </summary>
        private int AdjustYearForLichun(SwissEph sweph, DateTime dateTimeUT)
        {
            int year = dateTimeUT.Year;
            double jdLichun = GetSolarTerm(sweph, year, 315.0);
            double jd = GetJulianDate(sweph, dateTimeUT);
            return (jd < jdLichun && dateTimeUT.Month <= 2) ? year - 1 : year;
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
