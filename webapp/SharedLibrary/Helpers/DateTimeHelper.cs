using System;
using TimeZoneConverter;

namespace K9.SharedLibrary.Helpers
{
    public class DateTimeHelper
	{

	    public static DateTime ConvertToLocaleDateTime(DateTime rawdateTime, string timeZoneId)
	    {
	        rawdateTime = DateTime.SpecifyKind(rawdateTime, DateTimeKind.Unspecified);
	        TimeZoneInfo tz = TZConvert.GetTimeZoneInfo(timeZoneId);
	        return TimeZoneInfo.ConvertTimeFromUtc(rawdateTime, tz);
	    }

    }
}
