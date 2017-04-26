using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using K9.DataAccess.Models;
using K9.SharedLibrary.Interfaces;

namespace K9.DataAccess.Extensions
{
	public static class DbContextExtensions
	{

		public static void Create<T>(this DbContext context, T item) where T : ObjectBase, IIdentity
		{
			context.Set<T>().Add(item);
			context.SaveChanges();
		}

		public static void Update<T>(this DbContext context, T item) where T : ObjectBase, IIdentity
		{
			context.Set<T>().Attach(item);
			context.Entry(item).State = EntityState.Modified;
			context.SaveChanges();
		}

		public static void Delete<T>(this DbContext context, int id) where T : ObjectBase, IIdentity
		{
			T item = context.Set<T>().Find(id);
			if (item == null)
			{
				throw new IndexOutOfRangeException();
			}
			Delete(context, item);
		}

		public static void Delete<T>(this DbContext context, T item) where T : ObjectBase, IIdentity
		{
			context.Set<T>().Attach(item);
			context.Set<T>().Remove(item);
			context.SaveChanges();
		}

		public static bool Exists<T>(this DbContext context, int id) where T : ObjectBase, IIdentity
		{
			T item = context.Set<T>().Find(id);
			return item != null;
		}

		public static bool Exists<T>(this DbContext context, string name) where T : ObjectBase, IIdentity
		{
			return context.Set<T>().Any(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
		}

		public static bool Exists<T>(this DbContext context, Expression<Func<T, bool>> expression)
			where T : ObjectBase, IIdentity
		{
			return context.Set<T>().Where(expression).Any();
		}

		public static IQueryable<T> Find<T>(this DbContext context, string name) where T : ObjectBase, IIdentity
		{
			return context.Set<T>().Where(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
		}

		public static IQueryable<T> Find<T>(this DbContext context, Expression<Func<T, bool>> expression)
			where T : ObjectBase, IIdentity
		{
			return context.Set<T>().Where(expression);
		}

		public static T Find<T>(this DbContext context, params object[] keyValues)
			where T : ObjectBase, IIdentity
		{
			return context.Set<T>().Find(keyValues);
		}

	}
}
