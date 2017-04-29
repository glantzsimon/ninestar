using System.Configuration;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Config
{
	public static class AppConfig
	{
		public static string FacebookAppId
		{
			get { return ConfigurationManager.AppSettings.GetValue("FacebookAppId"); }
		}

		public static string FacebookAppSecret
		{
			get { return ConfigurationManager.AppSettings.GetValue("FacebookAppSecret"); }
		}

		public static string CompanyLogoUrl
		{
			get { return ConfigurationManager.AppSettings.GetValue("CompanyLogoUrl"); }
		}

		public static string CompanyName
		{
			get { return ConfigurationManager.AppSettings.GetValue("CompanyName"); }
		}

		public static string DefaultInputSize
		{
			get { return ConfigurationManager.AppSettings.GetValue("DefaultInputSize"); }
		}
		
	}
}