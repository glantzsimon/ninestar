using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Exceptions;
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
        private readonly IRepository<EmailTemplate> _emailTemplatesRepository;
        private readonly IRepository<SystemSetting> _systemSettingsRepository;
        private readonly ILogger _logger;
        private readonly SmtpConfiguration _smtpConfig;
        private readonly IMailer _mailer;
        private readonly DefaultValuesConfiguration _defaultConfig;

        public EmailQueueService(IRepository<EmailQueueItem> emailQueueItemsRepository, IRepository<User> usersRepository, IRepository<Contact> contactsRepository, IRepository<MembershipOption> membershipOptionsRepository, IRepository<UserMembership> userMembershipsRepository, IRepository<EmailTemplate> emailTemplatesRepository, IRepository<SystemSetting> systemSettingsRepository, ILogger logger, IOptions<DefaultValuesConfiguration> defaultConfig, IOptions<SmtpConfiguration> smtpConfig, IMailer mailer)
        {
            _emailQueueItemsRepository = emailQueueItemsRepository;
            _usersRepository = usersRepository;
            _contactsRepository = contactsRepository;
            _membershipOptionsRepository = membershipOptionsRepository;
            _userMembershipsRepository = userMembershipsRepository;
            _emailTemplatesRepository = emailTemplatesRepository;
            _systemSettingsRepository = systemSettingsRepository;
            _logger = logger;
            _smtpConfig = smtpConfig.Value;
            _mailer = mailer;
            _defaultConfig = defaultConfig.Value;
        }

        public void AddEmailToQueue(int emailTemplateId, string recipientEmailAddress, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null)
        {
            var contact = _contactsRepository.Find(e => e.EmailAddress == recipientEmailAddress).FirstOrDefault();
            if (contact == null)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueue => Contact not found: {recipientEmailAddress}");
                throw new Exception("Contact not found");
            }

            AddEmailToQueueForContact(emailTemplateId, contact.Id, subject, body, type);
        }

        public void AddEmailToQueueForContact(int emailTemplateId, int contactId, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null)
        {
            var contact = _contactsRepository.Find(contactId);
            if (contact == null)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueueForContact => Contact not found. ContactId: {contactId}");
                throw new Exception("Contact not found");
            }

            AddEmailToQueue(emailTemplateId, null, contactId, subject, body, type, scheduledOn);
        }

        public void AddEmailToQueueForUser(int emailTemplateId, int userId, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null, bool isTest = false)
        {
            var user = _usersRepository.Find(userId);
            if (user == null)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => AddEmailToQueueForUser => User not found. UserId: {userId}");
                throw new Exception("User not found");
            }

            AddEmailToQueue(emailTemplateId, userId, null, subject, body, type, scheduledOn, isTest);
        }

        public void ProcessQueue()
        {
            var systemSettings = _systemSettingsRepository.List().FirstOrDefault() ?? new SystemSetting();
            if (!systemSettings.IsPausedEmailJobQueue)
            {
                var now = DateTime.Now;
                var emailsToSend = _emailQueueItemsRepository.Find(e =>
                        !e.IsProcessed &&
                        !e.SentOn.HasValue &&
                        ((e.ScheduledOn.HasValue && e.ScheduledOn.Value <= now) || !e.ScheduledOn.HasValue))
                    .OrderBy(e => e.Id)
                    .Take(_defaultConfig.EmailQueueMaxBatchSize)
                    .Select(e =>
                    {
                        if (e.UserId.HasValue)
                        {
                            e.User = _usersRepository.Find(u => u.Id == e.UserId).FirstOrDefault();
                        }
                        else if (e.ContactId.HasValue)
                        {
                            e.Contact = _contactsRepository.Find(c => c.Id == e.ContactId).FirstOrDefault();
                        }

                        return e;
                    })
                    .ToList();

                _logger.Log(LogLevel.Info,
                    $"EmailQueueService => ProcessQueue => Sending {emailsToSend.Count} emails => Batch Size: {_defaultConfig.EmailQueueMaxBatchSize}");

                foreach (var email in emailsToSend)
                {
                    try
                    {
                        if (!email.UserId.HasValue && !email.ContactId.HasValue)
                        {
                            var error =
                                "EmailQueueService => ProcessQueue => Email cannot be sent. Both contactId and userId are null";
                            _logger.Error(error);
                            MarkEmailAsProcessed(email, error);
                            continue;
                        }

                        if (email.UserId.HasValue)
                        {
                            if (email.User == null)
                            {
                                var error =
                                    $"EmailQueueService => ProcessQueue => User not found: UserId {email.UserId}";
                                _logger.Error(error);
                                MarkEmailAsProcessed(email, error);
                                continue;
                            }

                            if (email.User.IsUnsubscribed)
                            {
                                var resultText =
                                    $"EmailQueueService => ProcessQueue => User with UserId {email.UserId} has unsubscribed. Cannot send email: {email.Subject}";
                                _logger.Info(resultText);
                                MarkEmailAsProcessed(email, resultText);
                                continue;
                            }

                            if (email.Type == EEmailType.MembershipPromotion)
                            {
                                var userMemberships = _userMembershipsRepository
                                    .Find(e => e.UserId == email.UserId)
                                    .Select(e =>
                                    {
                                        e.MembershipOption = _membershipOptionsRepository
                                            .Find(o => o.Id == e.MembershipOptionId).FirstOrDefault();
                                        return e;
                                    })
                                    .Where(e => e.IsActive)
                                    .ToList();

                                var maxSubscriptionType = userMemberships.Max(e => e.MembershipOption.SubscriptionType);
                                if (maxSubscriptionType > MembershipOption.ESubscriptionType.Free)
                                {
                                    var resultText =
                                        $"EmailQueueService => ProcessQueue => User with UserId {email.UserId} has already upgraded their membership to {maxSubscriptionType.ToString().SplitOnCapitalLetter()}. Cannot send email: {email.Subject}";
                                    _logger.Info(resultText);
                                    MarkEmailAsProcessed(email, resultText);
                                    continue;
                                }
                            }
                        }
                        else if (email.ContactId.HasValue)
                        {
                            if (email.Contact == null)
                            {
                                var error =
                                    $"EmailQueueService => ProcessQueue => Contact not found: ContactId {email.ContactId}";
                                _logger.Error(error);
                                MarkEmailAsProcessed(email, error);
                                continue;
                            }

                            if (email.Contact.IsUnsubscribed)
                            {
                                var resultText =
                                    $"EmailQueueService => ProcessQueue => Contact with ContactId {email.ContactId} has unsubscribed. Cannot send email: {email.Subject}";
                                _logger.Info(resultText);
                                MarkEmailAsProcessed(email, resultText);
                                continue;
                            }
                        }

                        SendEmail(email);
                    }
                    catch (Exception ex)
                    {
                        var error =
                            $"EmailQueueService => ProcessQueue => Error sending mail with mailId: {email.Id} => {ex.GetFullErrorMessage()}";
                        _logger.Log(LogLevel.Error, error);

                        try
                        {
                            email.Result = error;
                            _emailQueueItemsRepository.Update(email);
                        }
                        catch (Exception e)
                        {
                        }

                        continue;
                    }

                    try
                    {
                        email.SentOn = DateTime.Now;
                        _emailQueueItemsRepository.Update(email);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogLevel.Error,
                            $"EmailQueueService => ProcessQueue => Email sent but error updating mail with maildId: {email.Id} => Error: {e.GetFullErrorMessage()}");
                    }
                }
            }
        }

        private void MarkEmailAsProcessed(EmailQueueItem email, string result)
        {
            try
            {
                email.Result = result;
                email.IsProcessed = true;
                _emailQueueItemsRepository.Update(email);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"EmailQueueService => ProcessQueue => MarkEmailAsProcessed: {email.Id} => Error: {e.GetFullErrorMessage()}");
            }
        }

        private void AddEmailToQueue(int emailTemplateId, int? userId, int? contactId, string subject, string body, EEmailType type, TimeSpan? scheduledOn, bool allowResend = false)
        {
            var _90DaysAgo = DateTime.Today.Subtract(TimeSpan.FromDays(90));

            if (_emailQueueItemsRepository.Exists(e =>
                    e.EmailTemplateId == emailTemplateId &&
                    ((e.UserId.HasValue && e.UserId == userId) || (e.ContactId.HasValue && e.ContactId == e.ContactId)) &&
                    (!e.SentOn.HasValue || e.SentOn >= _90DaysAgo && !allowResend)))
            {
                var user = userId.HasValue ? _usersRepository.Find(userId.Value) : null;
                var contact = contactId.HasValue ? _contactsRepository.Find(contactId.Value) : null;
                var person = userId.HasValue ? $"user {user.FullName} (UserId {userId})" : $"contact {contact.FullName} (ContactId {contactId})";
                var emailTemplate = _emailTemplatesRepository.Find(emailTemplateId);
                var errorMessage = $"EmailQueueService => AddEmailToQueue => Email template '{emailTemplate.Name}' (Id {emailTemplateId}) has already been sent to {person} in the last 90 days";
                _logger.Log(LogLevel.Error, errorMessage);
                throw new EmailTemplateAlreadySentException(errorMessage);
            }

            _emailQueueItemsRepository.Create(new EmailQueueItem
            {
                Name = Guid.NewGuid().ToString(),
                Type = type,
                UserId = userId,
                ContactId = contactId,
                EmailTemplateId = emailTemplateId,
                Subject = subject,
                Body = body,
                ScheduledOn = scheduledOn.HasValue ? DateTime.Now.Add(scheduledOn.Value) : (DateTime?)null
            });
        }

        private void SendEmail(EmailQueueItem email)
        {
            _mailer.SendEmail(email.Subject, email.Body, email.RecipientEmailAddress, email.RecipientName,
                _smtpConfig.SmtpFromEmailAddress, _smtpConfig.SmtpFromDisplayName);
        }

    }
}