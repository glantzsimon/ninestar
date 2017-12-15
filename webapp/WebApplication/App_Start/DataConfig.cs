
using K9.Base.DataAccessLayer.Database;
using K9.DataAccessLayer.Database;
using System.Data.Entity.Migrations;
using K9.DataAccessLayer.Database.Seeds;
using WebMatrix.WebData;

namespace K9.WebApplication
{
    public class DataConfig
    {
        public static void InitialiseDatabase()
        {
            var migrator = new DbMigrator(new DatabaseInitialiserLocal());
            migrator.Update();
        }

        public static void InitialiseWebsecurity()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "User", "Id", "UserName", true);
            }
        }

        public static void InitialiseUsersAndRoles()
        {
            UsersAndRolesInitialiser.Seed();
            PermissionsSeeder.Seed(new LocalDb());
        }
    }
}