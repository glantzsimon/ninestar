using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Models;
using K9.DataAccessLayer.Models;
using K9.WebApplication.ViewModels;
using System;

namespace K9.WebApplication.Services
{
    public interface IAccountService
    {
		ELoginResult Login(string username, string password, bool isRemember);
        ELoginResult Login(int userId);
		void Logout();
		ServiceResult Register(UserAccount.RegisterModel model);
        ServiceResult RegisterOrLoginAuth(UserAccount.RegisterModel model);
        ServiceResult RegisterPersonalInformation(RegisterViewModel model);
        ServiceResult DeleteAccount(int userId);
        
        ServiceResult UpdatePassword(UserAccount.LocalPasswordModel model);
		ServiceResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model);
        ServiceResult ResetPassword(UserAccount.ResetPasswordModel model);

        bool ConfirmUserFromToken(string username, string token);
        
        ActivateAccountResult ActivateAccount(int userId, string token = "");
		ActivateAccountResult ActivateAccount(string username, string token = "");
		ActivateAccountResult ActivateAccount(User user, string token = "");
        ActivateAccountResult ActivateAccount(int userId);

        MyAccountViewModel GetAccount(int userId);
		string GetAccountActivationToken(int userId);
        UserOTP CreateAccountActivationOTP(int userId, bool recreate = false);
        UserOTP GetAccountActivationOTP(Guid uniqueIdentifier);
        UserOTP GetOTPForUser(int userId);
        void ResendActivationCode(int userId);
        void VerifyCode(int userId, int digit1, int digit2, int digit3, int digit4, int digit5, int digit6);
        void UnverifyCode(int userId, int digit1, int digit2, int digit3, int digit4, int digit5, int digit6);

        string GetPasswordResetLink(UserAccount.PasswordResetRequestModel model, string token);
        string GetActivationLink(UserAccount.RegisterModel model, string token);
    }
}