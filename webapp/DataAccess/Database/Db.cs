using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using K9.DataAccess.Models;

namespace K9.DataAccess.Database
{
	public class Db : DbContext
	{

		public Db()
			: base("name=DefaultConnection")
		{
			Configuration.LazyLoadingEnabled = false;
			Configuration.ProxyCreationEnabled = false;
		}


		#region Tables

		public DbSet<User> Users { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Enrollment> Enrollments { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<RolePermission> RolePermissions { get; set; }
		public DbSet<Message> Messages { get; set; }
		
		#endregion


		#region Event Handlers

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
		}

		#endregion

	}
}
