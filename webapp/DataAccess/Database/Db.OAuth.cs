// OpenIddict's EF6 tables are part of the existing LocalDb model in EVERY
// build configuration. Keeping model mapping consistent with the generated
// EF6 migration snapshot is necessary even when the OAuth server is disabled.
//
// This only declares the model; it does not run migrations or activate OAuth.
// All schema changes remain in webapp/DataAccess/Migrations/.
using System.Data.Entity;
using OpenIddict.EntityFramework;
using OpenIddict.EntityFramework.Models;

namespace K9.DataAccessLayer.Database
{
    public partial class LocalDb
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.UseOpenIddict();

            // The committed OAuth migration was generated with OpenIddict 7,
            // which maps Type to 150 characters. Keep that existing schema
            // when using OpenIddict 6.4 for Visual Studio 2017 compatibility.
            modelBuilder.Entity<OpenIddictEntityFrameworkToken>()
                .Property(token => token.Type)
                .HasMaxLength(150);
        }
    }
}
