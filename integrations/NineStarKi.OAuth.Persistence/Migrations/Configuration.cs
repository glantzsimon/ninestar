using System.Data.Entity.Migrations;

namespace NineStarKi.OAuth.Persistence.Migrations
{
    /// <summary>
    /// A separate EF6 Code First migration stream: it is NOT registered with
    /// the main website's automatic Startup DbMigrator.Update().
    /// </summary>
    public sealed class Configuration : DbMigrationsConfiguration<OAuthStoreContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            ContextKey = "NineStarKi.ChatGPT.OAuthStore";
            MigrationsDirectory = "Migrations";
        }

        protected override void Seed(OAuthStoreContext context)
        {
            // Deliberately empty: clients and redirect URIs must be registered
            // explicitly through OpenIddict's manager after review.
        }
    }
}
