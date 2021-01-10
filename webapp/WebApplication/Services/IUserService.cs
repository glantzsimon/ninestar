using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Services
{
    public interface IUserService
    {
        void UpdateActiveUserEmailAddressIfFromFacebook(Contact contact);
        void CheckPromoCode(int userId, string code);
        void SendPromoCode(EmailPromoCodeViewModel model);
    }
}