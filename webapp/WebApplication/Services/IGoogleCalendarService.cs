using K9.WebApplication.Models;
using System;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IGoogleCalendarService : IBaseService
    {
        // Calendar dates in the member's saved timezone; endDate is exclusive.
        List<CalendarEntry> GetCalendarEntries(int userId, DateTime startDate, DateTime endDate);
        string ConvertToICalendar(int userId, List<CalendarEntry> entries);
        string GenerateCalendar(int userId, DateTime startDate, DateTime endDate);
    }
}
