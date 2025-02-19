using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IUserService
    {
        void UpdateActiveUserEmailAddressIfFromFacebook(Contact contact);
        bool IsPromoCodeAlreadyUsed(string code);
        void UsePromoCode(int userId, string code);
        void SendPromoCode(EmailPromoCodeViewModel model);
        User Find(int id);
        User Find(string username);
        List<UserConsultation> GetPendingConsultations(int? userId = null);
        void DeleteUser(int id);
        void EnableMarketingEmails(string externalId, bool value = true);
        void EnableMarketingEmails(int id, bool value = true);
        bool AreMarketingEmailsAllowedForUser(int id);
    }
}