
using System.Configuration;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccess.Config
{
	public class AppConfig
	{
		public static bool AutomaticMigrationsEnabled
		{
			get { return ConfigurationManager.AppSettings.GetValueAsBoolean("AutomaticMigrationsEnabled"); }
		}

		public static bool AutomaticMigrationDataLossAllowed
		{
			get { return ConfigurationManager.AppSettings.GetValueAsBoolean("AutomaticMigrationDataLossAllowed"); }
		}

		public static string SystemUserPassword
		{
			get { return ConfigurationManager.AppSettings.GetValue("SystemUserPassword"); }
		}
	}
}
