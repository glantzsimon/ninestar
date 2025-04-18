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
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Linq;
using System.Web.Security;

namespace K9.WebApplication.Services
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IAccountMailerService _accountMailerService;
        private readonly IRepository<UserOTP> _otpRepository;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        private readonly IMembershipService _membershipService;

        public AccountService(INineStarKiBasePackage package, IRepository<User> userRepository, IOptions<WebsiteConfiguration> config, IMailer mailer, IAuthentication authentication, ILogger logger, IRoles roles, Services.IAccountMailerService accountMailerService, IRepository<UserOTP> otpRepository, IUserService userService,
            IContactService contactService,
            IMembershipService membershipService) : base(package)
        {
            _accountMailerService = accountMailerService;
            _otpRepository = otpRepository;
            _userService = userService;
            _contactService = contactService;
            _membershipService = membershipService;
        }

        public ELoginResult Login(string username, string password, bool isRemember)
        {
            if (My.Authentication.Login(username, password, isRemember))
            {
                return ELoginResult.Success;
            }
            if (My.Authentication.IsAccountLockedOut(username, 10, TimeSpan.FromDays(1)))
            {
                return ELoginResult.AccountLocked;
            }
            if (!My.Authentication.UserExists(username))
            {
                return ELoginResult.Fail;
            }
            if (!My.Authentication.IsConfirmed(username))
            {
                return ELoginResult.AccountNotActivated;
            }
            return ELoginResult.Fail;
        }

        public ELoginResult Login(int userId)
        {
            var user = My.UsersRepository.Find(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!My.Authentication.IsConfirmed(user.Username))
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
                My.Logger.Error($"AccountController => Login => Error: {e.GetFullErrorMessage()}");
            }

            return ELoginResult.Fail;
        }

        public ServiceResult Register(UserAccount.RegisterModel model)
        {
            var result = new ServiceResult();

            if (My.UsersRepository.Exists(u => u.Username == model.UserName))
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "RegisterModel.UserName",
                    ErrorMessage = Dictionary.UsernameIsUnavailableError
                });
            }
            if (My.UsersRepository.Exists(u => u.EmailAddress == model.EmailAddress))
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
                    My.Authentication.CreateUserAndAccount(model.UserName, model.Password,
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
                    My.Logger.Log(LogLevel.Error, $"AccountService => Register => CreatUserAndAccount => {ex.GetFullErrorMessage()}");

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
                    My.Roles.AddUserToRole(model.UserName, RoleNames.DefaultUsers);
                }
                catch (Exception ex)
                {
                    My.Logger.Log(LogLevel.Error, $"AccountService => Register => AddUserToRole => {ex.GetFullErrorMessage()}");

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
                    user = My.UsersRepository.Find(e => e.Username == model.UserName).FirstOrDefault();
                    otp = CreateAccountActivationOTP(user.Id);
                }
                catch (Exception e)
                {
                    My.Logger.Log(LogLevel.Error, $"AccountService => Register => CreateAccountActivationOTP => {e.GetFullErrorMessage()}");

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
                    My.Logger.Log(LogLevel.Error, $"AccountService => Register => GetOrCreateContact => {e.GetFullErrorMessage()}");

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
                    _accountMailerService.SendActivationEmailToUser(model, otp.SixDigitCode);
                }
                catch (Exception e)
                {
                    My.Logger.Log(LogLevel.Error, $"AccountService => Register => SendActivationEmail => {e.GetFullErrorMessage()}");

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

            if (My.UsersRepository.Exists(u => u.Username == model.UserName))
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
                My.UsersRepository.Create(newUser);
                My.Roles.AddUserToRole(model.UserName, RoleNames.DefaultUsers);
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
            var user = My.UsersRepository.Find(userId);

            if (user == null || My.Authentication.CurrentUserName != user.Username)
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
                    My.Authentication.Logout();
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
                if (My.Authentication.ChangePassword(My.Authentication.CurrentUserName, model.OldPassword, model.NewPassword))
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
                My.Logger.Error(ex.GetFullErrorMessage());
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
            var user = My.UsersRepository.Find(u => u.EmailAddress == model.EmailAddress).FirstOrDefault();

            if (user != null)
            {
                try
                {
                    model.UserName = user.Username;
                    var token = My.Authentication.GeneratePasswordResetToken(user.Username);
                    _accountMailerService.SendPasswordResetEmailToUser(model, token);
                    result.IsSuccess = true;
                    result.Data = token;
                }
                catch (Exception ex)
                {
                    My.Logger.Error(ex.GetFullErrorMessage());
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = ex.GetFullErrorMessage()
                    });
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
            var userId = My.Authentication.GetUserIdFromPasswordResetToken(token);
            var confirmUserId = My.Authentication.GetUserId(username);
            return userId == confirmUserId;
        }

        public ServiceResult ResetPassword(UserAccount.ResetPasswordModel model)
        {
            var result = new ServiceResult();

            try
            {
                My.Authentication.ResetPassword(model.Token, model.NewPassword);
                My.Authentication.Login(model.UserName, model.NewPassword);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
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
            var user = My.UsersRepository.Find(u => u.Id == userId).FirstOrDefault();
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
            var user = My.UsersRepository.Find(u => u.Username == username).FirstOrDefault();
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
                if (My.Authentication.IsConfirmed(user.Username))
                {
                    My.Logger.Error("Account already activated for user '{0}'.", user.Username);
                    result.Result = EActivateAccountResult.AlreadyActivated;
                    return result;
                }

                if (string.IsNullOrEmpty(token))
                {
                    token = GetAccountActivationToken(user.Id);
                }
                if (!My.Authentication.ConfirmAccount(user.Username, token))
                {
                    My.Logger.Error("ActivateAccount failed as user '{0}' was not found.", user.Username);
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
            var user = My.UsersRepository.Find(u => u.Id == userId).FirstOrDefault();
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
            var user = My.UsersRepository.Find(u => u.Username == username).FirstOrDefault();
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
                if (My.Authentication.IsConfirmed(user.Username))
                {
                    My.Logger.Error("Account already activated for user '{0}'.", user.Username);
                    result.Result = EActivateAccountResult.AlreadyActivated;
                    return result;
                }

                var token = GetAccountActivationToken(user.Id);
                if (!My.Authentication.ConfirmAccount(user.Username, token))
                {
                    My.Logger.Error("ActivateAccount failed as user '{0}' was not found.", user.Username);
                    result.Result = EActivateAccountResult.Fail;
                    return result;
                }

                result.Token = token;
                result.Result = EActivateAccountResult.Success;
            }

            return result;
        }

        public MyAccountViewModel GetAccount(int userId)
        {
            var user = _userService.Find(userId);
            return new MyAccountViewModel
            {
                User = user,
                UserInfo = _userService.GetOrCreateUserInfo(userId),
                Membership = _membershipService.GetActiveUserMembership(userId),
                AllowMarketingEmails = !user.IsUnsubscribed,
                Consultations = _userService.GetPendingConsultations(user.Id)
            };
        }

        public void Logout()
        {
            My.Authentication.Logout();
        }

        public string GetAccountActivationToken(int userId)
        {
            string sql = "SELECT ConfirmationToken FROM webpages_Membership " + $"WHERE UserId = {userId}";
            return My.UsersRepository.CustomQuery<string>(sql).FirstOrDefault();
        }

        public void ResendActivationCode(int userId)
        {
            var user = _userService.Find(userId);
            var otp = CreateAccountActivationOTP(userId, true);
            _accountMailerService.SendActivationEmailToUser(user, otp.SixDigitCode);
        }

        public UserOTP CreateAccountActivationOTP(int userId, bool recreate = false)
        {
            if (recreate)
            {
                var existing = _otpRepository.Find(e => e.UserId == userId).ToList();
                foreach (var userOtp in existing)
                {
                    userOtp.IsDeleted = true;
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
            var otp = _otpRepository.Find(e => e.UserId == userId && e.SixDigitCode == sixDigitCode && !e.IsDeleted).FirstOrDefault();

            if (otp == null)
            {
                My.Logger.Error($"Account Service => VerifyCode => Invalid OTP. UserId: {userId}, code:{sixDigitCode}");
                throw new Exception("Invalid OTP");
            }

            if (otp.VerifiedOn.HasValue)
            {
                My.Logger.Error($"Account Service => VerifyCode => OTP already verified. UserId: {userId}, code:{sixDigitCode}");
                throw new Exception("This six digit code has already been used to verify your account. Please log in.");
            }

            otp.VerifiedOn = DateTime.Now;
            _otpRepository.Update(otp);
        }

        public void UnverifyCode(int userId, int digit1, int digit2, int digit3, int digit4, int digit5, int digit6)
        {
            var sixDigitCode = int.Parse($"{digit1}{digit2}{digit3}{digit4}{digit5}{digit6}");
            var otp = _otpRepository.Find(e => e.UserId == userId && e.SixDigitCode == sixDigitCode && !e.IsDeleted).FirstOrDefault();

            if (otp == null)
            {
                My.Logger.Error($"Account Service => UnverifyCode => Invalid OTP. UserId: {userId}, code:{sixDigitCode}");
            }

            if (!otp.VerifiedOn.HasValue)
            {
                My.Logger.Error($"Account Service => UnverifyCode => OTP not verified. UserId: {userId}, code:{sixDigitCode}");
            }

            otp.VerifiedOn = null;
            _otpRepository.Update(otp);
        }

        public UserOTP GetAccountActivationOTP(Guid uniqueIdentifier)
        {
            return _otpRepository.Find(e => e.UniqueIdentifier == uniqueIdentifier).FirstOrDefault();
        }

        public UserOTP GetOTPForUser(int userId)
        {
            return _otpRepository.Find(e => e.UserId == userId && !e.VerifiedOn.HasValue).FirstOrDefault();
        }

        public string GetPasswordResetLink(UserAccount.PasswordResetRequestModel model, string token)
        {
            return My.UrlHelper.AbsoluteAction("ResetPassword", "Account", new { userName = model.UserName, token });
        }

        public string GetActivationLink(UserAccount.RegisterModel model, string token)
        {
            return My.UrlHelper.AbsoluteAction("ActivateAccount", "Account", new { userName = model.UserName, token });
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
            var user = My.UsersRepository.Find(e => e.Username == username).FirstOrDefault();
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