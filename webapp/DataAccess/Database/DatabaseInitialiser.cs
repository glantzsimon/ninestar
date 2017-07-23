using System;
using System.Data.Entity.Migrations;
using System.IO;
using K9.DataAccess.Config;
using K9.DataAccess.Database.Seeds;
using K9.SharedLibrary.Helpers;
using WebMatrix.WebData;

namespace K9.DataAccess.Database
{
	public class DatabaseInitialiser : DbMigrationsConfiguration<Db>
	{

		public DatabaseInitialiser()
		{
			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/appsettings.json"));
			var dbConfig = ConfigHelper.GetConfiguration<DatabaseConfiguration>(json);

			AutomaticMigrationsEnabled = dbConfig.Value.AutomaticMigrationsEnabled;
			AutomaticMigrationDataLossAllowed = dbConfig.Value.AutomaticMigrationDataLossAllowed;
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
