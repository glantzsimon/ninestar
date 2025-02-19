using System;

namespace K9.SharedLibrary.Extensions
{
    public static class DateTimeExtensions
	{
        
		public static bool IsBetween(this DateTime value, DateTime startDate, DateTime endDate)
		{
			return value >= startDate && value <= endDate;
		}
	}
}
