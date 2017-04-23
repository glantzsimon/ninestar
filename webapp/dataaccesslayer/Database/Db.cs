using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using K9.DataAccess.Models;
using K9.SharedLibrary.Interfaces;

namespace K9.DataAccess.Database
{
	public class Db : DbContext
	{

		public Db()
			: base("name=DefaultConnection")
		{
		}

		#region Tables

		public DbSet<User> Users { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Enrollment> Enrollments { get; set; }
		public DbSet<Student> Students { get; set; }
		
		#endregion

		#region Event Handlers

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();	
		}

		#endregion


		#region Methods

		public void Create<T>(T item) where T : ObjectBase, IIdentity
		{
			item.UpdateAuditFields();
			Set<T>().Add(item);
			SaveChanges();
		}

		public void Update<T>(T item) where T : ObjectBase, IIdentity
		{
			item.UpdateAuditFields();
			Set<T>().Attach(item);
			Entry(item).State = EntityState.Modified;
			SaveChanges();
		}

		public void Delete<T>(int id) where T : ObjectBase, IIdentity
		{
			T item = Set<T>().Find(id);
			if (item == null)
			{
				throw new IndexOutOfRangeException();
			}
			Delete(item);
		}

		public void Delete<T>(T item) where T : ObjectBase, IIdentity
		{
			Set<T>().Attach(item);
			Set<T>().Remove(item);
			SaveChanges();
		}

		public bool Exists<T>(int id) where T : ObjectBase, IIdentity
		{
			T item = Set<T>().Find(id);
			return item != null;
		}

		public bool Exists<T>(string name) where T : ObjectBase, IIdentity
		{
			return Set<T>().Any(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
		}

		public bool Exists<T>(Expression<Func<T, bool>> expression)
			where T : ObjectBase, IIdentity
		{
			return Set<T>().Where(expression).Any();
		}

		public IQueryable<T> Find<T>(string name) where T : ObjectBase, IIdentity
		{
			return Set<T>().Where(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
		}

		public IQueryable<T> Find<T>(Expression<Func<T, bool>> expression)
			where T : ObjectBase, IIdentity
		{
			return Set<T>().Where(expression);
		}

		public T Find<T>(params object[] keyValues)
			where T : ObjectBase, IIdentity
		{
			return Set<T>().Find(keyValues);
		}

		#endregion
	}
}
