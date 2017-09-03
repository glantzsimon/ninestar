
using System;
using K9.Globalisation;

namespace K9.WebApplication.Extensions
{
    public static class DateTimeExtensions
    {

        public static string ToHumanReadableString(this DateTime value)
        {
            var now = DateTime.Now;
            var elapsed = now.Subtract(value);

            if (elapsed.TotalSeconds < 60)
            {
                return Dictionary.JustNow;
            }
            if (elapsed.TotalMinutes < 2)
            {
                return string.Format(Dictionary.TimespanAgo, 1, Dictionary.Minute.ToLower());
            }
            if (elapsed.TotalHours < 1)
            {
                return String.Format(Dictionary.TimespanAgo, (int)elapsed.TotalMinutes, Dictionary.Minutes.ToLower());
            }
            if ((int)elapsed.TotalHours == 1)
            {
                return string.Format(Dictionary.TimespanAgo, 1, Dictionary.Hour.ToLower());
            }
            if ((int)elapsed.TotalDays < 1)
            {
                return string.Format(Dictionary.TimespanAgo, (int)elapsed.TotalHours, Dictionary.Hours.ToLower());
            }
            if ((int)elapsed.TotalDays < 2)
            {
                return Dictionary.Yesterday;
            }
            if ((int)elapsed.TotalDays < 7)
            {
                return string.Format(Dictionary.TimespanAgo, (int)elapsed.TotalDays, Dictionary.Days.ToLower());
            }
            if ((int)elapsed.TotalDays == 7)
            {
                return string.Format(Dictionary.TimespanAgo, 1, Dictionary.Week.ToLower());
            }
            if ((int)elapsed.TotalDays < 14)
            {
                return string.Format(Dictionary.TimespanAgo, (int)elapsed.TotalDays, Dictionary.Days.ToLower());
            }
            if ((int)elapsed.TotalDays == 14)
            {
                return string.Format(Dictionary.TimespanAgo, 2, Dictionary.Weeks.ToLower());
            }
            if ((int)elapsed.TotalDays < 21)
            {
                return string.Format(Dictionary.TimespanAgo, (int)elapsed.TotalDays, Dictionary.Days.ToLower());
            }
            if ((int)elapsed.TotalDays == 21)
            {
                return string.Format(Dictionary.TimespanAgo, 3, Dictionary.Weeks.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-1)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, (int)elapsed.TotalDays, Dictionary.Days.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-2)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 1, Dictionary.Month.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-3)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 2, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-4)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 3, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-5)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 4, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-6)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 5, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-7)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 6, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-8)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 7, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-9)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 8, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-10)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 9, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-11)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 10, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddMonths(-12)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 11, Dictionary.Months.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddYears(-1).AddMonths(-6)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 1, Dictionary.Year.ToLower());
            }
            if (elapsed.TotalDays < now.Subtract(now.AddYears(-2)).TotalDays)
            {
                return string.Format(Dictionary.TimespanAgo, 18, Dictionary.Months.ToLower());
            }
            if (value < now)
            {
                return string.Format(Dictionary.TimespanAgo, now.GetYearsElapsedSince(value), Dictionary.Years.ToLower());
            }
            return value.ToLongLocalDateString();
        }

        public static int GetYearsElapsedSince(this DateTime value, DateTime dateToCompare)
        {
            if (dateToCompare < value)
            {
                return value.Year - dateToCompare.Year - (dateToCompare.Day > value.Day ? 1 : 0);
            }
            return value.Year - dateToCompare.Year - (dateToCompare.Day < value.Day ? 1 : 0);
        }
    }
    
}
