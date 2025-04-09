using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IUserService : IBaseService
    {
        void UpdateActiveUserEmailAddressIfFromFacebook(Contact contact);
        User Find(int id);
        User Find(string username);
        UserInfo GetOrCreateUserInfo(int userId);
        List<UserConsultation> GetPendingConsultations(int? userId = null);
        void UpdateUserInfo(UserInfo userInfo);
        void DeleteUser(int id);
        void EnableMarketingEmails(string externalId, bool value = true);
        void EnableMarketingEmails(int id, bool value = true);
        bool AreMarketingEmailsAllowedForUser(int id);
    }
}