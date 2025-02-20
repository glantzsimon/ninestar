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

        public AccountMailerService(INineStarKiBasePackage package, IContactService contactService)
        : base(package)
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
            var user = Package.UsersRepository.Find(e => e.Username == model.UserName).FirstOrDefault();
            var imageUrl = Package.UrlHelper.AbsoluteContent(Package.WebsiteConfiguration.CompanyLogoUrl);

            var emailContent = TemplateParser.Parse(Globalisation.Dictionary.WelcomeEmail, new
            {
                Title = Dictionary.Welcome,
                model.FirstName,
                Company = Package.WebsiteConfiguration.CompanyName,
                PrivacyPolicyLink = Package.UrlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = Package.UrlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = Package.UrlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                ActivationCode = sixDigitCode,
                ImageUrl = imageUrl,
                From = Package.WebsiteConfiguration.CompanyName
            });

            Package.Mailer.SendEmail(Dictionary.AccountActivationTitle, emailContent, model.EmailAddress, model.GetFullName(), Package.WebsiteConfiguration.SupportEmailAddress, Package.WebsiteConfiguration.CompanyName);
        }

        public void SendPasswordResetEmail(UserAccount.PasswordResetRequestModel model, string token)
        {
            var resetPasswordLink = GetPasswordResetLink(model, token);
            var imageUrl = Package.UrlHelper.AbsoluteContent(Package.WebsiteConfiguration.CompanyLogoUrl);
            var user = Package.UsersRepository.Find(u => u.Username == model.UserName).FirstOrDefault();

            if (user == null)
            {
                Package.Logger.Error("SendPasswordResetEmail failed as no user was found. PasswordResetRequestModel: {0}", model);
                throw new NullReferenceException("User cannot be null");
            }

            var emailContent = TemplateParser.Parse(Globalisation.Dictionary.PasswordResetEmail, new
            {
                Title = Dictionary.Welcome,
                user.FirstName,
                Company = Package.WebsiteConfiguration.CompanyName,
                ResetPasswordLink = resetPasswordLink,
                ImageUrl = imageUrl,
                From = Package.WebsiteConfiguration.CompanyName
            });

            Package.Mailer.SendEmail(Dictionary.PasswordResetTitle, emailContent, model.EmailAddress, user.FullName, Package.WebsiteConfiguration.SupportEmailAddress, Package.WebsiteConfiguration.CompanyName);
        }

        private string GetPasswordResetLink(UserAccount.PasswordResetRequestModel model, string token)
        {
            return Package.UrlHelper.AbsoluteAction("ResetPassword", "Account", new { userName = model.UserName, token });
        }

        private string GetActivationLink(UserAccount.RegisterModel model, string token)
        {
            return Package.UrlHelper.AbsoluteAction("ActivateAccount", "Account", new { userName = model.UserName, token });
        }
    }
}