using K9.WebApplication.Packages;
using System;

namespace K9.WebApplication.Services
{
    public class GoogleCalendarService : BaseService, IGoogleCalendarService
    {
        public GoogleCalendarService(INineStarKiBasePackage my)
            : base(my)
        {
        }

        public string GetOrCreateSubscriptionUrl(int userId)
        {
            throw new NotImplementedException();
        }

        public string GenerateCalendar(int userId, DateTime startDate,
            DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public int? GetUserIdFromSubscriptionToken(string token)
        {
            throw new NotImplementedException();
        }

        public void RevokeSubscription(int userId)
        {
            throw new NotImplementedException();
        }
    }
}