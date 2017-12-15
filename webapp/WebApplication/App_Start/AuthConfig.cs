
using K9.Base.DataAccessLayer.Database;

namespace K9.WebApplication
{
    public class AuthConfig
	{
		public static void InitialiseWebSecurity()
		{
			DatabaseInitialiser<Db>.InitialiseWebsecurity();
		}
	}
}