namespace K9.WebApplication.Services
{
    public interface IEmailQueueService
    {
        void AddEmailToQueue(string recipientEmailAddress, string recipientFirstName, string recipientFullName, string subject, string body, bool useDefaultTemplate = true);
        void AddEmailToQueueForContact(int contactId, string subject, string body, bool useDefaultTemplate = true);
        void AddEmailToQueueForUser(int userId, string subject, string body, bool useDefaultTemplate = true);
        void ProcessQueue();
    }
}