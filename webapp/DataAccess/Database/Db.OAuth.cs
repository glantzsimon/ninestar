// OpenIddict's EF6 tables are part of the existing LocalDb model in EVERY
// build configuration. Keeping model mapping consistent with the generated
// EF6 migration snapshot is necessary even when the OAuth server is disabled.
//
// This only declares the model; it does not run migrations or activate OAuth.
// All schema changes remain in webapp/DataAccess/Migrations/.
using System.Data.Entity;
using OpenIddict.EntityFramework;

namespace K9.DataAccessLayer.Database
{
    public partial class LocalDb
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.UseOpenIddict();
        }
    }
}
