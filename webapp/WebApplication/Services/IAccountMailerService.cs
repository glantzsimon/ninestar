using K9.Base.DataAccessLayer.Models;

namespace K9.WebApplication.Services
{
    public interface IAccountMailerService
    {
        void SendActivationEmail(UserAccount.RegisterModel model, int sixDigitCode);
        void SendPasswordResetEmail(UserAccount.PasswordResetRequestModel model, string token);
    }
}