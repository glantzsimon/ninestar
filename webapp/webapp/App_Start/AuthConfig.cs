
namespace K9.WebApplication
{
	public class AuthConfig
	{
		public static void InitialiseWebSecurity()
		{
			DataAccess.Database.DatabaseInitialiser.InitialiseWebsecurity();
		}
	}
}