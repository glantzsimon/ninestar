using System.Threading.Tasks;

namespace K9.WebApplication.Helpers
{
    public interface IMailer
    {
        void SendEmail(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true, bool isSSL = true, int port = 465);
        Task SendEmailAsync(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true, bool isSSL = true, int port = 465);
    }
}
