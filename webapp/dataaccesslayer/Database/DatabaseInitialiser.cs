using System.Data.Entity.Migrations;
using K9.DataAccess.Config;
using WebMatrix.WebData;

namespace K9.DataAccess.Database
{
	public partial class DatabaseInitialiser : DbMigrationsConfiguration<Db>
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
			SeedUsersAndRoles(context);
			SeedCountries(context);
			SeedSchool(context);
		}

	}
}
