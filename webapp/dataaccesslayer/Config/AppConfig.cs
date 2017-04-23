
using System.Configuration;
using K9.SharedLibrary.Helpers;

namespace K9.DataAccess.Config
{
	public class AppConfig
	{
		public static string SystemUserPassword
		{
			get { return ConfigurationManager.AppSettings.GetValue("SystemUserPassword"); }
		}
	}
}
