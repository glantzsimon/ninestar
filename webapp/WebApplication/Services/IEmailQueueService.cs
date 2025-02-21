using System;
using K9.DataAccessLayer.Enums;

namespace K9.WebApplication.Services
{
    public interface IEmailQueueService 
    {
        void AddEmailToQueue(string recipientEmailAddress, string recipientFirstName, string recipientFullName, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null);
        void AddEmailToQueueForContact(int contactId, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null);
        void AddEmailToQueueForUser(int userId, string subject, string body, EEmailType type = EEmailType.General, TimeSpan? scheduledOn = null);
        void ProcessQueue();
    }
}