using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Enums;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    // Uses the established Forms Authentication provider. A native client keeps
    // the returned cookies and obtains a new CSRF token after login/logout.
    [RoutePrefix("api/mobile/v1/account")]
    public class MobileAccountController : MobileApiControllerBase
    {
        private readonly IRecaptchaService _captcha;
        public MobileAccountController(INineStarKiPackage package, IRecaptchaService captcha) : base(package) { _captcha = captcha; }

        [HttpGet, Route("csrf")]
        public ActionResult Csrf()
        {
            AntiForgery.GetTokens(Request.Cookies[AntiForgeryConfig.CookieName]?.Value, out var cookie, out var requestToken);
            if (cookie != null)
                Response.Cookies.Add(new HttpCookie(AntiForgeryConfig.CookieName, cookie)
                {
                    HttpOnly = true, Secure = Request.IsSecureConnection,
                    Path = string.IsNullOrEmpty(Request.ApplicationPath) ? "/" : Request.ApplicationPath
                });
            return ApiSuccess(new { RequestToken = requestToken });
        }

        [HttpPost, Route("login")]
        public ActionResult Login(MobileLoginRequest request)
        {
            if (!Request.IsSecureConnection && !Request.IsLocal) return ApiError(400, "https_required");
            if (!ValidInput(request)) return ApiError(400, "invalid_login_request");
            var username = request.Username;
            if (username.Contains("@"))
                username = Package.UsersRepository.Find(e => e.EmailAddress == username).FirstOrDefault()?.Username ?? username;
            Current.StopImpersonating();
            var result = Package.AccountService.Login(username, request.Password, request.RememberMe);
            if (result == ELoginResult.AccountLocked) return ApiError(429, "account_locked");
            if (result == ELoginResult.AccountNotActivated) return ApiError(403, "account_not_activated");
            if (result != ELoginResult.Success) return ApiError(401, "invalid_credentials");
            return ApiSuccess(new { Authenticated = true, RefreshCsrfToken = true });
        }

        [HttpPost, Route("logout")]
        public ActionResult Logout()
        {
            Package.AccountService.Logout();
            Current.StopImpersonating();
            Session.Clear();
            Session.Abandon();
            return ApiSuccess(new { Authenticated = false, RefreshCsrfToken = true });
        }

        [HttpGet, Route("me")]
        public ActionResult Me()
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var user = Package.UserService.Find(id);
            if (user == null) return ApiError(404, "account_not_found");
            var info = Package.UserInfosRepository.Find(e => e.UserId == id).FirstOrDefault();
            var membership = Package.MembershipService.GetActiveUserMembership(id);
            return ApiSuccess(new
            {
                user.FirstName, user.LastName, user.FullName, user.EmailAddress, user.PhoneNumber,
                BirthDate = user.BirthDate.ToString("yyyy-MM-dd"), user.Gender,
                AllowMarketingEmails = !user.IsUnsubscribed,
                BirthDetails = info == null ? null : new { info.TimeOfBirth, info.BirthTimeZoneId },
                Calculation = ReadOwnCalculationRequest(id),
                Membership = membership == null ? null : new
                {
                    membership.StartsOn, membership.EndsOn, membership.IsActive,
                    Paid = membership.IsAuthorisedToViewPaidContent(), Unlimited = membership.IsUnlimited(),
                    membership.ComplementaryPersonalChartReadingCount, membership.ComplementaryPredictionsReadingCount,
                    membership.ComplementaryCompatibilityReadingCount, membership.ComplementaryKarmicReadingCount
                }
            });
        }

        [HttpPost, Route("update")]
        public ActionResult Update(MobileAccountUpdateRequest request)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ValidInput(request) || !ValidInput(request.Settings)) return ApiError(400, "invalid_account_request");
            var user = Package.UserService.Find(id);
            if (user == null) return ApiError(404, "account_not_found");
            if (Package.UsersRepository.Find(e => e.EmailAddress == request.EmailAddress && e.Id != id).Any())
                return ApiError(409, "email_unavailable");
            // Update only these fields on the authenticated account's loaded entity.
            // No caller-supplied user IDs, roles, passwords or membership fields.
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.FullName = request.FirstName + " " + request.LastName;
            user.EmailAddress = request.EmailAddress;
            user.PhoneNumber = request.PhoneNumber;
            user.BirthDate = request.Settings.Person.DateOfBirth.Value.Date;
            user.Gender = request.Settings.Person.Gender;
            user.IsUnsubscribed = !request.AllowMarketingEmails;
            Package.UsersRepository.Update(user);
            var info = Package.UserService.GetOrCreateUserInfo(id);
            info.TimeOfBirth = request.Settings.Person.TimeOfBirth;
            info.BirthTimeZoneId = request.Settings.Person.BirthTimeZoneId;
            info.CalculationMethod = request.Settings.CalculationMethod;
            info.CalculatorType = request.Settings.CalculatorType;
            info.HousesDisplay = request.Settings.HousesDisplay;
            info.InvertDailyAndHourlyKiForSouthernHemisphere = request.Settings.InvertDailyAndHourlyKiForSouthernHemisphere;
            Package.UserInfosRepository.Update(info);
            SavePreferences(id, request.Settings);
            var contact = Package.ContactService.Find(request.EmailAddress);
            if (contact != null)
            {
                contact.IsUnsubscribed = !request.AllowMarketingEmails;
                Package.ContactsRepository.Update(contact);
            }
            return ApiSuccess(new { Updated = true });
        }

        private void SavePreferences(int id, MobileCalculationRequest settings)
        {
            var users = Package.UserService;
            users.UpdateUserPreference(id, Constants.SessionConstants.UserTimeZone, settings.UserTimeZoneId);
            users.UpdateUserPreference(id, Constants.SessionConstants.UserCalculationMethod, settings.CalculationMethod);
            users.UpdateUserPreference(id, Constants.SessionConstants.DefaultCalculatorType, settings.CalculatorType);
            users.UpdateUserPreference(id, Constants.SessionConstants.UserHousesDisplay, settings.HousesDisplay);
            users.UpdateUserPreference(id, Constants.SessionConstants.BirthTimeIsKnown, settings.Person.TimeOfBirthKnown);
            users.UpdateUserPreference(id, Constants.SessionConstants.InvertDailyAndHourlyKiForSouthernHemisphere, settings.InvertDailyAndHourlyKiForSouthernHemisphere);
            users.UpdateUserPreference(id, Constants.SessionConstants.InvertDailyAndHourlyCycleKiForSouthernHemisphere, settings.InvertDailyAndHourlyCycleKiForSouthernHemisphere);
        }

        [HttpPost, Route("preferences")]
        public ActionResult Preferences(MobileCalculationRequest request)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_preferences");
            SavePreferences(id, request);
            return ApiSuccess(new { Updated = true });
        }

        [HttpPost, Route("register")]
        public ActionResult Register(UserAccount.RegisterModel model, string captchaToken)
        {
            if (!Request.IsSecureConnection && !Request.IsLocal) return ApiError(400, "https_required");
            if (!ValidInput(model)) return ApiError(400, "invalid_registration_request");
            if (string.IsNullOrWhiteSpace(captchaToken) || !_captcha.Validate(captchaToken)) return ApiError(400, "invalid_captcha");
            // Register owns account creation, confirmation mail, roles and OTP.
            var result = Package.AccountService.Register(model);
            if (!result.IsSuccess) return ApiError(422, "registration_failed");
            return ApiSuccess(new { RequiresActivation = true, ActivationId = (result.Data as UserOTP)?.UniqueIdentifier });
        }

        [HttpPost, Route("activate")]
        public ActionResult Activate(string username, string token)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token) || username.Length > 200 || token.Length > 2000)
                return ApiError(400, "invalid_activation_request");
            var result = Package.AccountService.ActivateAccount(username, token);
            if (result.Result != EActivateAccountResult.Success && result.Result != EActivateAccountResult.AlreadyActivated)
                return ApiError(400, "invalid_activation_token");
            if (result.Result == EActivateAccountResult.Success)
            {
                Package.MembershipService.CreateFreeMembership(result.User.Id);
                Package.MembershipService.ScheduleRemindersForUser(result.User.Id);
            }
            return ApiSuccess(new { Activated = true });
        }

        [HttpPost, Route("activation-code")]
        public ActionResult VerifyActivationCode(MobileActivationRequest request, string captchaToken)
        {
            if (!ValidInput(request) || request.ActivationId == Guid.Empty) return ApiError(400, "invalid_activation_request");
            if (string.IsNullOrWhiteSpace(captchaToken) || !_captcha.Validate(captchaToken)) return ApiError(400, "invalid_captcha");
            var otp = Package.AccountService.GetAccountActivationOTP(request.ActivationId);
            if (otp == null || otp.VerifiedOn.HasValue) return ApiError(400, "invalid_activation_code");
            try
            {
                Package.AccountService.VerifyCode(otp.UserId, request.Digit1, request.Digit2, request.Digit3, request.Digit4, request.Digit5, request.Digit6);
            }
            catch (Exception) { return ApiError(400, "invalid_activation_code"); }
            var activated = Package.AccountService.ActivateAccount(otp.UserId);
            if (activated.Result != EActivateAccountResult.Success && activated.Result != EActivateAccountResult.AlreadyActivated)
                return ApiError(400, "activation_failed");
            if (activated.Result == EActivateAccountResult.Success)
                Package.MembershipService.ScheduleRemindersForUser(otp.UserId);
            return ApiSuccess(new { Activated = true });
        }

        [HttpPost, Route("resend-activation")]
        public ActionResult ResendActivation()
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            Package.AccountService.ResendActivationCode(id);
            return ApiSuccess(new { Sent = true });
        }

        [HttpPost, Route("password/reset-request")]
        public ActionResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model, string captchaToken)
        {
            if (!ValidInput(model)) return ApiError(400, "invalid_password_reset_request");
            if (string.IsNullOrWhiteSpace(captchaToken) || !_captcha.Validate(captchaToken)) return ApiError(400, "invalid_captcha");
            Package.AccountService.PasswordResetRequest(model);
            // Never expose the service's reset token or whether an email exists.
            return ApiSuccess(new { Accepted = true });
        }

        [HttpPost, Route("password/reset")]
        public ActionResult ResetPassword(UserAccount.ResetPasswordModel model)
        {
            if (!Request.IsSecureConnection && !Request.IsLocal) return ApiError(400, "https_required");
            if (!ValidInput(model)) return ApiError(400, "invalid_password_reset_request");
            var result = Package.AccountService.ResetPassword(model);
            return result.IsSuccess ? (ActionResult)ApiSuccess(new { Updated = true }) : ApiError(400, "password_reset_failed");
        }

        [HttpPost, Route("password/change")]
        public ActionResult ChangePassword(UserAccount.LocalPasswordModel model)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!Request.IsSecureConnection && !Request.IsLocal) return ApiError(400, "https_required");
            if (Current.IsImpersonating()) return ApiError(403, "impersonation_not_allowed");
            if (!ValidInput(model)) return ApiError(400, "invalid_password_request");
            return Package.AccountService.UpdatePassword(model).IsSuccess
                ? (ActionResult)ApiSuccess(new { Updated = true }) : ApiError(400, "password_change_failed");
        }

        [HttpPost, Route("delete")]
        public ActionResult Delete(MobileDeleteAccountRequest request)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!Request.IsSecureConnection && !Request.IsLocal) return ApiError(400, "https_required");
            if (Current.IsImpersonating()) return ApiError(403, "impersonation_not_allowed");
            if (!ValidInput(request) || !request.Confirm) return ApiError(400, "confirmation_required");
            var user = Package.UserService.Find(id);
            if (user == null || Package.AccountService.Login(user.Username, request.Password, false) != ELoginResult.Success)
                return ApiError(401, "invalid_credentials");
            if (!Package.AccountService.DeleteAccount(id).IsSuccess) return ApiError(422, "account_delete_failed");
            Session.Clear();
            Session.Abandon();
            return ApiSuccess(new { Deleted = true });
        }
    }
}
