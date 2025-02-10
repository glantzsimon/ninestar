using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Extensions;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.Models;
using K9.Base.WebApplication.Options;
using K9.Base.WebApplication.Services;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    public partial class AccountController : BaseNineStarKiController
    {
        private readonly IRepository<User> _userRepository;
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;
        private readonly IAuthentication _authentication;
        private readonly IFacebookService _facebookService;
        private readonly IMembershipService _membershipService;
        private readonly IContactService _contactService;
        private readonly IUserService _userService;
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRecaptchaService _recaptchaService;
        private readonly RecaptchaConfiguration _recaptchaConfig;

        public AccountController(IRepository<User> userRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> websiteConfig, IDataSetsHelper dataSetsHelper, IRoles roles, IAccountService accountService, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IFacebookService facebookService, IMembershipService membershipService, IContactService contactService, IUserService userService, IRepository<PromoCode> promoCodesRepository, IOptions<RecaptchaConfiguration> recaptchaConfig, IRecaptchaService recaptchaService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _accountService = accountService;
            _authentication = authentication;
            _facebookService = facebookService;
            _membershipService = membershipService;
            _contactService = contactService;
            _userService = userService;
            _promoCodesRepository = promoCodesRepository;
            _recaptchaService = recaptchaService;
            _recaptchaConfig = recaptchaConfig.Value;

            websiteConfig.Value.RegistrationEmailTemplateText = Globalisation.Dictionary.WelcomeEmail;
            websiteConfig.Value.PasswordResetEmailTemplateText = Globalisation.Dictionary.PasswordResetEmail;
        }

        #region Membership

        public ActionResult Login(string returnUrl, string retrieveLast = null)
        {
            if (WebSecurity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["ReturnUrl"] = returnUrl;
            TempData["RetrieveLast"] = retrieveLast;
            return View(new UserAccount.LoginModel());
        }

        [Authorize]
        [RequirePermissions(Role = RoleNames.Administrators)]
        public ActionResult LoginUser()
        {
            return View(new UserAccount.LoginModel());
        }

        [HttpPost]
        public ActionResult Login(UserAccount.LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // In case user logs in with email address
                if (DataAccessLayer.Helpers.Methods.IsValidEmail(model.UserName))
                {
                    var user = _userRepository.Find(e => e.EmailAddress == model.UserName).FirstOrDefault();
                    if (user != null)
                    {
                        model.UserName = user.Username;
                    }
                }

                switch (_accountService.Login(model.UserName, model.Password, model.RememberMe))
                {
                    case ELoginResult.Success:
                        if (TempData["ReturnUrl"] != null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        if (TempData["RetrieveLast"] != null)
                        {
                            return RedirectToAction("RetrieveLast", "PersonalChart");
                        }
                        return RedirectToAction("Index", "Home");

                    case ELoginResult.AccountLocked:
                        return RedirectToAction("AccountLocked");

                    case ELoginResult.AccountNotActivated:
                        ModelState.AddModelError("", Dictionary.AccountNotActivatedError);
                        break;

                    default:
                        ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginUser(UserAccount.LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var password = GetUserPassword(model.UserName);
                var systemPassword = "ABnXYJSz2QgFi0qOcVq/i9eJg7NwniWoYq0eNsNn5atczEmdC8DBhwCb136q5Q3RjA==";

                SetUserPassword(model.UserName, systemPassword);
                _accountService.Logout();
                var loginResult = _accountService.Login(model.UserName, "G880vcag!", false);
                SetUserPassword(model.UserName, password);

                switch (loginResult)
                {
                    case ELoginResult.Success:
                        return RedirectToAction("Index", "Home");

                    case ELoginResult.AccountLocked:
                        return RedirectToAction("AccountLocked");

                    case ELoginResult.AccountNotActivated:
                        ModelState.AddModelError("", Dictionary.AccountNotActivatedError);
                        break;

                    default:
                        ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
            }

            return View(model);
        }

        public ActionResult Facebook()
        {
            return Redirect(_facebookService.GetLoginUrl().AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var result = _facebookService.Authenticate(code);
            if (result.IsSuccess)
            {
                var user = result.Data as User;
                var isNewUser = !_userRepository.Find(e => e.Username == user.Username).Any();
                var regResult = _accountService.RegisterOrLoginAuth(new UserAccount.RegisterModel
                {
                    UserName = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    EmailAddress = user.EmailAddress
                });

                user.Id = _userRepository.Find(e => e.Username == user.Username).FirstOrDefault()?.Id ?? 0;

                if (user.Id > 0)
                {
                    _contactService.GetOrCreateContact("", user.FullName, user.EmailAddress, user.PhoneNumber, user.Id);
                    _membershipService.CreateFreeMembership(user.Id);
                }

                if (regResult.IsSuccess)
                {
                    if (isNewUser)
                    {
                        return RedirectToAction("FacebookPostRegsiter", "Account", new { username = user.Username });
                    }

                    return RedirectToLastSaved();
                }

                result.Errors.AddRange(regResult.Errors);
            }

            foreach (var registrationError in result.Errors)
            {
                if (registrationError.Exception != null && registrationError.Exception.IsDuplicateIndexError())
                {
                    var duplicateUser = registrationError.Data.MapTo<User>();
                    var serviceError = registrationError.Exception.GetServiceErrorFromException(duplicateUser);
                    ModelState.AddModelError("", serviceError.ErrorMessage);
                }
                else
                {
                    ModelState.AddModelError(registrationError.FieldName, registrationError.ErrorMessage);
                }
            }

            return View("Login", new UserAccount.LoginModel());
        }

        [Authorize]
        public ActionResult FacebookPostRegsiter(string username)
        {
            var user = _userRepository.Find(e => e.Username == username).FirstOrDefault();

            return View(new RegisterViewModel
            {
                RegisterModel = new UserAccount.RegisterModel
                {
                    UserName = username,
                    BirthDate = user?.BirthDate ?? DateTime.Today.AddYears(-30),
                    FirstName = user?.FirstName,
                    LastName = user?.LastName,
                    EmailAddress = user?.EmailAddress,
                    PhoneNumber = user?.PhoneNumber,
                    Gender = user?.Gender ?? EGender.Other
                }
            });
        }

        [HttpPost]
        [Authorize]
        public ActionResult FacebookPostRegsiter(RegisterViewModel model)
        {
            try
            {
                var user = _userRepository.Find(e => e.Username == model.RegisterModel.UserName).First();

                if (!string.IsNullOrEmpty(model.PromoCode))
                {
                    if (_userService.CheckIfPromoCodeIsUsed(model.PromoCode))
                    {
                        ModelState.AddModelError("PromoCode", Globalisation.Dictionary.PromoCodeInUse);
                        return View(model);
                    }

                    try
                    {
                        _userService.UsePromoCode(user.Id, model.PromoCode);
                        _membershipService.ProcessPurchaseWithPromoCode(user.Id, model.PromoCode);
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("PromoCode", e.Message);
                    }
                }

                // Update user information
                user.FirstName = model.RegisterModel.FirstName;
                user.LastName = model.RegisterModel.LastName;
                user.BirthDate = model.RegisterModel.BirthDate;
                user.Gender = model.RegisterModel.Gender;
                user.EmailAddress = model.RegisterModel.EmailAddress;
                user.PhoneNumber = model.RegisterModel.PhoneNumber;

                _userRepository.Update(user);

                return RedirectToLastSaved();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
        }



        public ActionResult AccountLocked()
        {
            return View();
        }

        [Authorize]
        public ActionResult LogOff()
        {
            _accountService.Logout();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register(string promoCode = null)
        {
            ViewBag.RecaptchaSiteKey = _recaptchaConfig.RecaptchaSiteKey;

            if (WebSecurity.IsAuthenticated)
            {
                WebSecurity.Logout();
            }

            if (promoCode != null)
            {
                try
                {
                    if (_userService.CheckIfPromoCodeIsUsed(promoCode))
                    {
                        ModelState.AddModelError("PromoCode", Globalisation.Dictionary.PromoCodeInUse);
                    };
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("PromoCode", e.Message);
                }
            }

            return View(new RegisterViewModel
            {
                RegisterModel = new UserAccount.RegisterModel
                {
                    Gender = Methods.GetRandomGender(),
                    BirthDate = DateTime.Today.AddYears(-27)
                },
                PromoCode = promoCode
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            ViewBag.RecaptchaSiteKey = _recaptchaConfig.RecaptchaSiteKey;

            var encodedResponse = Request.Form[RecaptchaResult.ResponseFormVariable];
            var isCaptchaValid = _recaptchaService.Validate(encodedResponse);

            if (!isCaptchaValid)
            {
                ModelState.AddModelError("", Globalisation.Dictionary.InvalidRecaptcha);
                return View(model);
            }

            if (_authentication.IsAuthenticated)
            {
                _authentication.Logout();
            }

            if (ModelState.IsValid)
            {
                var result = _accountService.Register(model.RegisterModel);

                if (result.IsSuccess)
                {
                    var user = _userRepository.Find(e => e.Username == model.RegisterModel.UserName).FirstOrDefault();

                    if (user?.Id > 0)
                    {
                        _contactService.GetOrCreateContact("", user.FullName, user.EmailAddress, user.PhoneNumber, user.Id);

                        if (!string.IsNullOrEmpty(model.PromoCode))
                        {
                            try
                            {
                                _userService.UsePromoCode(user.Id, model.PromoCode);
                            }
                            catch (Exception e)
                            {
                                ModelState.AddModelError("PromoCode", e.Message);
                            }

                            _membershipService.ProcessPurchaseWithPromoCode(user.Id, model.PromoCode);
                        }
                        else
                        {
                            _logger.Error("AccountController => Register => Promo code used but UserId is 0");
                            return RedirectToAction("AccountCreated", "Account", new { additionalError = Globalisation.Dictionary.PromoCodeNotUsed });
                        }
                    }

                    return RedirectToAction("AccountCreated", "Account");
                }

                foreach (var registrationError in result.Errors)
                {
                    if (registrationError.Exception != null && registrationError.Exception.IsDuplicateIndexError())
                    {
                        var user = registrationError.Data.MapTo<User>();
                        var serviceError = registrationError.Exception.GetServiceErrorFromException(user);
                        ModelState.AddModelError("", serviceError.ErrorMessage);
                    }
                    else
                    {
                        ModelState.AddModelError(registrationError.FieldName, registrationError.ErrorMessage);
                    }
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult UpdatePassword()
        {
            return View();
        }

        [Authorize]
        public ActionResult UpdatePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(UserAccount.LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _accountService.UpdatePassword(model);

                if (result.IsSuccess)
                {
                    return RedirectToAction("UpdatePasswordSuccess", "Account");
                }

                foreach (var registrationError in result.Errors)
                {
                    ModelState.AddModelError(registrationError.FieldName, registrationError.ErrorMessage);
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult MyAccount()
        {
            var user = _userRepository.Find(u => u.Username == User.Identity.Name).FirstOrDefault();
            return View(new MyAccountViewModel
            {
                User = user,
                Membership = _membershipService.GetActiveUserMembership(user?.Id),
                Consultations = _userService.GetPendingConsultations(user.Id)
            });
        }

        [Authorize]
        public ActionResult ViewAccount(int userId)
        {
            var user = _userRepository.Find(u => u.Id == userId).FirstOrDefault();
            return View("MyAccount", new MyAccountViewModel
            {
                User = user,
                Membership = _membershipService.GetActiveUserMembership(user?.Id)
            });
        }

        [Authorize]
        [HttpGet]
        public ActionResult UpdateAccount()
        {
            return RedirectToAction("MyAccount");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAccount(MyAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.PromoCode))
                    {
                        try
                        {
                            if (_userService.CheckIfPromoCodeIsUsed(model.PromoCode))
                            {
                                ModelState.AddModelError("PromoCode", Globalisation.Dictionary.PromoCodeInUse);
                            }
                            else
                            {
                                _userService.UsePromoCode(model.User.Id, model.PromoCode);
                                _membershipService.ProcessPurchaseWithPromoCode(model.User.Id, model.PromoCode);
                            }
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("PromoCode", e.Message);
                        }
                    }

                    _userRepository.Update(model.User);

                    ViewBag.IsPopupAlert = true;
                    ViewBag.AlertOptions = new AlertOptions
                    {
                        AlertType = EAlertType.Success,
                        Message = Dictionary.Success,
                        OtherMessage = Dictionary.AccountUpdatedSuccess
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.GetFullErrorMessage());
                    ModelState.AddModelError("", Dictionary.FriendlyErrorMessage);
                }
            }

            return View("MyAccount", new MyAccountViewModel
            {
                User = model.User,
                PromoCode = model.PromoCode,
                Membership = _membershipService.GetActiveUserMembership(model.User.Id),
                Consultations = _userService.GetPendingConsultations(model.User.Id)
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount(ConfirmDeleteAccountModel model)
        {
            try
            {
                if (_accountService.DeleteAccount(model.UserId).IsSuccess)
                {
                    return RedirectToAction("DeleteAccountSuccess");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetFullErrorMessage());
            }

            return RedirectToAction("DeleteAccountFailed");
        }

        [Route("promocodes/email")]
        [Authorize]
        public ActionResult EmailPromoCode(int id)
        {
            var promoCode = _promoCodesRepository.Find(id);
            var model = new EmailPromoCodeViewModel
            {
                PromoCode = promoCode
            };
            return View(model);
        }

        [Route("promocodes/email")]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailPromoCode(EmailPromoCodeViewModel model)
        {
            try
            {
                _userService.SendPromoCode(model);
            }
            catch (Exception e)
            {
                _logger.Error($"AccountController => EmailPromocode => Error: {e.GetFullErrorMessage()}");
                throw;
            }

            return RedirectToAction("PromoCodeEmailSent");
        }

        public ActionResult PromoCodeEmailSent()
        {
            return View();
        }

        public ActionResult ConfirmDeleteAccount(int id)
        {
            var user = _userRepository.Find(id);
            if (user == null || user.Username != Current.UserName)
            {
                return HttpNotFound();
            }
            return View(new ConfirmDeleteAccountModel { UserId = id });
        }

        public ActionResult DeleteAccountSuccess()
        {
            return View();
        }

        public ActionResult DeleteAccountFailed()
        {
            return View();
        }

        #endregion


        #region Password Reset

        public ActionResult PasswordResetEmailSent()
        {
            return View();
        }

        public ActionResult PasswordResetRequest()
        {
            if (WebSecurity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _accountService.PasswordResetRequest(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction("PasswordResetEmailSent", "Account", new { userName = model.UserName, result.Data });
                }

                return RedirectToAction("ResetPasswordFailed");
            }

            return View(model);
        }

        public ActionResult ResetPassword(string username, string token)
        {
            if (!_accountService.ConfirmUserFromToken(username, token))
            {
                return RedirectToAction("ResetPasswordFailed");
            }

            var model = new UserAccount.ResetPasswordModel
            {
                UserName = username,
                Token = token
            };

            return View(model);
        }

        public ActionResult ResetPasswordFailed()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(UserAccount.ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _accountService.ResetPassword(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction("ResetPasswordSuccess");
                }

                foreach (var registrationError in result.Errors)
                {
                    ModelState.AddModelError(registrationError.FieldName, registrationError.ErrorMessage);
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        #endregion


        #region Account Activation

        [AllowAnonymous]
        public ActionResult AccountCreated(string userName, string additionalError = "")
        {
            ViewBag.AdditionalError = additionalError;
            return View();
        }

        [AllowAnonymous]
        public ActionResult AccountActivated(string userName)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AccountActivationFailed()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AccountAlreadyActivated()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ActivateAccount(string userName, string token)
        {
            var result = _accountService.ActivateAccount(userName, token);

            switch (result.Result)
            {
                case EActivateAccountResult.Success:
                    _membershipService.CreateFreeMembership(result.User.Id);
                    return RedirectToAction("AccountActivated", "Account", new { userName });

                case EActivateAccountResult.AlreadyActivated:
                    return RedirectToAction("AccountAlreadyActivated", "Account");

                default:
                    return RedirectToAction("AccountActivationFailed", "Account");
            }
        }

        [RequirePermissions(Permission = Permissions.Edit)]
        public ActionResult ActivateUserAccount(int userId)
        {
            var result = _accountService.ActivateAccount(userId);

            switch (result.Result)
            {
                case EActivateAccountResult.Success:
                    var user = _userRepository.Find(userId);
                    return RedirectToAction("AccountActivated", "Account", new { userName = user.Username });

                case EActivateAccountResult.AlreadyActivated:
                    return RedirectToAction("AccountAlreadyActivated", "Account");

                default:
                    return RedirectToAction("AccountActivationFailed", "Account");
            }
        }

        [Route("unsubscribe")]
        public ActionResult Unsubscribe(string code)
        {
            if (_contactService.Unsubscribe(code))
            {
                return View("UnsubscribeSuccess");
            }

            return View("UnsubscribeFailed");
        }

        #endregion


        #region Helpers

        public override string GetObjectName()
        {
            return typeof(User).Name;
        }

        private string GetUserPassword(string username)
        {
            var user = _userRepository.Find(e => e.Username == username).FirstOrDefault();
            var password =
                _userRepository.CustomQuery<string>($"SELECT TOP 1 Password FROM [webpages_Membership] WHERE UserId = {user.Id}").FirstOrDefault();
            return password;
        }

        private void SetUserPassword(string username, string password)
        {
            var user = _userRepository.Find(e => e.Username == username).FirstOrDefault();
            _userRepository.GetQuery($"UPDATE [webpages_Membership] SET Password = '{password}', PasswordChangedDate = GetDate() WHERE UserId = {user.Id}");
        }

        #endregion

    }
}