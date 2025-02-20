using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Web.Mvc;
using K9.Base.WebApplication.Config;
using K9.WebApplication.Config;

namespace K9.WebApplication.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IRepository<Contact> _contactsRepository;
        private readonly ILogger _logger;
        private readonly IRepository<User> _usersRepository;
        private readonly IRepository<EmailTemplate> _emailTemplatesRepository;
        private readonly DefaultValuesConfiguration _defaultValuesConfig;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public EmailTemplateService(ILogger logger, IRepository<Contact> contactsRepository, IRepository<User> usersRepository, IRepository<EmailTemplate> emailTemplatesRepository, IOptions<WebsiteConfiguration> config, IOptions<DefaultValuesConfiguration> defaultValuesConfig)
        {
            _contactsRepository = contactsRepository;
            _logger = logger;
            _usersRepository = usersRepository;
            _emailTemplatesRepository = emailTemplatesRepository;
            _defaultValuesConfig = defaultValuesConfig.Value;
            _config = config.Value;
            _urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
        }

        public EmailTemplate Find(int id)
        {
            return _emailTemplatesRepository.Find(id);
        }

        public string ParseForUser(int emailTemplateId, string title, User user, object data)
        {
            if (user == null)
            {
                throw new Exception("User is null");
            }

            return Parse(emailTemplateId, user.FirstName,
                _urlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }), data);
        }

        public string ParseForContact(int emailTemplateId, string title, Contact contact, object data)
        {
            if (contact == null)
            {
                throw new Exception("Contact is null");
            }

            return Parse(emailTemplateId, contact.FirstName,
                _urlHelper.AbsoluteAction("UnsubscribeContact", "Account", new { externalId = contact.Name }), data);
        }

        public string ParseForUser(string title, string body, User user, object data)
        {
            if (user == null)
            {
                throw new Exception("User is null");
            }

            return Parse(title, body, user.FirstName,
                _urlHelper.AbsoluteAction("UnsubscribeUser", "Account", new { externalId = user.Name }), data);
        }

        public string ParseForContact(string title, string body, Contact contact, object data)
        {
            if (contact == null)
            {
                throw new Exception("Contact is null");
            }

            return Parse(title, body, contact.FirstName,
                _urlHelper.AbsoluteAction("UnsubscribeContact", "Account", new { externalId = contact.Name }), data);
        }

        private string Parse(int emailTemplateId, string recipientFirstName, string unsubscribeLink, object data)
        {
            var template = Find(emailTemplateId);
            if (template == null)
            {
                throw new Exception($"Email Template {emailTemplateId} was not found");
            }

            return Parse(template.Subject, TemplateParser.Parse(template.HtmlBody, data), recipientFirstName, unsubscribeLink, data);
        }

        private string Parse(string title, string body, string recipientFirstName, string unsubscribeLink, object data)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new Exception("Title cannot be empty");
            }

            if (string.IsNullOrEmpty(body))
            {
                throw new Exception("Body cannot be empty");
            }

            if (string.IsNullOrEmpty(recipientFirstName))
            {
                throw new Exception("Recipient first name cannot be empty");
            }

            if (string.IsNullOrEmpty(unsubscribeLink))
            {
                throw new Exception("Unsubscribe link cannot be empty");
            }

            body = TemplateParser.Parse(body, data);

            return TemplateParser.Parse(Globalisation.Dictionary.BaseEmailTemplate, new
            {
                Title = title,
                FirstName = recipientFirstName,
                Body = body,
                PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy",
                "Home"),
                TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                UnsubscribeLink = unsubscribeLink,
                DateTime.Now.Year,
                _config.CompanyName,
                _defaultValuesConfig.CompanyAddress,
                HeaderImageSrc = $"{_defaultValuesConfig.BaseEmailTemplateImagesPath}/emailtemplates/email-template-header.jpg",
                CompanyLogoSrc = $"{_defaultValuesConfig.BaseEmailTemplateImagesPath}/company/logo-small.png",
            });
        }
    }
}