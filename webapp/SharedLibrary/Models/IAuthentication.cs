using System;

namespace K9.SharedLibrary.Models
{
	public interface IAuthentication
	{
	    string CurrentUserName { get; }
        int CurrentUserId { get; }
        bool Initialized { get; }
        bool HasUserId { get; }
        bool IsAuthenticated { get; }
        bool ChangePassword(string userName, string currentPassword, string newPassword);
        bool ConfirmAccount(string accountConfirmationToken);
        bool ConfirmAccount(string userName, string accountConfirmationToken);
        string CreateAccount(string userName, string password, bool requireConfirmationToken = false);
        string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false);
        string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440);
        DateTime GetCreateDate(string userName);
        DateTime GetLastPasswordFailureDate(string userName);
        DateTime GetPasswordChangedDate(string userName);
        int GetPasswordFailuresSinceLastSuccess(string userName);
        int GetUserId(string userName);
        int GetUserIdFromPasswordResetToken(string token);

        bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, int intervalInSeconds);
        bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, TimeSpan interval);
        bool IsConfirmed(string userName);
        bool IsCurrentUser(string userName);
        bool Login(string userName, string password, bool persistCookie = false);
        void Logout();
        void RequireAuthenticatedUser();
        void RequireRoles(params string[] roles);
        void RequireUser(string userName);
        void RequireUser(int userId);
        bool ResetPassword(string passwordResetToken, string newPassword);
        bool UserExists(string userName);
    }
}