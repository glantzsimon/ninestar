using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Config;
using Microsoft.Web.WebPages.OAuth;
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

		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if (WebSecurity.IsAuthenticated)
			{
				WebSecurity.Logout();
				return RedirectToAction("Index", "Home");
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(new UserAccount.LoginModel());
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(UserAccount.LoginModel model)
		{
			if (ModelState.IsValid)
			{
				if (!WebSecurity.IsConfirmed(model.UserName))
				{
					ModelState.AddModelError("", Dictionary.AccountNotActivatedError);
				}
				else if (WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
				{
					if (!string.IsNullOrEmpty(ViewBag.ReturnUrl))
					{
						return Redirect(ViewBag.ReturnUrl);
					}
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
				}
			}
			else
			{
				ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
			}

			return View(model);
		}

		public ActionResult LogOff()
		{
			WebSecurity.Logout();
			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
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
								FirstName = model.FirstName,
								model.LastName,
								model.PhoneNumber,
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Disassociate(string provider, string providerUserId)
		{
			string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
			ManageMessageId? message = null;

			if (ownerAccount == User.Identity.Name)
			{
				using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
				{
					bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
					if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
					{
						OAuthWebSecurity.DeleteAccount(provider, providerUserId);
						scope.Complete();
						message = ManageMessageId.RemoveLoginSuccess;
					}
				}
			}

			return RedirectToAction("Manage", new { Message = message });
		}

		public ActionResult Manage(ManageMessageId? message)
		{
			ViewBag.StatusMessage = GetPasswordChangedMessage(message);

			ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			ViewBag.ReturnUrl = Url.Action("Manage");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Manage(UserAccount.LocalPasswordModel model)
		{
			bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			ViewBag.HasLocalPassword = hasLocalAccount;

			if (hasLocalAccount)
			{
				if (ModelState.IsValid)
				{
					bool changePasswordSucceeded = true;
					try
					{
						changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
					}
					catch (Exception)
					{
					}

					if (changePasswordSucceeded)
					{
						if (Request.IsAjaxRequest())
						{
							ViewBag.Message = GetPasswordChangedMessage(ManageMessageId.ChangePasswordSuccess);
						}
						return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
					}

					ModelState.AddModelError("", Dictionary.CurrentPasswordCorrectNewInvalidError);
				}
			}
			else
			{
				// User does not have a local password so remove any validation errors caused by a missing OldPassword field
				ModelState state = ModelState["OldPassword"];
				if (state != null)
				{
					state.Errors.Clear();
				}

				if (ModelState.IsValid)
				{
					try
					{
						WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
						return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
					}
					catch (Exception e)
					{
						ModelState.AddModelError("", e);
					}
				}
			}

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
		}

		[AllowAnonymous]
		public ActionResult ExternalLoginCallback(string returnUrl)
		{
			AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
			if (!result.IsSuccessful)
			{
				return RedirectToAction("ExternalLoginFailure");
			}

			if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, false))
			{
				return Redirect(returnUrl);
			}

			if (User.Identity.IsAuthenticated)
			{
				OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
				return Redirect(returnUrl);
			}

			// User is new, ask for their desired membership name
			string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
			var clientData = OAuthWebSecurity.GetOAuthClientData(result.Provider);
			var emailAddress = result.ExtraData["email"];
			ViewBag.ProviderDisplayName = clientData.DisplayName;
			ViewBag.ReturnUrl = returnUrl;

			if (string.IsNullOrEmpty(emailAddress))
			{
				emailAddress = string.Format("{0}@{1}.com", result.UserName.RemoveSpaces(), result.Provider.RemoveSpaces());
			}

			return View("ExternalLoginConfirmation", new UserAccount.RegisterExternalLoginModel { UserName = result.UserName, EmailAddress = emailAddress, ExternalLoginData = loginData });

		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLoginConfirmation(UserAccount.RegisterExternalLoginModel model, string returnUrl)
		{
			string provider;
			string providerUserId;

			if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
			{
				return RedirectToAction("Manage");
			}

			if (ModelState.IsValid)
			{
				User user = _repository.Find(u => u.Username.ToLower() == model.UserName.ToLower()).FirstOrDefault();
				if (user == null)
				{
					_repository.Create(new User { Username = model.UserName, EmailAddress = model.EmailAddress });

					OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
					OAuthWebSecurity.Login(provider, providerUserId, false);

					return Redirect(returnUrl);
				}

				ModelState.AddModelError("UserName", Dictionary.UsernameIsUnavailableError);
			}

			ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		[AllowAnonymous]
		[ChildActionOnly]
		public ActionResult ExternalLoginsList(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
		}

		[ChildActionOnly]
		public ActionResult RemoveExternalLogins()
		{
			ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
			List<UserAccount.ExternalLogin> externalLogins = new List<UserAccount.ExternalLogin>();
			foreach (OAuthAccount account in accounts)
			{
				AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

				externalLogins.Add(new UserAccount.ExternalLogin
				{
					Provider = account.Provider,
					ProviderDisplayName = clientData.DisplayName,
					ProviderUserId = account.ProviderUserId
				});
			}

			ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			return PartialView("_RemoveExternalLoginsPartial", externalLogins);
		}

		#endregion


		#region Password Reset

		[AllowAnonymous]
		public ActionResult PasswordResetEmailSent(string userName, string token)
		{
			var userId = WebSecurity.GetUserIdFromPasswordResetToken(token);
			var confirmId = WebSecurity.GetUserId(userName);

			if (Equals(userId, confirmId))
			{
				ViewBag.Message = Dictionary.PasswordResetEmailSent;
			}
			else
			{
				ModelState.AddModelError("", Dictionary.PasswordResetFailError);
			}

			return View();
		}

		[AllowAnonymous]
		public ActionResult PasswordResetRequest()
		{
			if (WebSecurity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model)
		{
			if (ModelState.IsValid)
			{
				var user = _repository.Find(u => u.EmailAddress == model.EmailAddress).FirstOrDefault();
				if (user == null)
				{
					ModelState.AddModelError("", Dictionary.PasswordResetFailError);
				}
				else
				{
					model.UserName = user.Username;
					var token = WebSecurity.GeneratePasswordResetToken(user.Username);
					SendPasswordResetEmail(model, token);

					return RedirectToAction("PasswordResetEmailSent", "Account", new { userName = model.UserName, token });
				}
			}

			return View(model);
		}

		[AllowAnonymous]
		public ActionResult ResetPassword(string userName, string token)
		{
			UserAccount.ResetPasswordModel model;
			var userId = WebSecurity.GetUserIdFromPasswordResetToken(token);
			var confirmUserId = WebSecurity.GetUserId(userName);

			if (!Equals(userId, confirmUserId))
			{
				ModelState.AddModelError("", Dictionary.PasswordResetFailError);

				model = new UserAccount.ResetPasswordModel
				{
					Token = ""
				};
			}
			else
			{
				model = new UserAccount.ResetPasswordModel
				{
					UserName = userName,
					Token = token
				};
			}

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword(UserAccount.ResetPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				WebSecurity.ResetPassword(model.Token, model.NewPassword);
				WebSecurity.Login(model.UserName, model.NewPassword);
				ViewBag.Message = Dictionary.PasswordSetConfirmation;
				return RedirectToAction("Index", "Home");
			}

			return View(model);
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
			var userId = WebSecurity.GetUserId(userName);

			if (!_repository.Exists(userId))
			{
				ModelState.AddModelError("", Dictionary.InvalidUsernameError);
			}
			else
			{
				ViewBag.Message = Dictionary.AccountCreatedSuccessfully;
			}

			return View();
		}

		[AllowAnonymous]
		public ActionResult AccountActivated(string userName, string token)
		{
			var userId = WebSecurity.GetUserId(userName);

			if (!_repository.Exists(userId))
			{
				ModelState.AddModelError("", Dictionary.InvalidUsernameError);
			}
			else if (!WebSecurity.IsConfirmed(userName))
			{
				ModelState.AddModelError("", Dictionary.AccountActivationFailed);
			}
			else if (!WebSecurity.ConfirmAccount(userName, token))
			{
				ModelState.AddModelError("", Dictionary.AccountActivationFailed);
			}
			else
			{
				ViewBag.Message = Dictionary.AccountActivatedSuccessfully;
			}

			return View();
		}

		[AllowAnonymous]
		public ActionResult ActivateAccount(string userName, string token)
		{
			if (WebSecurity.IsConfirmed(userName))
			{
				ModelState.AddModelError("", Dictionary.AccountAlreadyActivated);
			}
			else if (!WebSecurity.ConfirmAccount(userName, token))
			{
				ModelState.AddModelError("", Dictionary.AccountActivationFailed);
			}
			else
			{
				return RedirectToAction("AccountActivated", "Account", new { userName, token });
			}

			return View();
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

			Mailer.SendEmail(Dictionary.AccountActivationTitle, emailContent, model.EmailAddress, model.FullName);
		}

		#endregion


		#region Helpers

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess
		}

		internal class ExternalLoginResult : ActionResult
		{
			public ExternalLoginResult(string provider, string returnUrl)
			{
				Provider = provider;
				ReturnUrl = returnUrl;
			}

			public string Provider { get; private set; }
			public string ReturnUrl { get; private set; }

			public override void ExecuteResult(ControllerContext context)
			{
				OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
			}
		}

		private static string GetPasswordChangedMessage(ManageMessageId? messageId)
		{
			return messageId == ManageMessageId.ChangePasswordSuccess ? Dictionary.PasswordHasChangedConfirmation
				: messageId == ManageMessageId.SetPasswordSuccess ? Dictionary.PasswordSetConfirmation
				: messageId == ManageMessageId.RemoveLoginSuccess ? Dictionary.ExternalLoginRemovedConfirmation
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