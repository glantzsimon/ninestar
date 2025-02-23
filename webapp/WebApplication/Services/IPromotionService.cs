using System.Collections.Generic;
using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Services
{
    public interface IPromotionService : IBaseService
    {
        Promotion Find(string code);
        UserPromotion FindForUser(string code, int userId);
        List<UserPromotion> ListForUser(int userId);
        bool IsPromotionAlreadyUsed(string code, int userId);
        void UsePromotion(int userId, string code);
        void SendRegistrationPromotion(EmailPromoCodeViewModel model);
        void SendMembershipPromotion(EmailPromoCodeViewModel model);
        void SendFirstMembershipReminderToUser(int userId);
        void SendSecondMembershipReminderToUser(int userId);
        void SendThirdMembershipReminderToUser(int userId);
    }
}