
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Security;
using K9.DataAccess.Config;
using K9.SharedLibrary.Authentication;
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
				WebSecurity.InitializeDatabaseConnection("DefaultConnection", "User", "Id", "UserName", autoCreateTables: true);
			}
		}

		protected override void Seed(Db context)
		{
			SeedUsersAndRoles(context);
			SeedCountries(context);
		}

	}
}
