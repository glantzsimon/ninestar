using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
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
        private readonly IRepository<EmailQueueItem> _emailQueueItemsRepository;
        private readonly IRepository<User> _usersRepository;
        private readonly IRepository<Contact> _contactsRepository;
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;
        private readonly IRepository<UserMembership> _userMembershipsRepository;
        private readonly ILogger _logger;
        private readonly SmtpConfiguration _smtpConfig;
        private readonly IMailer _mailer;
        private readonly DefaultValuesConfiguration _defaultConfig;

        public EmailQueueService(IRepository<EmailQueueItem> emailQueueItemsRepository, IRepository<User> usersRepository, IRepository<Contact> contactsRepository, IRepository<MembershipOption> membershipOptionsRepository, IRepository<UserMembership> userMembershipsRepository, ILogger logger, IOptions<DefaultValuesConfiguration> defaultConfig, IOptions<SmtpConfiguration> smtpConfig, IMailer mailer)
        {
            _emailQueueItemsRepository = emailQueueItemsRepository;
            _usersRepository = usersRepository;
            _contactsRepository = contactsRepository;
            _membershipOptionsRepository = membershipOptionsRepository;
            _userMembershipsRepository = userMembershipsRepository;
            _logger = logger;
            _smtpConfig = smtpConfig.Value;
            _mailer = mailer;
            _defaultConfig = defaultConfig.Value;
        }

        public void AddEmailToQueue(string recipientEmailAddress, string recipientFirstName, string recipientFullName, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null)
        {
            var contact = _contactsRepository.Find(e => e.EmailAddress == recipientEmailAddress).FirstOrDefault();
            if (contact == null)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueue => Contact not found: {recipientEmailAddress}");
                throw new Exception("Contact not found");
            }

            AddEmailToQueueForContact(contact.Id, subject, body, type);
        }

        public void AddEmailToQueueForContact(int contactId, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null)
        {
            var contact = _contactsRepository.Find(contactId);
            if (contact == null)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueueForContact => Contact not found. ContactId: {contactId}");
                throw new Exception("Contact not found");
            }

            _emailQueueItemsRepository.Create(new EmailQueueItem
            {
                Type = type,
                ContactId = contactId,
                Subject = subject,
                Body = body,
                ScheduleOn = scheduledOn.HasValue ? DateTime.Now.Add(scheduledOn.Value) : (DateTime?)null
            });
        }

        public void AddEmailToQueueForUser(int userId, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null)
        {
            var user = _usersRepository.Find(userId);
            if (user == null)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueueForUser => User not found. UserId: {userId}");
                throw new Exception("User not found");
            }

            _emailQueueItemsRepository.Create(new EmailQueueItem
            {
                Name = Guid.NewGuid().ToString(),
                Type = type,
                UserId = userId,
                Subject = subject,
                Body = body,
                ScheduleOn = scheduledOn.HasValue ? DateTime.Now.Add(scheduledOn.Value) : (DateTime?)null
            });
        }

        public void ProcessQueue()
        {
            var emailsToSend = _emailQueueItemsRepository.Find(e => !e.SentOn.HasValue && (!e.ScheduleOn.HasValue || e.ScheduleOn.Value >= DateTime.Now))
                .OrderBy(e => e.Id)
                .Take(_defaultConfig.EmailQueueMaxBatchSize).ToList();

            _logger.Log(LogLevel.Info, $"EmailQueueService => ProcessQueue => Sending emails => Batch Size: {_defaultConfig.EmailQueueMaxBatchSize}");

            foreach (var email in emailsToSend)
            {
                try
                {
                    if (email.UserId.HasValue)
                    {
                        var user = _usersRepository.Find(email.UserId.Value);

                        if (user == null)
                        {
                            _logger.Error($"EmailQueueService => ProcessQueue => User not found: UserId {email.UserId}");
                            continue;
                        }

                        if (user.IsUnsubscribed)
                        {
                            _logger.Info($"EmailQueueService => ProcessQueue => User with UserId {email.UserId} has unsubscribed. Cannot send email: {email.Subject}");
                            continue;
                        }

                        if (email.Type == EEmailType.MembershipPromotion)
                        {
                            var userMemberships = _userMembershipsRepository
                                .Find(e => e.UserId == email.UserId)
                                .Where(e => e.IsActive)
                                .ToList();
                            var maxSubscriptionType = _membershipOptionsRepository
                                .Find(e => userMemberships.Select(m => m.MembershipOptionId).Contains(e.Id))
                                .Max(e => e.SubscriptionType);

                            if (maxSubscriptionType > MembershipOption.ESubscriptionType.Free)
                            {
                                _logger.Info($"EmailQueueService => ProcessQueue => User with UserId {email.UserId} has already upgraded their membership to {maxSubscriptionType.ToString().SplitOnCapitalLetter()}. Cannot send email: {email.Subject}");
                                continue;
                            }
                        }

                        SendEmail(email);
                    }
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
                _smtpConfig.SmtpFromEmailAddress, _smtpConfig.SmtpFromDisplayName);
        }

    }
}