using System.Data.Entity;
using WebMatrix.WebData;
using K9.DataAccess.Database.Seeds;

namespace K9.DataAccess.Database
{
	public class DatabaseInitialiserRecreate : DropCreateDatabaseIfModelChanges<Db>
	{

		public static void InitialiseWebsecurity()
		{
			if (!WebSecurity.Initialized)
			{
				WebSecurity.InitializeDatabaseConnection("DefaultConnection", "User", "Id", "UserName", true);
			}
		}

		protected override void Seed(Db context)
		{
			UsersAndRolesSeeder.SeedUsersAndRoles(context);
			CountriesSeeder.SeedCountries(context);
			SchoolSeeder.SeedSchool(context);
		}

	}
}
