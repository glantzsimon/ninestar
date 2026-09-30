using System;

namespace K9.WebApplication.Services
{
    public interface IGoogleCalendarService : IBaseService
    {
        string GetOrCreateSubscriptionUrl(int userId);

        string GenerateCalendar(int userId, DateTime startDate, DateTime endDate);

        int? GetUserIdFromSubscriptionToken(string token);

        void RevokeSubscription(int userId);
    }
}