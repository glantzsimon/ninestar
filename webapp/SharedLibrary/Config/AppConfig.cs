
using System.Configuration;
using K9.SharedLibrary.Extensions;

namespace K9.SharedLibrary.Config
{
	public class AppConfig
	{
		public static string SmtpServer
		{
			get { return ConfigurationManager.AppSettings.GetValue("SmtpServer"); }
		}

		public static string SmtpUserId
		{
			get { return ConfigurationManager.AppSettings.GetValue("SmtpUserId"); }
		}

		public static string SmtpPassword
		{
			get { return ConfigurationManager.AppSettings.GetValue("SmtpPassword"); }
		}

		public static string SmtpFromEmailAddress
		{
			get { return ConfigurationManager.AppSettings.GetValue("SmtpFromEmailAddress"); }
		}

		public static string SmtpFromDisplayName
		{
			get { return ConfigurationManager.AppSettings.GetValue("SmtpFromDisplayName"); }
		}
	}
}
