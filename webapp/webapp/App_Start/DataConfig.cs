
using System.Data.Entity.Migrations;
using K9.DataAccess.Database;

namespace K9.WebApplication
{
	public class DataConfig
	{
		public static void InitialiseDatabase()
		{
			var migrator = new DbMigrator(new DatabaseInitialiser());
			migrator.Update();
		}
	}
}