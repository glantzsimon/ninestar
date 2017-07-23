using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Helpers
{
	public class Mailer : IMailer
	{
		private readonly ISmtpConfiguration _config;

		public Mailer(ISmtpConfiguration config)
		{
			_config = config;
		}

		public void SendEmail(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true)
		{
			fromEmailAddress = string.IsNullOrEmpty(fromEmailAddress) ? _config.SmtpFromEmailAddress : fromEmailAddress;
			fromDisplayName = string.IsNullOrEmpty(fromDisplayName) ? _config.SmtpFromDisplayName : fromDisplayName;

			var from = new MailAddress(fromEmailAddress, fromDisplayName);
			var recipient = new MailAddress(recipientEmailAddress, recipientDisplayName);

			var message = new MailMessage(from, recipient);
			message.IsBodyHtml = isHtml;
			message.Subject = subject;
			message.Body = body;

			var emailClient = new SmtpClient(_config.SmtpServer);
			var smtpUserInfo = new NetworkCredential(_config.SmtpUserId, _config.SmtpPassword);

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
