using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Packages;
using System;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class AccountMailerService : BaseService, IAccountMailerService
    {
        private readonly IContactService _contactService;
        private readonly IEmailTemplateService _emailTemplateService;

        public AccountMailerService(INineStarKiBasePackage my, IContactService contactService, IEmailTemplateService emailTemplateService)
        : base(my)
        {
            _contactService = contactService;
            _emailTemplateService = emailTemplateService;
        }

        public void SendActivationEmailToUser(User user, int sixDigitCode)
        {
            SendActivationEmailToUser(new UserAccount.RegisterModel
            {
                BirthDate = user.BirthDate,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            }, sixDigitCode);
        }

        public void SendActivationEmailToUser(UserAccount.RegisterModel model, int sixDigitCode)
        {
            var user = My.UsersRepository.Find(e => e.Username == model.UserName).FirstOrDefault();
            var title = Dictionary.Welcome;
            var body = _emailTemplateService.ParseForUser(
                title,
                Globalisation.Dictionary.AccountActivationEmail,
                user,
                new
                {
                    model.FirstName,
                    ActivationCode = sixDigitCode,
                });

            try
            {
                My.Mailer.SendEmail(
                    title,
                    body,
                    user.EmailAddress,
                    user.FullName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
                throw;
            }
        }

        public void SendPasswordResetEmailToUser(UserAccount.PasswordResetRequestModel model, string token)
        {
            var resetPasswordLink = GetPasswordResetLink(model, token);
            var imageUrl = My.UrlHelper.AbsoluteContent(My.WebsiteConfiguration.CompanyLogoUrl);
            var user = My.UsersRepository.Find(u => u.Username == model.UserName).FirstOrDefault();

            if (user == null)
            {
                My.Logger.Error("SendPasswordResetEmail failed as no user was found. PasswordResetRequestModel: {0}", model);
                throw new NullReferenceException("User cannot be null");
            }
            var title = Dictionary.PasswordResetTitle;
            var body = _emailTemplateService.ParseForUser(
                title,
                Globalisation.Dictionary.PasswordResetEmail,
                user,
                new
                {
                    user.FirstName,
                    ResetPasswordLink = resetPasswordLink,
                });

            try
            {
                My.Mailer.SendEmail(
                    title,
                    body,
                    user.EmailAddress,
                    user.FullName);
            }
            catch (Exception ex)
            {
                My.Logger.Error(ex.GetFullErrorMessage());
                throw;
            }
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