using System.Threading.Tasks;
using MailKit.Security;

namespace K9.SharedLibrary.Helpers
{
    public interface IMailer
    {
        void SendEmail(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true, SecureSocketOptions socketOptions = SecureSocketOptions.Auto, int? port = null);
        Task SendEmailAsync(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true, SecureSocketOptions socketOptions = SecureSocketOptions.Auto, int? port = null);
    }
}
