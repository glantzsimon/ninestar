using System.Data.Entity;
using OpenIddict.EntityFramework;

namespace NineStarKi.OAuth.Persistence
{
    /// <summary>
    /// Dedicated EF6 store for OpenIddict authorisations, clients, scopes and tokens.
    /// The user/password/Membership tables are NOT mapped or touched.
    /// Shares the normal website's configured SQL Server connection, but uses
    /// its own migration history context key and explicit rollout.
    /// </summary>
    public sealed class OAuthStoreContext : DbContext
    {
        public OAuthStoreContext() : base("name=DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseOpenIddict();
        }
    }
}
