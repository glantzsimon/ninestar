using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Helpers
{
	public class Mailer : IMailer
	{
		private readonly IOptions<SmtpConfiguration> _config;

		public Mailer(IOptions<SmtpConfiguration> config)
		{
			_config = config;
		}

		public void SendEmail(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true)
		{
			fromEmailAddress = string.IsNullOrEmpty(fromEmailAddress) ? _config.Value.SmtpFromEmailAddress : fromEmailAddress;
			fromDisplayName = string.IsNullOrEmpty(fromDisplayName) ? _config.Value.SmtpFromDisplayName : fromDisplayName;

			var from = new MailAddress(fromEmailAddress, fromDisplayName);
			var recipient = new MailAddress(recipientEmailAddress, recipientDisplayName);

			var message = new MailMessage(from, recipient);
			message.IsBodyHtml = isHtml;
			message.Subject = subject;
			message.Body = body;

			var emailClient = new SmtpClient(_config.Value.SmtpServer);
			var smtpUserInfo = new NetworkCredential(_config.Value.SmtpUserId, _config.Value.SmtpPassword);

			emailClient.UseDefaultCredentials = false;
			emailClient.Credentials = smtpUserInfo;
			emailClient.Send(message);
		}

		public Task SendEmailAsync(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true)
		{
			return Task.Factory.StartNew(() =>
			{
				SendEmail(subject, body, recipientEmailAddress, recipientDisplayName, fromEmailAddress, fromDisplayName, isHtml);
			});
		}

	}
}
