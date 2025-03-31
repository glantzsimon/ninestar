using System;

namespace K9.SharedLibrary.Extensions
{
    public static class DateTimeExtensions
    {

        public static bool IsBetween(this DateTime value, DateTime startDate, DateTime endDate)
        {
            return value >= startDate && value <= endDate;
        }

        public static string ToShortDateFormatString(this DateTime value)
        {
            return value.ToString("dd/MMM/yy").ToLower();
        }

        public static string ToLongDateTimeString(this DateTime value)
        {
            return value.ToString("dd MMMM yyyy - HHHH:mm");
        }

        public static string ToAjaxDateTimeString(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd HHHH:mm");
        }

        public static int GetAgeInYearsAsOf(this DateTime birthDate, DateTime date)
        {
            int age = date.Year - birthDate.Year;
            if (!HasBirthdayPassed(birthDate, date))
            {
                age--;
            }
            return age;
        }

        public static bool HasBirthdayPassed(this DateTime birthDate, DateTime? date = null)
        {
            date = date.HasValue ? date : DateTime.Today;
            DateTime birthdayThisYear = new DateTime(date.Value.Year, birthDate.Month, birthDate.Day);

            return date >= birthdayThisYear;
        }

    }
}
