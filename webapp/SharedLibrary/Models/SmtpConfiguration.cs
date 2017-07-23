
namespace K9.SharedLibrary.Models
{
	public class SmtpConfiguration : ISmtpConfiguration, IConfigurable
	{
		public string SmtpServer { get; set; }
		public string SmtpUserId { get; set; }
		public string SmtpPassword { get; set; }
		public string SmtpFromEmailAddress { get; set; }
		public string SmtpFromDisplayName { get; set; }
	}
}
