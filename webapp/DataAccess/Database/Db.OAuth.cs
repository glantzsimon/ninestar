// OpenIddict's EF6 mappings are compiled into the existing LocalDb model
// in ordinary Debug builds so EF6 can scaffold the correct next migration.
// No separate DbContext or migration history is permitted. The actual
// database migration belongs in webapp/DataAccess/Migrations/.
#if NINESTAR_OAUTH
using System.Data.Entity;
using OpenIddict.EntityFramework;

namespace K9.DataAccessLayer.Database
{
    public partial class LocalDb
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Official OpenIddict EF6 mapping. The four OAuth tables belong
            // to this same DbContext and the existing DataAccess migrations.
            modelBuilder.UseOpenIddict();
        }
    }
}
#endif
