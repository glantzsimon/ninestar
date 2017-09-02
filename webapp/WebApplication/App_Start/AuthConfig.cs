
using System;
using System.IO;
using K9.DataAccess.Database;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Config;

namespace K9.WebApplication
{
	public class AuthConfig
	{
		public static void InitialiseWebSecurity()
		{
			DatabaseInitialiser.InitialiseWebsecurity();

			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/appsettings.json"));
			var config = ConfigHelper.GetConfiguration<OAuthConfiguration>(json).Value;
		}
	}
}