using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Services
{
    public interface IUserService
    {
        void UpdateActiveUserEmailAddressIfFromFacebook(Contact contact);
    }
}