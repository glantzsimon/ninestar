using System.Threading.Tasks;

namespace K9.SharedLibrary.Helpers
{
	public interface IMailer
	{
		void SendEmail(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true);
		Task SendEmailAsync(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true);
	}
}
