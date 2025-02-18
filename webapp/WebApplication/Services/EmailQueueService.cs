using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using NLog;
using System;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly IRepository<Contact> _contactsRepository;
        private readonly ILogger _logger;
        private readonly IRepository<User> _usersRepository;
        private readonly IRepository<EmailQueueItem> _emailQueueItemsRepository;
        private readonly IMailer _mailer;
        private readonly DefaultValuesConfiguration _defaultConfig;
        private readonly SmtpConfiguration _config;

        public EmailQueueService(ILogger logger, IRepository<Contact> contactsRepository, IRepository<User> usersRepository, IRepository<EmailQueueItem> emailQueueItemsRepository, IMailer mailer, IOptions<SmtpConfiguration> config, IOptions<DefaultValuesConfiguration> defaultConfig)
        {
            _contactsRepository = contactsRepository;
            _logger = logger;
            _usersRepository = usersRepository;
            _emailQueueItemsRepository = emailQueueItemsRepository;
            _mailer = mailer;
            _defaultConfig = defaultConfig.Value;
            _config = config.Value;
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