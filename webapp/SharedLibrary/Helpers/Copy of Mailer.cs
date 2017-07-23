using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using K9.SharedLibrary.Config;

namespace K9.SharedLibrary.Helpers
{
	//public static class Mailer
	//{

	//    #region Properties

	//    public static string SmtpServer = AppConfig.SmtpServer;
	//    public static string SmtpUserId = AppConfig.SmtpUserId;
	//    public static string SmtpPassword = AppConfig.SmtpPassword;
	//    public static string SmtpFromEmailAddress = AppConfig.SmtpFromEmailAddress;
	//    public static string SmtpFromDisplayName = AppConfig.SmtpFromDisplayName;

	//    #endregion


	//    #region Methods

	//    public static void SendEmail(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true)
	//    {
	//        fromEmailAddress = string.IsNullOrEmpty(fromEmailAddress) ? SmtpFromEmailAddress : fromEmailAddress;
	//        fromDisplayName = string.IsNullOrEmpty(fromDisplayName) ? SmtpFromDisplayName : fromDisplayName;

	//        var from = new MailAddress(fromEmailAddress, fromDisplayName);
	//        var recipient = new MailAddress(recipientEmailAddress, recipientDisplayName);

	//        var message = new MailMessage(from, recipient);
	//        message.IsBodyHtml = isHtml;
	//        message.Subject = subject;
	//        message.Body = body;

	//        var emailClient = new SmtpClient(SmtpServer);
	//        var smtpUserInfo = new NetworkCredential(SmtpUserId, SmtpPassword);

	//        emailClient.UseDefaultCredentials = false;
	//        emailClient.Credentials = smtpUserInfo;
	//        emailClient.Send(message);
	//    }

	//    public static Task SendEmailAsync(string subject, string body, string recipientEmailAddress, string recipientDisplayName, string fromEmailAddress = "", string fromDisplayName = "", bool isHtml = true)
	//    {
	//        return Task.Factory.StartNew(() =>
	//        {
	//            SendEmail(subject, body, recipientEmailAddress, recipientDisplayName, fromEmailAddress, fromDisplayName, isHtml);
	//        });
	//    }

	//    #endregion

	//}
}
