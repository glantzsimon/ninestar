
using System.Collections.Specialized;

namespace K9.SharedLibrary.Helpers
{
	public static class ConfigurationFileHelpers
	{

		public static string GetValue(this NameValueCollection appSettings, string key)
		{
			return appSettings[key];
		}

		public static int GetValueAsInteger(this NameValueCollection appSettings, string key)
		{
			var value = 0;
			int.TryParse(appSettings[key], out value);
			return value;
		}

		public static bool GetValueAsBoolean(this NameValueCollection appSettings, string key)
		{
			var value = false;
			bool.TryParse(appSettings[key], out value);
			return value;
		}
	}
}
