using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace K9.WebApplication.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMailer _mailer;
        private readonly IAuthentication _authentication;
        private readonly ILogger _logger;
        private readonly IRoles _roles;
        private readonly Services.IAccountMailerService _accountMailerService;
        private readonly IRepository<UserOTP> _otpRepository;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly WebsiteConfiguration _config;
        private UrlHelper _urlHelper;

        public UrlHelper UrlHelper
        {
            get => _urlHelper;
            set => _urlHelper = value;
        }

        public AccountService(IRepository<User> userRepository, IOptions<WebsiteConfiguration> config, IMailer mailer, IAuthentication authentication, ILogger logger, IRoles roles, Services.IAccountMailerService accountMailerService, IRepository<UserOTP> otpRepository, IUserService userService,
            IContactService contactService)
        {
            _userRepository = userRepository;
            _mailer = mailer;
            _authentication = authentication;
            _logger = logger;
            _roles = roles;
            _accountMailerService = accountMailerService;
            _otpRepository = otpRepository;
            _userService = userService;
            _contactService = contactService;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public ELoginResult Login(string username, string password, bool isRemember)
        {
            if (_authentication.Login(username, password, isRemember))
            {
                return ELoginResult.Success;
            }
            if (_authentication.IsAccountLockedOut(username, 10, TimeSpan.FromDays(1)))
            {
                return ELoginResult.AccountLocked;
            }
            if (!_authentication.IsConfirmed(username))
            {
                return ELoginResult.AccountNotActivated;
            }
            return ELoginResult.Fail;
        }

        public ELoginResult Login(int userId)
        {
            var user = _userRepository.Find(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!_authentication.IsConfirmed(user.Username))
            {
                return ELoginResult.AccountNotActivated;
            }

            try
            {
                FormsAuthentication.SetAuthCookie(user.Username, true);
                return ELoginResult.Success;
            }
            catch (Exception e)
            {
                _logger.Error($"AccountController => Login => Error: {e.GetFullErrorMessage()}");
            }

            return ELoginResult.Fail;
        }

        public ServiceResult Register(UserAccount.RegisterModel model)
        {
            var result = new ServiceResult();

            if (_userRepository.Exists(u => u.Username == model.UserName))
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "RegisterModel.UserName",
                    ErrorMessage = Dictionary.UsernameIsUnavailableError
                });
            }
            if (_userRepository.Exists(u => u.EmailAddress == model.EmailAddress))
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "RegisterModel.EmailAddress",
                    ErrorMessage = Dictionary.EmailIsUnavailableError
                });
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "RegisterModel.Password",
                    ErrorMessage = Dictionary.InvalidPasswordEnteredError
                });
            }
            if (model.Password != model.ConfirmPassword)
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "RegisterModel.ConfirmPassword",
                    ErrorMessage = Dictionary.PasswordMatchError
                });
            }

            if (!result.Errors.Any())
            {
                var newUser = new
                {
                    model.EmailAddress,
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber,
                    model.BirthDate,
                    model.Gender,
                    Name = Guid.NewGuid(),
                    FullName = $"{model.FirstName} {model.LastName}",
                    IsUnsubscribed = false,
                    IsSystemStandard = false,
                    IsDeleted = false,
                    CreatedBy = SystemUser.System,
                    CreatedOn = DateTime.Now,
                    LastUpdatedBy = SystemUser.System,
                    LastUpdatedOn = DateTime.Now,
                    IsOAuth = false
                };

                try
                {
                    _authentication.CreateUserAndAccount(model.UserName, model.Password,
                        newUser, true);
                }
                catch (MembershipCreateUserException e)
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = ErrorCodeToString(e.StatusCode)
                    });
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, $"AccountService => Register => CreatUserAndAccount => {ex.GetFullErrorMessage()}");

                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = Globalisation.Dictionary.ErrorCreatingUserAccount,
                        Exception = ex,
                        Data = newUser
                    });
                    return result;
                }

                try
                {
                    _roles.AddUserToRole(model.UserName, RoleNames.DefaultUsers);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, $"AccountService => Register => AddUserToRole => {ex.GetFullErrorMessage()}");
                    
                    TryDeleteUserAccount(model.UserName);

                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = Globalisation.Dictionary.ErrorCreatingUserAccount,
                        Exception = ex,
                        Data = newUser
                    });
                    return result;
                }

                UserOTP otp = null;
                User user = null;
                try
                {
                    user = _userRepository.Find(e => e.Username == model.UserName).FirstOrDefault();
                    otp = CreateAccountActivationOTP(user.Id);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, $"AccountService => Register => CreateAccountActivationOTP => {e.GetFullErrorMessage()}");

                    TryDeleteUserAccount(model.UserName);

                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = Globalisation.Dictionary.ErrorCreatingUserAccount,
                        Exception = e,
                        Data = newUser
                    });
                    return result;
                }

                try
                {
                    _contactService.GetOrCreateContact("", user.FullName, user.EmailAddress, user.PhoneNumber, user.Id);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, $"AccountService => Register => GetOrCreateContact => {e.GetFullErrorMessage()}");

                    TryDeleteUserAccount(model.UserName);

                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = Globalisation.Dictionary.ErrorCreatingUserAccount,
                        Exception = e,
                        Data = newUser
                    });
                    return result;
                }

                try
                {
                    _accountMailerService.SendActivationEmail(model, otp.SixDigitCode);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, $"AccountService => Register => SendActivationEmail => {e.GetFullErrorMessage()}");

                    TryDeleteUserAccount(model.UserName);

                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = Globalisation.Dictionary.ErrorCreatingUserAccount,
                        Exception = e,
                        Data = newUser
                    });
                    return result;
                }

                result.IsSuccess = true;
                result.Data = otp;
                return result;
            }

            return result;
        }

        public ServiceResult RegisterOrLoginAuth(UserAccount.RegisterModel model)
        {
            var result = new ServiceResult();

            if (_userRepository.Exists(u => u.Username == model.UserName))
            {
                result.IsSuccess = true;
                FormsAuthentication.SetAuthCookie(model.UserName, false);
                return result;
            }

            var newUser = new User
            {
                Username = model.UserName,
                EmailAddress = model.EmailAddress,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = model.BirthDate,
                FullName = $"{model.FirstName} {model.LastName}",
                IsUnsubscribed = false,
                IsSystemStandard = false,
                IsOAuth = true,
                AccountActivated = true,
                IsDeleted = false,
                CreatedBy = SystemUser.System,
                CreatedOn = DateTime.Now,
                LastUpdatedBy = SystemUser.System,
                LastUpdatedOn = DateTime.Now
            };

            try
            {
                _userRepository.Create(newUser);
                _roles.AddUserToRole(model.UserName, RoleNames.DefaultUsers);
                result.IsSuccess = true;
                FormsAuthentication.SetAuthCookie(newUser.Username, false);
                return result;
            }
            catch (MembershipCreateUserException e)
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "",
                    ErrorMessage = ErrorCodeToString(e.StatusCode)
                });
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "",
                    ErrorMessage = ex.Message,
                    Exception = ex,
                    Data = newUser
                });
            }

            return result;
        }

        public ServiceResult DeleteAccount(int userId)
        {
            var result = new ServiceResult();
            var user = _userRepository.Find(userId);

            if (user == null || _authentication.CurrentUserName != user.Username)
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "Username",
                    ErrorMessage = Dictionary.UserNotFoundError
                });
            }

            if (!result.Errors.Any())
            {
                try
                {
                    _authentication.Logout();
                    _userService.DeleteUser(userId);
                    result.IsSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = ex.Message,
                        Exception = ex,
                        Data = user
                    });
                }
            }

            return result;
        }

        public ServiceResult UpdatePassword(UserAccount.LocalPasswordModel model)
        {
            var result = new ServiceResult();
            try
            {
                if (_authentication.ChangePassword(_authentication.CurrentUserName, model.OldPassword, model.NewPassword))
                {
                    result.IsSuccess = true;
                }
                else
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = Dictionary.CurrentPasswordCorrectNewInvalidError
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetFullErrorMessage());
                result.Errors.Add(new ServiceError
                {
                    FieldName = "",
                    ErrorMessage = Dictionary.UpdatePaswordError
                });
            }
            return result;
        }

        public ServiceResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model)
        {
            var result = new ServiceResult();
            var user = _userRepository.Find(u => u.EmailAddress == model.EmailAddress).FirstOrDefault();

            if (user != null)
            {
                try
                {
                    model.UserName = user.Username;
                    var token = _authentication.GeneratePasswordResetToken(user.Username);
                    _accountMailerService.SendPasswordResetEmail(model, token);
                    result.IsSuccess = true;
                    result.Data = token;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.GetFullErrorMessage());
                }
            }
            else
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = nameof(UserAccount.PasswordResetRequestModel.UserName),
                    ErrorMessage = Dictionary.InvalidUsernameError
                });
            }

            return result;
        }

        public bool ConfirmUserFromToken(string username, string token)
        {
            var userId = _authentication.GetUserIdFromPasswordResetToken(token);
            var confirmUserId = _authentication.GetUserId(username);
            return userId == confirmUserId;
        }

        public ServiceResult ResetPassword(UserAccount.ResetPasswordModel model)
        {
            var result = new ServiceResult();

            try
            {
                _authentication.ResetPassword(model.Token, model.NewPassword);
                _authentication.Login(model.UserName, model.NewPassword);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetFullErrorMessage());
                result.Errors.Add(new ServiceError
                {
                    FieldName = "",
                    ErrorMessage = Dictionary.PasswordResetFailError
                });
            }
            return result;
        }

        public ActivateAccountResult ActivateAccount(int userId, string token = "")
        {
            var user = _userRepository.Find(u => u.Id == userId).FirstOrDefault();
            if (user != null)
            {
                return ActivateAccount(user, token);
            }
            return new ActivateAccountResult
            {
                Result = EActivateAccountResult.Fail
            };
        }

        public ActivateAccountResult ActivateAccount(string username, string token = "")
        {
            var user = _userRepository.Find(u => u.Username == username).FirstOrDefault();
            if (user != null)
            {
                return ActivateAccount(user, token);
            }
            return new ActivateAccountResult
            {
                Result = EActivateAccountResult.Fail
            };
        }

        public ActivateAccountResult ActivateAccount(User user, string token = "")
        {
            var result = new ActivateAccountResult
            {
                User = user
            };

            if (user != null)
            {
                if (_authentication.IsConfirmed(user.Username))
                {
                    _logger.Error("Account already activated for user '{0}'.", user.Username);
                    result.Result = EActivateAccountResult.AlreadyActivated;
                    return result;
                }

                if (string.IsNullOrEmpty(token))
                {
                    token = GetAccountActivationToken(user.Id);
                }
                if (!_authentication.ConfirmAccount(user.Username, token))
                {
                    _logger.Error("ActivateAccount failed as user '{0}' was not found.", user.Username);
                    result.Result = EActivateAccountResult.Fail;
                    return result;
                }

                result.Token = token;
                result.Result = EActivateAccountResult.Success;
            }

            return result;
        }

        public ActivateAccountResult ActivateAccount(int userId)
        {
            var user = _userRepository.Find(u => u.Id == userId).FirstOrDefault();
            if (user != null)
            {
                return ActivateAccount(user);
            }
            return new ActivateAccountResult
            {
                Result = EActivateAccountResult.Fail
            };
        }

        public ActivateAccountResult ActivateAccount(string username)
        {
            var user = _userRepository.Find(u => u.Username == username).FirstOrDefault();
            if (user != null)
            {
                return ActivateAccount(user);
            }
            return new ActivateAccountResult
            {
                Result = EActivateAccountResult.Fail
            };
        }

        public ActivateAccountResult ActivateAccount(User user)
        {
            var result = new ActivateAccountResult
            {
                User = user
            };

            if (user != null)
            {
                if (_authentication.IsConfirmed(user.Username))
                {
                    _logger.Error("Account already activated for user '{0}'.", user.Username);
                    result.Result = EActivateAccountResult.AlreadyActivated;
                    return result;
                }

                var token = GetAccountActivationToken(user.Id);
                if (!_authentication.ConfirmAccount(user.Username, token))
                {
                    _logger.Error("ActivateAccount failed as user '{0}' was not found.", user.Username);
                    result.Result = EActivateAccountResult.Fail;
                    return result;
                }

                result.Token = token;
                result.Result = EActivateAccountResult.Success;
            }

            return result;
        }

        public void Logout()
        {
            _authentication.Logout();
        }

        public string GetAccountActivationToken(int userId)
        {
            string sql = "SELECT ConfirmationToken FROM webpages_Membership " + $"WHERE UserId = {userId}";
            return _userRepository.CustomQuery<string>(sql).FirstOrDefault();
        }

        public void ResentActivationCode(int userId)
        {
            var user = _userService.Find(userId);
            var otp = CreateAccountActivationOTP(userId, true);
            _accountMailerService.SendActivationEmail(user, otp.SixDigitCode);
        }

        public UserOTP CreateAccountActivationOTP(int userId, bool recreate = false)
        {
            if (recreate)
            {
                var existing = _otpRepository.Find(e => e.UserId == userId).Where(e => e.VerifiedOn.HasValue);
                foreach (var userOtp in existing)
                {
                    userOtp.VerifiedOn = DateTime.UtcNow;
                    _otpRepository.Update(userOtp);
                }
            }

            var otp = new UserOTP
            {
                UniqueIdentifier = Guid.NewGuid(),
                UserId = userId
            };

            _otpRepository.Create(otp);

            return otp;
        }

        public void VerifyCode(int userId, int digit1, int digit2, int digit3, int digit4, int digit5, int digit6)
        {
            var sixDigitCode = int.Parse($"{digit1}{digit2}{digit3}{digit4}{digit5}{digit6}");
            var otp = _otpRepository.Find(e => e.UserId == userId && e.SixDigitCode == sixDigitCode).FirstOrDefault();

            if (otp == null)
            {
                _logger.Error($"Account Service => VerifyCode => Invalid OTP. UserId: {userId}, code:{sixDigitCode}");
                throw new Exception("Invalid OTP");
            }

            if (otp.VerifiedOn.HasValue)
            {
                _logger.Error($"Account Service => VerifyCode => OTP already verified. UserId: {userId}, code:{sixDigitCode}");
                throw new Exception("This six digit code has already been used to verify your account. Please log in.");
            }

            otp.VerifiedOn = DateTime.UtcNow;
            _otpRepository.Update(otp);
        }

        public UserOTP GetAccountActivationOTP(Guid uniqueIdentifier)
        {
            return _otpRepository.Find(e => e.UniqueIdentifier == uniqueIdentifier).FirstOrDefault();
        }

        public string GetPasswordResetLink(UserAccount.PasswordResetRequestModel model, string token)
        {
            return _urlHelper.AbsoluteAction("ResetPassword", "Account", new { userName = model.UserName, token });
        }

        public string GetActivationLink(UserAccount.RegisterModel model, string token)
        {
            return _urlHelper.AbsoluteAction("ActivateAccount", "Account", new { userName = model.UserName, token });
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return Dictionary.UsernameExistsError;

                case MembershipCreateStatus.DuplicateEmail:
                    return Dictionary.UserNameEmailExistsError;

                case MembershipCreateStatus.InvalidPassword:
                    return Dictionary.InvalidPasswordEnteredError;

                case MembershipCreateStatus.InvalidEmail:
                    return Dictionary.InvalidEmailError;

                case MembershipCreateStatus.InvalidAnswer:
                    return Dictionary.InvalidPasswordRetreivalError;

                case MembershipCreateStatus.InvalidQuestion:
                    return Dictionary.InvalidRetrievalQuestionError;

                case MembershipCreateStatus.InvalidUserName:
                    return Dictionary.InvalidUsernameError;

                case MembershipCreateStatus.ProviderError:
                    return Dictionary.ProviderError;

                case MembershipCreateStatus.UserRejected:
                    return Dictionary.UserRejectedError;

                default:
                    return Dictionary.DefaultAuthError;
            }
        }

        private void TryDeleteUserAccount(string username)
        {
            var user = _userRepository.Find(e => e.Username == username).FirstOrDefault();
            if (user != null)
            {
                try
                {
                    _userService.DeleteUser(user.Id);
                }
                catch (Exception e)
                {
                }
            }
        }

    }
}