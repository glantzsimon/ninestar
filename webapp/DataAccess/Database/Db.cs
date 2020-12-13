using System.Data.Entity;
using K9.DataAccessLayer.Models;

namespace K9.DataAccessLayer.Database
{
    public class LocalDb : Base.DataAccessLayer.Database.Db
	{
	    public DbSet<Donation> Donations { get; set; }
	    public DbSet<NineStarKiPersonalProfile> EnergyInfos { get; set; }
    }
}
