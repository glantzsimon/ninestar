using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using NLog;
using System;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class EmailQueueService : BaseService, IEmailQueueService
    {
        private readonly IContactService _contactService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IRepository<EmailQueueItem> _emailQueueItemsRepository;

        public EmailQueueService(INineStarKiBasePackage my, IContactService contactService, IEmailTemplateService emailTemplateService, IRepository<EmailQueueItem> emailQueueItemsRepository)
            : base(my)
        {
            _contactService = contactService;
            _emailTemplateService = emailTemplateService;
            _emailQueueItemsRepository = emailQueueItemsRepository;
        }

        public void AddEmailToQueue(string recipientEmailAddress, string recipientFirstName, string recipientFullName, string subject, string body)
        {
                var contact = _contactService.Find(recipientEmailAddress);
                if (contact == null)
                {
                    My.Logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueue => Contact not found: {recipientEmailAddress}");
                    throw new Exception("Contact not found");
                }

                AddEmailToQueueForContact(contact.Id, subject, body);
        }

        public void AddEmailToQueueForContact(int contactId, string subject, string body)
        {
            var contact = _contactService.Find(contactId);
            if (contact == null)
            {
                My.Logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueueForContact => Contact not found. ContactId: {contactId}");
                throw new Exception("Contact not found");
            }

            var templateBody = _emailTemplateService.ParseForContact(
                subject,
                body,
                contact,
                null);

            _emailQueueItemsRepository.Create(new EmailQueueItem
            {
                RecipientName = contact.FirstName,
                RecipientEmailAddress = contact.EmailAddress,
                Subject = subject,
                Body = body
            });
        }

        public void AddEmailToQueueForUser(int userId, string subject, string body)
        {
            var user = My.UsersRepository.Find(userId);
            if (user == null)
            {
                My.Logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueueForUser => User not found. UserId: {userId}");
                throw new Exception("User not found");
            }

            var templateBody = _emailTemplateService.ParseForUser(
                subject,
                body,
                user,
                null);
            
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
                .Take(My.DefaultValuesConfiguration.EmailQueueMaxBatchSize).ToList();

            My.Logger.Log(LogLevel.Info, $"EmailQueueService => ProcessQueue => Sending emails");

            foreach (var email in emailsToSend)
            {
                try
                {
                    SendEmail(email);
                }
                catch (Exception ex)
                {
                    My.Logger.Log(LogLevel.Error, $"EmailQueueService => ProcessQueue => Error sending mail with mailId: {email.Id} => {ex.GetFullErrorMessage()}");
                    continue;
                }

                try
                {
                    email.SentOn = DateTime.UtcNow;
                    _emailQueueItemsRepository.Update(email);
                }
                catch (Exception e)
                {
                    My.Logger.Log(LogLevel.Error, $"EmailQueueService => ProcessQueue => Email sent but error updating mail with maildId: {email.Id} => Error: {e.GetFullErrorMessage()}");
                }
            }
        }

        private void SendEmail(EmailQueueItem email)
        {
            My.Mailer.SendEmail(email.Subject, email.Body, email.RecipientEmailAddress, email.RecipientName,
                My.SmtpConfiguration.SmtpFromEmailAddress, My.SmtpConfiguration.SmtpFromDisplayName);
        }

    }
}