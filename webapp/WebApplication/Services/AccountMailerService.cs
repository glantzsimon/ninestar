using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Packages;
using NLog;
using System;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class AccountMailerService : BaseService, IAccountMailerService
    {
        private readonly IContactService _contactService;

        public AccountMailerService(INineStarKiBasePackage my, IContactService contactService)
        : base(my)
        {
            _contactService = contactService;
        }

        public void SendActivationEmail(User user, int sixDigitCode)
        {
            SendActivationEmail(new UserAccount.RegisterModel
            {
                BirthDate = user.BirthDate,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            }, sixDigitCode);
        }

        public void SendActivationEmail(UserAccount.RegisterModel model, int sixDigitCode)
        {
            var contact = _contactService.Find(model.EmailAddress);
            var user = My.UsersRepository.Find(e => e.Username == model.UserName).FirstOrDefault();
            var imageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl);

            var emailContent = TemplateParser.Parse(Globalisation.Dictionary.WelcomeEmail, new
            {
                Title = Dictionary.Welcome,
                model.FirstName,
                Company = My.WebsiteConfiguration.CompanyName,
                PrivacyPolicyLink = My.UrlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = My.UrlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = My.UrlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                ActivationCode = sixDigitCode,
                ImageUrl = imageUrl,
                From = My.WebsiteConfiguration.CompanyName
            });

            My.Mailer.SendEmail(Dictionary.AccountActivationTitle, emailContent, model.EmailAddress, model.GetFullName(), My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);
        }

        public void SendPasswordResetEmail(UserAccount.PasswordResetRequestModel model, string token)
        {
            var resetPasswordLink = GetPasswordResetLink(model, token);
            var imageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl);
            var user = My.UsersRepository.Find(u => u.Username == model.UserName).FirstOrDefault();

            if (user == null)
            {
                My.Logger.Error("SendPasswordResetEmail failed as no user was found. PasswordResetRequestModel: {0}", model);
                throw new NullReferenceException("User cannot be null");
            }

            var emailContent = TemplateParser.Parse(Globalisation.Dictionary.PasswordResetEmail, new
            {
                Title = Dictionary.Welcome,
                user.FirstName,
                Company = My.WebsiteConfiguration.CompanyName,
                ResetPasswordLink = resetPasswordLink,
                ImageUrl = imageUrl,
                From = My.WebsiteConfiguration.CompanyName
            });

            My.Mailer.SendEmail(Dictionary.PasswordResetTitle, emailContent, model.EmailAddress, user.FullName, My.WebsiteConfiguration.SupportEmailAddress, My.WebsiteConfiguration.CompanyName);
        }

        private string GetPasswordResetLink(UserAccount.PasswordResetRequestModel model, string token)
        {
            return My.UrlHelper.AbsoluteAction("ResetPassword", "Account", new { userName = model.UserName, token });
        }

        private string GetActivationLink(UserAccount.RegisterModel model, string token)
        {
            return My.UrlHelper.AbsoluteAction("ActivateAccount", "Account", new { userName = model.UserName, token });
        }
    }
}