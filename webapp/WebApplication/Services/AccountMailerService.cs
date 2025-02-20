using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Config;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class AccountMailerService : Services.IAccountMailerService
    {
        private readonly IRepository<User> _usersRepository;
        private readonly IMailer _mailer;
        private readonly ILogger _logger;
        private readonly IRoles _roles;
        private readonly IContactService _contactService;
        private readonly WebsiteConfiguration _config;
        private UrlHelper _urlHelper;

        public UrlHelper UrlHelper
        {
            get => _urlHelper;
            set => _urlHelper = value;
        }

        public AccountMailerService(IOptions<WebsiteConfiguration> config, IRepository<User> usersRepository, IMailer mailer, ILogger logger, IRoles roles,
            IContactService contactService)
        {
            _usersRepository = usersRepository;
            _mailer = mailer;
            _logger = logger;
            _roles = roles;
            _contactService = contactService;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
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
            var user = _usersRepository.Find(e => e.Username == model.UserName).FirstOrDefault();
            var imageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl);

            var emailContent = TemplateParser.Parse(Globalisation.Dictionary.WelcomeEmail, new
            {
                Title = Dictionary.Welcome,
                model.FirstName,
                Company = _config.CompanyName,
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = _urlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }),
                ActivationCode = sixDigitCode,
                ImageUrl = imageUrl,
                From = _config.CompanyName
            });

            _mailer.SendEmail(Dictionary.AccountActivationTitle, emailContent, model.EmailAddress, model.GetFullName(), _config.SupportEmailAddress, _config.CompanyName);
        }

        public void SendPasswordResetEmail(UserAccount.PasswordResetRequestModel model, string token)
        {
            var resetPasswordLink = GetPasswordResetLink(model, token);
            var imageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl);
            var user = _usersRepository.Find(u => u.Username == model.UserName).FirstOrDefault();

            if (user == null)
            {
                _logger.Error("SendPasswordResetEmail failed as no user was found. PasswordResetRequestModel: {0}", model);
                throw new NullReferenceException("User cannot be null");
            }

            var emailContent = TemplateParser.Parse(string.IsNullOrEmpty(_config.PasswordResetEmailTemplateText) ? Dictionary.PasswordResetEmail : _config.PasswordResetEmailTemplateText, new
            {
                Title = Dictionary.Welcome,
                user.FirstName,
                Company = _config.CompanyName,
                ResetPasswordLink = resetPasswordLink,
                ImageUrl = imageUrl,
                From = _config.CompanyName
            });

            _mailer.SendEmail(Dictionary.PasswordResetTitle, emailContent, model.EmailAddress, user.FullName, _config.SupportEmailAddress, _config.CompanyName);
        }

        private string GetPasswordResetLink(UserAccount.PasswordResetRequestModel model, string token)
        {
            return _urlHelper.AbsoluteAction("ResetPassword", "Account", new { userName = model.UserName, token });
        }

        private string GetActivationLink(UserAccount.RegisterModel model, string token)
        {
            return _urlHelper.AbsoluteAction("ActivateAccount", "Account", new { userName = model.UserName, token });
        }
    }
}