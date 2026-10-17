
using K9.Base.DataAccessLayer.Database;
using K9.DataAccessLayer.Database;
using K9.DataAccessLayer.Database.Seeds;
using System.Data.Entity.Migrations;
using WebMatrix.WebData;

namespace K9.WebApplication
{
    public class DataConfig
    {
        public static void InitialiseDatabase()
        {
#if NINESTAR_OAUTH_COMPILE_CHECK
            // Building the feature-flagged OAuth source is safe, but starting
            // the website would invoke EF6's automatic migration routine.
            // In this special Debug-only build mode, fail BEFORE touching DB.
            throw new System.InvalidOperationException(
                "OAuth compile-check mode cannot start the website or run EF migrations.");
#else
            var migrator = new DbMigrator(new DatabaseInitialiserLocal());
            migrator.Update();
#endif
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
            TimeZonesSeeder.Seed(new LocalDb());
        }
    }
}