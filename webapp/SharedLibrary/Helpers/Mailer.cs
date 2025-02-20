using K9.SharedLibrary.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;

namespace K9.SharedLibrary.Helpers
{
    public class Mailer : IMailer
    {
        private readonly SmtpConfiguration _config;

        public Mailer(IOptions<SmtpConfiguration> config)
        {
            _config = config.Value;
        }

        public void SendEmail(string subject, string body, string recipientEmailAddress, string recipientDisplayName, bool isHtml = true, SecureSocketOptions socketOptions = SecureSocketOptions.Auto, int? port = null)
        {
            SendEmail(subject, body, recipientEmailAddress, recipientDisplayName, _config.SmtpFromEmailAddress,
                _config.SmtpFromDisplayName, isHtml, socketOptions, port);
        }

        public void SendEmail(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress, string fromDisplayName, bool isHtml = true, SecureSocketOptions socketOptions = SecureSocketOptions.Auto, int? port = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromDisplayName, fromEmailAddress));
            message.To.Add(new MailboxAddress(recipientDisplayName, recipientEmailAddress));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_config.SmtpServer, port.HasValue ? port.Value : _config.SmtpPort, socketOptions);
                client.Authenticate(_config.SmtpUserId, _config.SmtpPassword);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public Task SendEmailAsync(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true, SecureSocketOptions socketOptions = SecureSocketOptions.Auto, int? port = null)
        {
            return Task.Factory.StartNew(() =>
            {
                SendEmail(subject, body, recipientEmailAddress, recipientDisplayName, fromEmailAddress, fromDisplayName, isHtml, socketOptions, port);
            });
        }

    }
}
