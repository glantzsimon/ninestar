using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using K9.DataAccess.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Options;
using NLog;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
	public class AccountController : Controller
	{
		private readonly IRepository<User> _repository;
		private readonly ILogger _logger;


		#region Constructors

		public AccountController(IRepository<User> repository, ILogger logger)
		{
			_repository = repository;
			_logger = logger;
		}

		#endregion


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
				if (WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
				{
					if (TempData["ReturnUrl"] != null)
					{
						return Redirect(TempData["ReturnUrl"].ToString());
					}
					return RedirectToAction("Index", "Home");
				}
				if (WebSecurity.IsAccountLockedOut(model.UserName, 10, TimeSpan.FromDays(1)))
				{
					return RedirectToAction("AccountLocked");
				}
				ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
			}
			else
			{
				ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
			}

			return View(model);
		}

		public ActionResult AccountLocked()
		{
			return View();
		}

		[Authorize]
		public ActionResult LogOff()
		{
			WebSecurity.Logout();
			return RedirectToAction("Index", "Home");
		}

		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(UserAccount.RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				if (_repository.Exists(u => u.Username == model.UserName))
				{
					ModelState.AddModelError("UserName", Dictionary.UsernameIsUnavailableError);
				}
				else if (_repository.Exists(u => u.EmailAddress == model.EmailAddress))
				{
					ModelState.AddModelError("EmailAddress", Dictionary.EmailIsUnavailableError);
				}

				if (model.Password != model.ConfirmPassword)
				{
					ModelState.AddModelError("ConfirmPassword", Dictionary.PasswordMatchError);
				}

				if (ModelState.IsValid)
				{
					try
					{
						var token = WebSecurity.CreateUserAndAccount(model.UserName, model.Password,
							new
							{
								model.EmailAddress,
								model.FirstName,
								model.LastName,
								model.PhoneNumber,
								model.BirthDate,
								CreatedBy = SystemUser.System,
								CreatedOn = DateTime.Now,
								LastUpdatedBy = SystemUser.System,
								LastUpdatedOn = DateTime.Now
							}, true);

						SendActivationemail(model, token);

						return RedirectToAction("AccountCreated", "Account", new { userName = model.UserName });
					}
					catch (MembershipCreateUserException e)
					{
						ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
					}
					catch (Exception ex)
					{
						ModelState.AddModelError("", ex.Message);
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
				try
				{
					if (WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
					{
						return RedirectToAction("UpdatePasswordSuccess");
					}
					ModelState.AddModelError("", Dictionary.CurrentPasswordCorrectNewInvalidError);
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					ModelState.AddModelError("", Dictionary.UpdatePaswordError);
				}
			}

			return View(model);
		}

		[Authorize]
		public ActionResult MyAccount()
		{
			var user = _repository.Find(u => u.Username == User.Identity.Name).FirstOrDefault();
			return View(user);
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
					_logger.Error(ex.Message);
					ModelState.AddModelError("", Dictionary.FriendlyErrorMessage);
				}
			}

			return View("MyAccount", model);
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
				var user = _repository.Find(u => u.EmailAddress == model.EmailAddress).FirstOrDefault();
				if (user == null)
				{
					return RedirectToAction("ResetPasswordFailed");
				}

				var token = string.Empty;
				try
				{
					model.UserName = user.Username;
					token = WebSecurity.GeneratePasswordResetToken(user.Username);
					SendPasswordResetEmail(model, token);
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					return RedirectToAction("ResetPasswordFailed");
				}

				return RedirectToAction("PasswordResetEmailSent", "Account", new { userName = model.UserName, token });
			}

			return View(model);
		}

		public ActionResult ResetPassword(string userName, string token)
		{
			var userId = WebSecurity.GetUserIdFromPasswordResetToken(token);
			var confirmUserId = WebSecurity.GetUserId(userName);

			if (!Equals(userId, confirmUserId))
			{
				return RedirectToAction("ResetPasswordFailed");
			}

			var model = new UserAccount.ResetPasswordModel
			{
				UserName = userName,
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
				try
				{
					WebSecurity.ResetPassword(model.Token, model.NewPassword);
					WebSecurity.Login(model.UserName, model.NewPassword);

					return RedirectToAction("ResetPasswordSuccess");
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					ModelState.AddModelError("", Dictionary.PasswordResetFailError);
				}
			}

			return View(model);
		}

		[Authorize]
		public ActionResult ResetPasswordSuccess()
		{
			return View();
		}

		private string GetPasswordResetLink(UserAccount.PasswordResetRequestModel model, string token)
		{
			return Url.AsboluteAction("ResetPassword", "Account", new { userName = model.UserName, token });
		}

		private void SendPasswordResetEmail(UserAccount.PasswordResetRequestModel model, string token)
		{
			var resetPasswordLink = GetPasswordResetLink(model, token);
			var imageUrl = Url.AbsoluteContent(AppConfig.CompanyLogoUrl);
			var user = _repository.Find(u => u.Username == model.UserName).First();

			if (user == null)
			{
				_logger.Error("SendPasswordResetEmail failed as no user was found. PasswordResetRequestModel: {0}", model);
				throw new NullReferenceException();
			}

			var firstName = user.FirstName;
			var name = user.Name;

			var emailContent = TemplateProcessor.PopulateTemplate(Dictionary.PasswordResetEmail, new
			{
				Title = Dictionary.Welcome,
				FirstName = firstName,
				Company = AppConfig.CompanyName,
				ResetPasswordLink = resetPasswordLink,
				ImageUrl = imageUrl,
				From = Dictionary.ClientServices
			});

			Mailer.SendEmail(Dictionary.PasswordResetTitle, emailContent, model.EmailAddress, name);
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
			if (WebSecurity.IsConfirmed(userName))
			{
				return RedirectToAction("AccountAlreadyActivated", "Account");
			}

			if (!WebSecurity.ConfirmAccount(userName, token))
			{
				return RedirectToAction("AccountActivationFailed", "Account");
			}

			return RedirectToAction("AccountActivated", "Account", new { userName });
		}

		private string GetActivationLink(UserAccount.RegisterModel model, string token)
		{
			return Url.AsboluteAction("ActivateAccount", "Account", new { userName = model.UserName, token });
		}

		private void SendActivationemail(UserAccount.RegisterModel model, string token)
		{
			var activationLink = GetActivationLink(model, token);
			var imageUrl = Url.AbsoluteContent(AppConfig.CompanyLogoUrl);

			var emailContent = TemplateProcessor.PopulateTemplate(Dictionary.WelcomeEmail, new
			{
				Title = Dictionary.Welcome,
				FirstName = model.FirstName,
				Company = AppConfig.CompanyName,
				ActivationLink = activationLink,
				ImageUrl = imageUrl,
				From = Dictionary.ClientServices
			});

			Mailer.SendEmail(Dictionary.AccountActivationTitle, emailContent, model.EmailAddress, model.GetFullName());
		}

		#endregion


		#region Helpers

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess
		}

		private static string GetPasswordChangedMessage(ManageMessageId? messageId)
		{
			return messageId == ManageMessageId.ChangePasswordSuccess ? Dictionary.PasswordHasChangedConfirmation
				: messageId == ManageMessageId.SetPasswordSuccess ? Dictionary.PasswordSetConfirmation
				: "";
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

		#endregion

	}
}