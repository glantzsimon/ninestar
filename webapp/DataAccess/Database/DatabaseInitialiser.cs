using System.Data.Entity;
using System.Data.Entity.Migrations;
using K9.DataAccess.Config;
using K9.DataAccess.Database.Seeds;
using WebMatrix.WebData;

namespace K9.DataAccess.Database
{
	public class DatabaseInitialiser : DbMigrationsConfiguration<Db>
	{

		public DatabaseInitialiser()
		{
			AutomaticMigrationsEnabled = AppConfig.AutomaticMigrationsEnabled;
			AutomaticMigrationDataLossAllowed = AppConfig.AutomaticMigrationDataLossAllowed;
		}

		public static void InitialiseWebsecurity()
		{
			if (!WebSecurity.Initialized)
			{
				WebSecurity.InitializeDatabaseConnection("DefaultConnection", "User", "Id", "UserName", true);
			}
		}

		protected override void Seed(Db context)
		{
			CountriesSeeder.SeedCountries(context);
			SchoolSeeder.SeedSchool(context);
		}

	}

	public class UsersAndRolesInitialiser
	{
		
		public static void Seed()
		{
			var db = new Db();
			UsersAndRolesSeeder.SeedUsersAndRoles(db);
			db.Dispose();
		}

	}
}
