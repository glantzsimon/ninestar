using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Extensions;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.Models;
using K9.Base.WebApplication.Options;
using K9.Base.WebApplication.Services;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;
using K9.Base.DataAccessLayer.Enums;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    public partial class AccountController : BaseNineStarKiController
    {
        private readonly IRepository<User> _repository;
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;
        private readonly IAuthentication _authentication;
        private readonly IFacebookService _facebookService;
        private readonly IMembershipService _membershipService;

        public AccountController(IRepository<User> repository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> websiteConfig, IDataSetsHelper dataSetsHelper, IRoles roles, IAccountService accountService, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IFacebookService facebookService, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _repository = repository;
            _logger = logger;
            _accountService = accountService;
            _authentication = authentication;
            _facebookService = facebookService;
            _membershipService = membershipService;

            websiteConfig.Value.RegistrationEmailTemplateText = Globalisation.Dictionary.WelcomeEmail;
            websiteConfig.Value.PasswordResetEmailTemplateText = Globalisation.Dictionary.PasswordResetEmail;
        }

        #region Membership

        public ActionResult Login(string returnUrl)
        {
            if (WebSecurity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["ReturnUrl"] = returnUrl;
            return View(new UserAccount.LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserAccount.LoginModel model)
        {
            if (ModelState.IsValid)
            {
                switch (_accountService.Login(model.UserName, model.Password, model.RememberMe))
                {
                    case ELoginResult.Success:
                        if (TempData["ReturnUrl"] != null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
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

        public ActionResult Facebook()
        {
            return Redirect(_facebookService.GetLoginUrl().AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            ServiceResult result = null;
            result = _facebookService.Authenticate(code);
            if (result.IsSuccess)
            {
                var user = result.Data as User;
                var regResult = _accountService.RegisterOrLoginAuth(new UserAccount.RegisterModel
                {
                    UserName = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    EmailAddress = user.EmailAddress
                });

                if (regResult.IsSuccess)
                {
                    return RedirectToAction("Index", "Home");
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

        public ActionResult Register()
        {
            return View(new UserAccount.RegisterModel
            {
                Gender = EGender.Female
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserAccount.RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _accountService.Register(model);

                if (result.IsSuccess)
                {
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
            var user = _repository.Find(u => u.Username == User.Identity.Name).FirstOrDefault();
            return View(new MyAccountViewModel
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
        public ActionResult UpdateAccount(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repository.Update(model);
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
                User = model,
                Membership = _membershipService.GetActiveUserMembership(model.Id)
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

        public ActionResult ConfirmDeleteAccount(int id)
        {
            var user = _repository.Find(id);
            if (user == null || user.Username != _authentication.CurrentUserName)
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
        public ActionResult AccountCreated(string userName)
        {
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
                    var user = _repository.Find(userId);
                    return RedirectToAction("AccountActivated", "Account", new { userName = user.Username });

                case EActivateAccountResult.AlreadyActivated:
                    return RedirectToAction("AccountAlreadyActivated", "Account");

                default:
                    return RedirectToAction("AccountActivationFailed", "Account");
            }
        }

        #endregion


        #region Helpers

        public override string GetObjectName()
        {
            return typeof(User).Name;
        }

        #endregion

    }
}