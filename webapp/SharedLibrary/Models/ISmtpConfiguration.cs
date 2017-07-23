
namespace K9.SharedLibrary.Models
{
	public interface ISmtpConfiguration
	{
		string SmtpServer { get;set; }
		string SmtpUserId { get; set; }
		string SmtpPassword { get; set; }
		string SmtpFromEmailAddress { get; set; }
		string SmtpFromDisplayName { get; set; }
	}
}
