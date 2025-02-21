using K9.Base.DataAccessLayer.Models;

namespace K9.WebApplication.Services
{
    public interface IAccountMailerService
    {
        void SendActivationEmailToUser(UserAccount.RegisterModel model, int sixDigitCode);
        void SendActivationEmailToUser(User user, int sixDigitCode);
        void SendPasswordResetEmailToUser(UserAccount.PasswordResetRequestModel model, string token);
    }
}