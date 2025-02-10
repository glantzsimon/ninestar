using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IUserService
    {
        void UpdateActiveUserEmailAddressIfFromFacebook(Contact contact);
        bool CheckIfPromoCodeIsUsed(string code);
        void UsePromoCode(int userId, string code);
        void SendPromoCode(EmailPromoCodeViewModel model);
        User Find(int userId);
        User Find(string username);
        List<UserConsultation> GetPendingConsultations(int? userId = null);
    }
}