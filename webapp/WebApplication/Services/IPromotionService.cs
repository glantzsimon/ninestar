using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IPromotionService : IBaseService
    {
        Promotion Find(int id);
        Promotion Find(string code);
        Promotion FindByMembershipOption(int membershipOptionId);
        UserPromotion FindForUser(string code, int userId);
        List<UserPromotion> ListForUser(int userId);
        bool IsPromotionAlreadyUsed(string code, int userId);
        void UsePromotion(int userId, string code);
        void SendRegistrationPromotion(EmailPromoCodeViewModel model);
        void SendMembershipPromotion(EmailPromoCodeViewModel model);
        void SendFirstMembershipReminderToUser(int userId);
        void SendSecondMembershipReminderToUser(int userId);
        void SendThirdMembershipReminderToUser(int userId);
        void SendPromotionFromTemplateToUser(int userId, EmailTemplate emailTemplate, Promotion promotion,
            bool isScheduled = false, TimeSpan? scheduledOn = null, bool isTest = false);
    }
}