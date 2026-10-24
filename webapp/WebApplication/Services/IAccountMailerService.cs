using K9.Base.DataAccessLayer.Models;

namespace K9.WebApplication.Services
{
    public interface IAccountMailerService
    {
        void SendActivationEmailToUser(UserAccount.RegisterModel model, UserOTP otp);
        void SendActivationEmailToUser(User user, UserOTP otp);
        void SendPasswordResetEmailToUser(UserAccount.PasswordResetRequestModel model, string token);
    }
}