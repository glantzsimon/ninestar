
using K9.WebApplication.Config;
using Microsoft.Web.WebPages.OAuth;

namespace K9.WebApplication
{
	public class AuthConfig
	{
		public static void InitialiseWebSecurity()
		{
			DataAccess.Database.DatabaseInitialiser.InitialiseWebsecurity();

			OAuthWebSecurity.RegisterFacebookClient(AppConfig.FacebookAppId, AppConfig.FacebookAppSecret);
			OAuthWebSecurity.RegisterGoogleClient();
		}
	}
}