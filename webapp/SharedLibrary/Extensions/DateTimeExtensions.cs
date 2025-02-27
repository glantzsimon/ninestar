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
	        return value.ToString("dd/MMM/yy");
	    }

	    public static bool HasBirthdayPassedThisYear(this DateTime dob)
	    {
	        DateTime today = DateTime.Today;
	        DateTime birthdayThisYear = new DateTime(today.Year, dob.Month, dob.Day);

	        return today >= birthdayThisYear;
	    }

	}
}
