
using System.Data.Entity.Migrations;

namespace K9.WebApplication
{
	public class DataConfig
	{
		public static void InitialiseDatabase()
		{
			var migrator = new DbMigrator(new K9.DataAccess.Database.DatabaseInitialiser());
			migrator.Update();
		}
	}
}