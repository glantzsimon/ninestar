using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly IRepository<Contact> _contactsRepository;
        private readonly ILogger _logger;
        private readonly IRepository<User> _usersRepository;
        private readonly IRepository<EmailQueueItem> _emailQueueItemsRepository;
        private readonly IMailer _mailer;
        private readonly IContactService _contactService;
        private readonly DefaultValuesConfiguration _defaultConfig;
        private readonly SmtpConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public EmailQueueService(ILogger logger, IRepository<Contact> contactsRepository, IRepository<User> usersRepository, IRepository<EmailQueueItem> emailQueueItemsRepository, IMailer mailer, IOptions<SmtpConfiguration> config, IOptions<DefaultValuesConfiguration> defaultConfig, IContactService contactService)
        {
            _contactsRepository = contactsRepository;
            _logger = logger;
            _usersRepository = usersRepository;
            _emailQueueItemsRepository = emailQueueItemsRepository;
            _mailer = mailer;
            _contactService = contactService;
            _defaultConfig = defaultConfig.Value;
            _config = config.Value;
            _urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
        }

        public void AddEmailToQueue(string recipientEmailAddress, string recipientFirstName, string recipientFullName, string subject, string body, bool useDefaultTemplate = true)
        {
            if (useDefaultTemplate)
            {
                var contact = _contactService.Find(recipientEmailAddress);
                if (contact == null)
                {
                    _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueue => Contact not found: {recipientEmailAddress}");
                    throw new Exception("Contact not found");
                }

                AddEmailToQueueForContact(contact.Id, subject, body, useDefaultTemplate);
            }
        }

        public void AddEmailToQueueForContact(int contactId, string subject, string body, bool useDefaultTemplate = true)
        {
            var contact = _contactService.Find(contactId);
            if (contact == null)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueueForContact => Contact not found. ContactId: {contactId}");
                throw new Exception("Contact not found");
            }

            if (useDefaultTemplate)
            {
                body = TemplateParser.Parse(Globalisation.Dictionary.DefaultEmailTemplate, new
                {
                    Subject = subject,
                    contact.FirstName,
                    Body = body,
                    PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                    TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                    UnsubscribeLink =
                    _urlHelper.AbsoluteAction("UnsubscribeContact", "Account", new {externalId = contact.Name}),
                });
            }

            _emailQueueItemsRepository.Create(new EmailQueueItem
            {
                RecipientName = contact.FirstName,
                RecipientEmailAddress = contact.EmailAddress,
                Subject = subject,
                Body = body
            });
        }

        public void AddEmailToQueueForUser(int userId, string subject, string body, bool useDefaultTemplate = true)
        {
            var user = _usersRepository.Find(userId);
            if (user == null)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueueForUser => User not found. UserId: {userId}");
                throw new Exception("User not found");
            }

            if (useDefaultTemplate)
            {
                body = TemplateParser.Parse(Globalisation.Dictionary.DefaultEmailTemplate, new
                {
                    Subject = subject,
                    user.FirstName,
                    Body = body,
                    PrivacyPolicyLink = _urlHelper.AbsoluteAction("PrivacyPolicy", "Home"),
                    TermsOfServiceLink = _urlHelper.AbsoluteAction("TermsOfService", "Home"),
                    UnsubscribeLink =
                    _urlHelper.AbsoluteAction("UnsubscribeUser", "Account", new {externalId = user.Name}),
                });
            }

            _emailQueueItemsRepository.Create(new EmailQueueItem
            {
                RecipientName = user.FirstName,
                RecipientEmailAddress = user.EmailAddress,
                Subject = subject,
                Body = body
            });
        }

        public void ProcessQueue()
        {
            var emailsToSend = _emailQueueItemsRepository.Find(e => !e.SentOn.HasValue)
                .OrderBy(e => e.Id)
                .Take(_defaultConfig.EmailQueueMaxBatchSize).ToList();

            _logger.Log(LogLevel.Info, $"EmailQueueService => ProcessQueue => Sending emails");

            foreach (var email in emailsToSend)
            {
                try
                {
                    SendEmail(email);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, $"EmailQueueService => ProcessQueue => Error sending mail with mailId: {email.Id} => {ex.GetFullErrorMessage()}");
                    continue;
                }

                try
                {
                    email.SentOn = DateTime.UtcNow;
                    _emailQueueItemsRepository.Update(email);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, $"EmailQueueService => ProcessQueue => Email sent but error updating mail with maildId: {email.Id} => Error: {e.GetFullErrorMessage()}");
                }
            }
        }

        private void SendEmail(EmailQueueItem email)
        {
            _mailer.SendEmail(email.Subject, email.Body, email.RecipientEmailAddress, email.RecipientName,
                _config.SmtpFromEmailAddress, _config.SmtpFromDisplayName);
        }

    }
}