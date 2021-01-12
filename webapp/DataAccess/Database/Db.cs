using K9.DataAccessLayer.Models;
using System.Data.Entity;

namespace K9.DataAccessLayer.Database
{
    public class LocalDb : Base.DataAccessLayer.Database.Db
	{
	    public DbSet<Contact> Contacts { get; set; }
	    public DbSet<Donation> Donations { get; set; }
	    public DbSet<MembershipOption> MembershipOptions { get; set; }
	    public DbSet<UserMembership> UserMemberships { get; set; }
	    public DbSet<PromoCode> PromoCodes { get; set; }
	    public DbSet<UserPromoCode> UserPromoCodes { get; set; }
	    public DbSet<Consultation> Consultations { get; set; }
	    public DbSet<UserConsultation> UserConsultations { get; set; }
    }
}
