using System;
using K9.DataAccessLayer.Enums;

namespace K9.WebApplication.Services
{
    public interface IEmailQueueService
    {
        void AddEmailToQueue(int emailTemplateId, string recipientEmailAddress, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null);
        void AddEmailToQueueForContact(int emailTemplateId, int contactId, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null);
        void AddEmailToQueueForUser(int emailTemplateId, int userId, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null);
        void ProcessQueue();
    }
}