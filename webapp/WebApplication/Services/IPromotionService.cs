using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Services
{
    public interface IPromotionService : IBaseService
    {
        PromoCode Find(string code);
        bool IsPromoCodeAlreadyUsed(string code);
        void UsePromoCode(int userId, string code);
        void SendRegistrationPromoCode(EmailPromoCodeViewModel model);
        void SendMembershipPromoCode(EmailPromoCodeViewModel model);
        void SendFirstMembershipReminderToUser(int userId);
        void SendSecondMembershipReminderToUser(int userId);
        void SendThirdMembershipReminderToUser(int userId);
    }
}