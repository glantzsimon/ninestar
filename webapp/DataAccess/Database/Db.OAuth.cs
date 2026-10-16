// This file deliberately remains opt-in until the website's legacy
// packages.config/git-crypt dependencies have been reviewed and OpenIddict
// is installed in the existing DataAccess project.
//
// Enable NINESTAR_OAUTH only after the required NuGet packages have been
// installed across the existing solution and the DataAccess Code First
// migration has been generated against the COMPLETE LocalDb model.
// Never create a second DbContext or migrate an isolated OAuth context.
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
