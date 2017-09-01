
using System;
using K9.Globalisation;

namespace K9.WebApplication.Extensions
{
	public static class DateTimeExtensions
	{

		public static string ToHumanReadableString(this DateTime value)
		{
			var elapsed = DateTime.Now.Subtract(value);

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
			return value.ToLongLocalDateString();
		}

	}
}
