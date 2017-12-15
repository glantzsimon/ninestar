using K9.Base.DataAccessLayer.Database;
using K9.DataAccessLayer.Database.Seeds;

namespace K9.DataAccessLayer.Database
{
    public class DatabaseInitialiserLocal : DatabaseInitialiser<LocalDb>
	{
        protected override void Seed(LocalDb db)
		{
            base.Seed(db);

		    PermissionsSeeder.Seed(db);
        }
	}
}
