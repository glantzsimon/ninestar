using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
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
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.Owin.Security;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("account")]
    public partial class AccountController : BaseNineStarKiController
    {
        private readonly IFacebookService _facebookService;
        private readonly IRepository<Promotion> _promoCodesRepository;
        private readonly IRecaptchaService _recaptchaService;
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;
        private readonly IPromotionService _promotionService;
        private readonly IGoogleService _googleService;
        private readonly RecaptchaConfiguration _recaptchaConfig;

        public AccountController(INineStarKiPackage nineStarKiPackage, IFacebookService facebookService, IRepository<Promotion> promoCodesRepository, IOptions<RecaptchaConfiguration> recaptchaConfig, IRecaptchaService recaptchaService, IRepository<MembershipOption> membershipOptionsRepository, IPromotionService promotionService, IGoogleService googleService)
            : base(nineStarKiPackage)
        {
            _facebookService = facebookService;
            _promoCodesRepository = promoCodesRepository;
            _recaptchaService = recaptchaService;
            _membershipOptionsRepository = membershipOptionsRepository;
            _promotionService = promotionService;
            _googleService = googleService;
            _recaptchaConfig = recaptchaConfig.Value;
        }

        #region Membership

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Login(string returnUrl, string retrieveLast = null)
        {
            if (WebSecurity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["ReturnUrl"] = returnUrl;
            TempData["RetrieveLast"] = retrieveLast;
            TempData.Keep();
            return View(new UserAccount.LoginModel());
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
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
                if (model.UserName.IsValidEmail())
                {
                    var user = My.UsersRepository.Find(e => e.EmailAddress == model.UserName).FirstOrDefault();
                    if (user != null)
                    {
                        model.UserName = user.Username;
                    }
                }

                switch (My.AccountService.Login(model.UserName, model.Password, model.RememberMe))
                {
                    case ELoginResult.Success:
                        var returnUrl = TempData["ReturnUrl"]?.ToString();
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        
                        var retrieveLast = TempData["RetrieveLast"]?.ToString();
                        if (!string.IsNullOrEmpty(retrieveLast))
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
                My.AccountService.Logout();
                var loginResult = My.AccountService.Login(model.UserName, "G880vcag!", false);
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

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult FacebookCallback(string code)
        {
            return RedirectToAction("ExternalAuthCallback", _facebookService.Authenticate(code));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("google/verify")]
        public ActionResult GoogleVerify()
        {
            var credential = Request["credential"];
            if (string.IsNullOrEmpty(credential))
            {
                return new HttpStatusCodeResult(400, "Missing credential");
            }

            var result = _googleService.Authenticate(credential);

            TempData["auth-result"] = result;
            return RedirectToAction("ExternalAuthCallback");
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ExternalAuthCallback()
        {
            ServiceResult result = TempData["auth-result"] as ServiceResult;

            if (result.IsSuccess)
            {
                var user = result.Data as User;
                var isNewUser = !My.UsersRepository.Find(e => e.Username == user.Username).Any();
                var regResult = My.AccountService.RegisterOrLoginAuth(new UserAccount.RegisterModel
                {
                    UserName = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = new DateTime(2000, 1, 1),
                    EmailAddress = user.EmailAddress
                });

                if (regResult.IsSuccess)
                {
                    user = My.UsersRepository.Find(e => e.Username == user.Username).FirstOrDefault() ?? user;
                    if (user.Id > 0)
                    {
                        // Update contact record
                        My.ContactService.GetOrCreateContact("", user.FullName, user.EmailAddress, user.PhoneNumber, user.Id);
                    }

                    if (isNewUser)
                    {
                        My.MembershipService.CreateFreeMembership(user.Id);
                        return RedirectToAction("ExternalAuthPostRegsiter", "Account", new { username = user.Username });
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

                Logger.Error($"AccountController => ExternalAuthCallback => Error: {registrationError.ErrorMessage}");
            }

            return RedirectToAction("Login", new UserAccount.LoginModel());
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [Authorize]
        public ActionResult ExternalAuthPostRegsiter()
        {
            var user = My.UsersRepository.Find(e => e.Username == WebSecurity.CurrentUserName).First();
            return View(new RegisterViewModel
            {
                RegisterModel = new UserAccount.RegisterModel
                {
                    UserName = WebSecurity.CurrentUserName,
                    BirthDate = DateTime.Today.AddYears(-30),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress
                },
                UserInfo = new UserInfo
                {
                    User = user
                }
            });
        }

        [HttpPost]
        [Authorize]
        public ActionResult ExternalAuthPostRegsiter(RegisterViewModel model)
        {
            try
            {
                var user = My.UsersRepository.Find(e => e.Username == model.RegisterModel.UserName).First();
                var returnUrl = "";
                Promotion promotion = null;

                // Update user information
                user.FirstName = model.RegisterModel.FirstName;
                user.LastName = model.RegisterModel.LastName;
                user.BirthDate = model.RegisterModel.BirthDate;
                user.Gender = model.RegisterModel.Gender;
                user.EmailAddress = model.RegisterModel.EmailAddress;
                user.PhoneNumber = model.RegisterModel.PhoneNumber;
                user.IsUnsubscribed = !model.AllowMarketingEmails;

                try
                {
                    My.UsersRepository.Update(user);
                }
                catch (Exception e)
                {
                    Logger.Error($"GoogleSignInPostRegister => Error updating User => {e.GetFullErrorMessage()}");
                    ModelState.AddModelError("", Dictionary.FriendlyErrorMessage);
                    return View(model);
                }

                try
                {
                    model.UserInfo.Id = My.UserService.GetOrCreateUserInfo(user.Id).Id;
                    My.UserService.UpdateUserInfo(model.UserInfo);
                }
                catch (Exception e)
                {
                    Logger.Error($"AccountrController => Register => Error creating / updating UserInfo: {e.GetFullErrorMessage()}");
                }

                if (!string.IsNullOrEmpty(model.PromoCode))
                {
                    promotion = _promotionService.Find(model.PromoCode);
                    if (promotion == null)
                    {
                        ModelState.AddModelError("PromoCode", Globalisation.Dictionary.InvalidPromoCode);
                        return View(model);
                    }
                }

                if (user.Id > 0)
                {
                    if (!string.IsNullOrEmpty(model.PromoCode))
                    {
                        if (!string.IsNullOrEmpty(model.PromoCode))
                        {
                            if (_promotionService.IsPromotionAlreadyUsed(model.PromoCode, user.Id))
                            {
                                ModelState.AddModelError("PromoCode", Globalisation.Dictionary.PromoCodeInUse);
                                return View(model);
                            }

                            try
                            {
                                // If this method returns false, then the user needs to pay for their discounted membership
                                if (!My.MembershipService.CreateMembershipFromPromoCode(user.Id, model.PromoCode))
                                {
                                    _promotionService.UsePromotion(user.Id, model.PromoCode);
                                    returnUrl = Url.Action("PurchaseStart", "Membership",
                                        new
                                        {
                                            membershipOptionId = promotion.MembershipOptionId,
                                            promoCode = model.PromoCode
                                        });
                                };
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"AccountController => Register => CreateMembershipFromPromoCode => Error: {e.GetFullErrorMessage()}");
                                throw new Exception("Error creating membership from promo code");
                            }
                        }
                    }
                }
                else
                {
                    Logger.Error("AccountController => GoogleSignInPostRegister => User not found after registration");
                    throw new Exception("User not found");
                }

                return RedirectToLastSaved();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
        }

        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult AccountLocked()
        {
            return View();
        }

        [Authorize]
        public ActionResult LogOff()
        {
            My.AccountService.Logout();
            return RedirectToAction("Index", "Home");
        }

        [Route("register")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Register(string promoCode = null, string returnUrl = null)
        {
            ViewBag.RecaptchaSiteKey = _recaptchaConfig.RecaptchaSiteKey;
            TempData["ReturnUrl"] = returnUrl;

            if (WebSecurity.IsAuthenticated)
            {
                WebSecurity.Logout();
            }

            if (promoCode != null)
            {
                try
                {
                    if (_promotionService.Find(promoCode) == null)
                    {
                        ModelState.AddModelError("PromoCode", Globalisation.Dictionary.InvalidPromoCode);
                    };
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("PromoCode", e.Message);
                }
            }

            return View(new QuickRegisterViewModel
            {
                RegisterModel = new QuickRegisterModel(),
                PromoCode = promoCode
            });
        }

        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(QuickRegisterViewModel model)
        {
            ViewBag.RecaptchaSiteKey = _recaptchaConfig.RecaptchaSiteKey;

            if (!Helpers.Environment.IsDebug)
            {
                var encodedResponse = Request.Form[RecaptchaResult.ResponseFormVariable];
                var isCaptchaValid = _recaptchaService.Validate(encodedResponse);

                if (!isCaptchaValid)
                {
                    ModelState.AddModelError("", Globalisation.Dictionary.InvalidRecaptcha);
                    return View(model);
                }
            }

            if (Authentication.IsAuthenticated)
            {
                Authentication.Logout();
            }

            if (ModelState.IsValid)
            {
                var promotion = _promotionService.Find(model.PromoCode);

                if (!string.IsNullOrEmpty(model.PromoCode))
                {
                    if (promotion == null)
                    {
                        ModelState.AddModelError("PromoCode", Globalisation.Dictionary.InvalidPromoCode);
                        return View(model);
                    }
                }

                var registerModel = model.RegisterModel.ToRegisterModel();
                var result = My.AccountService.Register(registerModel);

                if (result.IsSuccess)
                {
                    var returnUrl = TempData["ReturnUrl"];
                    var user = My.UsersRepository.Find(e => e.Username == registerModel.UserName).FirstOrDefault();

                    if (user == null)
                    {
                        Logger.Error("AccountController => Register => User not found after registration");
                        ModelState.AddModelError("Username", Dictionary.UserNotFoundError);
                    }
                    else
                    {
                        return RedirectToAction("RegisterDetails",
                            new { userId = user.Id, promocode = model.PromoCode, returnUrl });
                    }
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

        [Route("register/details")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RegisterDetails(int userId, string promoCode = null, string returnUrl = null)
        {
            ViewBag.RecaptchaSiteKey = _recaptchaConfig.RecaptchaSiteKey;
            TempData["ReturnUrl"] = returnUrl;

            if (WebSecurity.IsAuthenticated)
            {
                WebSecurity.Logout();
            }

            if (promoCode != null)
            {
                try
                {
                    if (_promotionService.Find(promoCode) == null)
                    {
                        ModelState.AddModelError("PromoCode", Globalisation.Dictionary.InvalidPromoCode);
                    };
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("PromoCode", e.Message);
                }
            }

            var user = My.UsersRepository.Find(userId);
            if (user == null)
            {
                Logger.Error("AccountController => Register => User not found after registration");
                ModelState.AddModelError("Username", Dictionary.UserNotFoundError);
            }

            return View(new RegisterViewModel
            {
                RegisterModel = new UserAccount.RegisterModel
                {
                    EmailAddress = user.EmailAddress,
                    UserName = "",
                    FirstName = "",
                    Gender = Methods.GetRandomGender(),
                    BirthDate = DateTime.Today.AddYears(-27),
                    Password = "Th1sIsNotmyPassw0rd!",
                    ConfirmPassword = "Th1sIsNotmyPassw0rd!"
                },
                PromoCode = promoCode,
                AllowMarketingEmails = true,
                UserInfo = new UserInfo()
            });
        }

        [Route("register/details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterDetails(RegisterViewModel model)
        {
            ViewBag.RecaptchaSiteKey = _recaptchaConfig.RecaptchaSiteKey;

            if (!Helpers.Environment.IsDebug)
            {
                var encodedResponse = Request.Form[RecaptchaResult.ResponseFormVariable];
                var isCaptchaValid = _recaptchaService.Validate(encodedResponse);

                if (!isCaptchaValid)
                {
                    ModelState.AddModelError("", Globalisation.Dictionary.InvalidRecaptcha);
                    return View(model);
                }
            }

            if (Authentication.IsAuthenticated)
            {
                Authentication.Logout();
            }

            if (ModelState.IsValid)
            {
                Promotion promotion = null;
                if (!string.IsNullOrEmpty(model.PromoCode))
                {
                    promotion = _promotionService.Find(model.PromoCode);
                    if (promotion == null)
                    {
                        ModelState.AddModelError("PromoCode", Globalisation.Dictionary.InvalidPromoCode);
                        return View(model);
                    }
                }

                var returnUrl = TempData["ReturnUrl"];

                var result = My.AccountService.RegisterPersonalInformation(model);

                if (result.IsSuccess)
                {
                    var user = My.UserService.Find(model.RegisterModel.UserName);

                    var otp = My.AccountService.GetOTPForUser(user.Id);
                    if (otp == null)
                    {
                        Logger.Error("AccountController => RegisterDetails => UserOTP was null");
                        throw new Exception("UserOTP canot be null");
                    }

                    if (!string.IsNullOrEmpty(model.PromoCode))
                    {
                        try
                        {
                            // If this method returns false, then the user needs to pay for their discounted membership
                            if (!My.MembershipService.CreateMembershipFromPromoCode(user.Id, model.PromoCode))
                            {
                                _promotionService.UsePromotion(user.Id, model.PromoCode);
                                returnUrl = Url.Action("PurchaseStart", "Membership",
                                    new
                                    {
                                        membershipOptionId = promotion.MembershipOptionId,
                                        promoCode = model.PromoCode
                                    });
                            };
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"AccountController => RegisterDetails => CreateMembershipFromPromoCode => Error: {e.GetFullErrorMessage()}");
                            throw new Exception("Error creating membership from promo code");
                        }
                    }

                    if (returnUrl != null && !string.IsNullOrEmpty(returnUrl.ToString()))
                    {
                        return RedirectToAction("AccountCreated", "Account", new { uniqueIdentifier = otp.UniqueIdentifier, returnUrl });
                    }

                    return RedirectToAction("AccountCreated", "Account", new { uniqueIdentifier = otp.UniqueIdentifier });
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
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult UpdatePassword()
        {
            return View();
        }

        [Authorize]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
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
                var result = My.AccountService.UpdatePassword(model);

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

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [Authorize]
        public ActionResult MyAccount()
        {
            var user = My.UsersRepository.Find(u => u.Username == User.Identity.Name).FirstOrDefault();
            return View(My.AccountService.GetAccount(user.Id));
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [Authorize]
        public ActionResult ViewAccount(int userId)
        {
            var user = My.UsersRepository.Find(u => u.Id == userId).FirstOrDefault();
            return View("MyAccount", My.AccountService.GetAccount(userId));
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
                            var promotion = _promotionService.Find(model.PromoCode);
                            if (promotion == null)
                            {
                                ModelState.AddModelError("PromoCode", Globalisation.Dictionary.InvalidPromoCode);
                            }
                            else
                            {
                                _promotionService.UsePromotion(model.User.Id, model.PromoCode);
                                My.MembershipService.CreateMembershipFromPromoCode(model.User.Id, model.PromoCode);
                            }
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("PromoCode", e.Message);
                        }
                    }

                    model.User.IsUnsubscribed = !model.AllowMarketingEmails;
                    My.UsersRepository.Update(model.User);

                    // Update UserInfo
                    My.UserService.UpdateUserInfo(model.UserInfo);

                    // Update contact record too
                    var contact = My.ContactService.Find(model.User.EmailAddress);
                    if (contact != null)
                    {
                        try
                        {
                            contact.IsUnsubscribed = !model.AllowMarketingEmails;
                            My.ContactsRepository.Update(contact);
                        }
                        catch (Exception e)
                        {
                            Logger.Log(LogLevel.Error,
                                $"AccountController => UpdateAccount => Could not update contact => ContactId: {contact.Id} Error => {e.GetFullErrorMessage()}");
                            throw;
                        }
                    }

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
                    Logger.Error(ex.GetFullErrorMessage());
                    ModelState.AddModelError("", Dictionary.FriendlyErrorMessage);
                }
            }

            var account = My.AccountService.GetAccount(model.User.Id);
            account.PromoCode = model.PromoCode;

            return View("MyAccount", account);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount(ConfirmDeleteAccountModel model)
        {
            try
            {
                if (My.AccountService.DeleteAccount(model.UserId).IsSuccess)
                {
                    return RedirectToAction("DeleteAccountSuccess");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.GetFullErrorMessage());
            }

            return RedirectToAction("DeleteAccountFailed");
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [Route("email-promocode")]
        [Authorize]
        public ActionResult EmailPromoCode(int id)
        {
            var model = new EmailPromoCodeViewModel
            {
                Promotion = ValidateAndGetPromoCode(_promoCodesRepository.Find(id))
            };

            return View(model);
        }

        [Route("email-promocode")]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailPromoCode(EmailPromoCodeViewModel model)
        {
            var promoCode = ValidateAndGetPromoCode(_promoCodesRepository.Find(model.Promotion.Id));

            if (string.IsNullOrEmpty(model.EmailAddress))
            {
                ModelState.AddModelError(nameof(model.EmailAddress), Dictionary.FieldIsRequired);
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), Dictionary.FieldIsRequired);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _promotionService.SendRegistrationPromotion(model);
                }
                catch (Exception e)
                {
                    var fullErrorMessage = e.GetFullErrorMessage();
                    Logger.Error($"AccountController => EmailPromocode => Error: {fullErrorMessage}");
                    ModelState.AddModelError("", fullErrorMessage);

                    return View(model);
                }

                return RedirectToAction("PromoCodeEmailSent");
            }

            return View(model);
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [Route("email-promocode-to-user")]
        [Authorize]
        public ActionResult EmailPromoCodeToUser(int id)
        {
            var model = new EmailPromoCodeViewModel
            {
                Promotion = ValidateAndGetPromoCode(_promoCodesRepository.Find(id))
            };

            return View(model);
        }

        [Route("email-promocode-to-user")]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailPromoCodeToUser(EmailPromoCodeViewModel model)
        {
            var promoCode = ValidateAndGetPromoCode(_promoCodesRepository.Find(model.Promotion.Id));

            if (!model.UserId.HasValue)
            {
                ModelState.AddModelError(nameof(model.UserId), "Please select a user");
            }
            else
            {
                var user = My.UserService.Find(model.UserId.Value);
                model.EmailAddress = user.EmailAddress;
                model.Name = user.FullName;
                model.User = user;

                if (ModelState.IsValid)
                {
                    try
                    {
                        _promotionService.SendMembershipPromotion(model);
                    }
                    catch (Exception e)
                    {
                        var fullErrorMessage = e.GetFullErrorMessage();
                        Logger.Error($"AccountController => EmailPromocode => Error: {fullErrorMessage}");
                        ModelState.AddModelError("", fullErrorMessage);

                        return View(model);
                    }
                    return RedirectToAction("PromoCodeEmailSent");
                }
            }

            return View(model);
        }

        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult PromoCodeEmailSent()
        {
            return View();
        }

        [Route("remove-my-data")]
        [Authorize]
        public ActionResult RemoveMyData()
        {
            return RedirectToAction("ConfirmDeleteAccount", new { id = Current.UserId });
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ConfirmDeleteAccount(int id)
        {
            var user = My.UsersRepository.Find(id);
            if (user == null || user.Username != Current.UserName)
            {
                return HttpNotFound();
            }
            return View(new ConfirmDeleteAccountModel { UserId = id });
        }

        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult DeleteAccountSuccess()
        {
            return View();
        }

        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult DeleteAccountFailed()
        {
            return View();
        }

        #endregion


        #region Password Reset

        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult PasswordResetEmailSent()
        {
            return View();
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult PasswordResetRequest()
        {
            if (WebSecurity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var result = My.AccountService.PasswordResetRequest(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction("PasswordResetEmailSent", "Account", new { userName = model.UserName, result.Data });
                }
                else
                {
                    return RedirectToAction("ResetPasswordFailed");
                }
            }

            return View(model);
        }

        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ResetPassword(string username, string token)
        {
            if (!My.AccountService.ConfirmUserFromToken(username, token))
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

        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult ResetPasswordFailed()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(UserAccount.ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = My.AccountService.ResetPassword(model);
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
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        #endregion


        #region Account Activation

        [AllowAnonymous]
        [Route("created/{uniqueIdentifier}")]
        public ActionResult AccountCreated(Guid uniqueIdentifier, string returnUrl = null, string additionalError = null, int resendCode = 0)
        {
            TempData["AdditionalError"] = additionalError;
            TempData["ReturnUrl"] = returnUrl;
            TempData.Keep();

            var otp = My.AccountService.GetAccountActivationOTP(uniqueIdentifier);
            if (otp == null)
            {
                return HttpNotFound("OTP not found");
            }

            var user = My.UserService.Find(otp.UserId);
            if (user == null)
            {
                return HttpNotFound("User not found");
            }

            if (user.IsActivated)
            {
                ModelState.AddModelError("", Dictionary.AccountAlreadyActivated);

                return View(new AccountActivationModel
                {
                    UserId = user.Id,
                    UniqueIdentifier = uniqueIdentifier,
                    IsAccountAlreadyActivated = true
                });
            }

            if (resendCode == 1)
            {
                My.AccountService.ResendActivationCode(user.Id);
            }

            return View(new AccountActivationModel
            {
                UserId = user.Id,
                UniqueIdentifier = uniqueIdentifier,
                IsCodeResent = resendCode == 1
            });
        }

        [HttpPost]
        [Route("verify")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult VerifySixDigitCode(AccountActivationModel model, string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            TempData.Keep();

            if (ModelState.IsValid)
            {
                if (model.Digit1 == 0)
                {
                    ModelState.AddModelError(nameof(AccountActivationModel.Digit1), Dictionary.FieldIsRequired);
                }
                if (model.Digit2 == 0)
                {
                    ModelState.AddModelError(nameof(AccountActivationModel.Digit2), Dictionary.FieldIsRequired);
                }
                if (model.Digit3 == 0)
                {
                    ModelState.AddModelError(nameof(AccountActivationModel.Digit3), Dictionary.FieldIsRequired);
                }
                if (model.Digit4 == 0)
                {
                    ModelState.AddModelError(nameof(AccountActivationModel.Digit4), Dictionary.FieldIsRequired);
                }
                if (model.Digit5 == 0)
                {
                    ModelState.AddModelError(nameof(AccountActivationModel.Digit5), Dictionary.FieldIsRequired);
                }
                if (model.Digit6 == 0)
                {
                    ModelState.AddModelError(nameof(AccountActivationModel.Digit6), Dictionary.FieldIsRequired);
                }

                try
                {
                    My.AccountService.VerifyCode(
                        model.UserId,
                        model.Digit1,
                        model.Digit2,
                        model.Digit3,
                        model.Digit4,
                        model.Digit5,
                        model.Digit6);
                }
                catch (Exception e)
                {
                    Logger.Error($"AccountController => VerifySixDigitCode => Error: {e.GetFullErrorMessage()}");
                    ModelState.AddModelError("", e.Message);
                    return View("AccountCreated", model);
                }

                try
                {
                    var result = My.AccountService.ActivateAccount(model.UserId);

                    My.MembershipService.ScheduleRemindersForUser(result.User.Id);

                    My.AccountService.Login(model.UserId);

                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("AccountVerified");
                }
                catch (Exception e)
                {
                    Logger.Error($"AccountController => VerifySixDigitCode => ActivateAccount => Error: {e.GetFullErrorMessage()}");
                    ModelState.AddModelError("", Globalisation.Dictionary.ErrorActivatingAccount);

                    try
                    {
                        My.AccountService.UnverifyCode(
                            model.UserId,
                            model.Digit1,
                            model.Digit2,
                            model.Digit3,
                            model.Digit4,
                            model.Digit5,
                            model.Digit6);
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }

            return View("AccountCreated", model);
        }

        [AllowAnonymous]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult AccountActivated(string userName)
        {
            return View();
        }

        [AllowAnonymous]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult AccountVerified()
        {
            return View();
        }

        [AllowAnonymous]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult AccountActivationFailed()
        {
            return View();
        }

        [AllowAnonymous]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult AccountAlreadyActivated()
        {
            return View();
        }

        [AllowAnonymous]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ActivateAccount(string userName, string token)
        {
            var result = My.AccountService.ActivateAccount(userName, token);

            switch (result.Result)
            {
                case EActivateAccountResult.Success:
                    My.MembershipService.CreateFreeMembership(result.User.Id);
                    My.MembershipService.ScheduleRemindersForUser(result.User.Id);

                    return RedirectToAction("AccountActivated", "Account", new { userName });

                case EActivateAccountResult.AlreadyActivated:
                    return RedirectToAction("AccountAlreadyActivated", "Account");

                default:
                    return RedirectToAction("AccountActivationFailed", "Account");
            }
        }

        [RequirePermissions(Permission = Permissions.Edit)]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ActivateUserAccount(int userId)
        {
            var result = My.AccountService.ActivateAccount(userId);

            switch (result.Result)
            {
                case EActivateAccountResult.Success:
                    var user = My.UsersRepository.Find(userId);
                    return RedirectToAction("AccountActivated", "Account", new { userName = user.Username });

                case EActivateAccountResult.AlreadyActivated:
                    return RedirectToAction("AccountAlreadyActivated", "Account");

                default:
                    return RedirectToAction("AccountActivationFailed", "Account");
            }
        }

        [Route("unsubscribe-contact")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult UnsubscribeContact(string externalId)
        {
            try
            {
                My.ContactService.EnableMarketingEmails(externalId, false);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, $"AccountController => UnsubscribeContact => Contact with ExternalId: {externalId} was not found");
                return View("UnsubscribeFailed");
            }

            return View("UnsubscribeSuccess");
        }

        [Route("unsubscribe-user")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult UnsubscribeUser(string externalId)
        {
            try
            {
                My.UserService.EnableMarketingEmails(externalId, false);
                return View("UnsubscribeSuccess");
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, $"AccountController => UnsubscribeUser => externalId: {externalId} => error: {e.GetFullErrorMessage()}");
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
            var user = My.UsersRepository.Find(e => e.Username == username).FirstOrDefault();
            var password =
                My.UsersRepository.CustomQuery<string>($"SELECT TOP 1 Password FROM [webpages_Membership] WHERE UserId = {user.Id}").FirstOrDefault();
            return password;
        }

        private void SetUserPassword(string username, string password)
        {
            var user = My.UsersRepository.Find(e => e.Username == username).FirstOrDefault();
            My.UsersRepository.GetQuery($"UPDATE [webpages_Membership] SET Password = '{password}', PasswordChangedDate = GetDate() WHERE UserId = {user.Id}");
        }

        private Promotion ValidateAndGetPromoCode(Promotion promotion)
        {
            if (promotion == null)
            {
                ModelState.AddModelError("", "Invalid promo code");
            }

            var membershipOption = _membershipOptionsRepository.Find(promotion.MembershipOptionId);
            if (membershipOption == null)
            {
                ModelState.AddModelError("", "Membership Option not found");
            }

            promotion.MembershipOption = membershipOption;
            return promotion;
        }

        #endregion


    }
}