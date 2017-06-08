using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Extensions
{
	public static class DbContextExtensions
	{

		public static List<T> GetQuery<T>(this DbContext context, string sql) where T : class
		{
			return Dapper.SqlMapper.Query<T>(context.Database.Connection, sql).ToList();
		}

		public static List<T> List<T>(this DbContext context) where T : class, IObjectBase
		{
			return context.Set<T>().ToList();
		}

		public static int GetCount<T>(this DbContext context, string whereClause = "") where T : class, IObjectBase
		{
			return context.Database.SqlQuery<int>(string.Format("SELECT COUNT(*) FROM {0} {1}", typeof(T).Name, whereClause)).First();
		}

		public static void Create<T>(this DbContext context, T item) where T : class, IObjectBase
		{
			context.Set<T>().Add(item);
			context.SaveChanges();
		}

		public static void Update<T>(this DbContext context, T item) where T : class, IObjectBase
		{
			context.Set<T>().Attach(item);
			context.Entry(item).State = EntityState.Modified;
			context.SaveChanges();
		}

		public static void Delete<T>(this DbContext context, int id) where T : class, IObjectBase
		{
			T item = context.Set<T>().Find(id);
			if (item == null)
			{
				throw new IndexOutOfRangeException();
			}
			Delete(context, item);
		}

		public static void Delete<T>(this DbContext context, T item) where T : class, IObjectBase
		{
			context.Set<T>().Attach(item);
			context.Set<T>().Remove(item);
			context.SaveChanges();
		}

		public static bool Exists<T>(this DbContext context, int id) where T : class, IObjectBase
		{
			T item = context.Set<T>().Find(id);
			return item != null;
		}

		public static bool Exists<T>(this DbContext context, string name) where T : class, IObjectBase
		{
			return context.Set<T>().Any(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
		}

		public static bool Exists<T>(this DbContext context, Expression<Func<T, bool>> expression)
			where T : class, IObjectBase
		{
			return context.Set<T>().Where(expression).Any();
		}

		public static IQueryable<T> Find<T>(this DbContext context, string name) where T : class, IObjectBase
		{
			return context.Set<T>().Where(t => t.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
		}

		public static IQueryable<T> Find<T>(this DbContext context, Expression<Func<T, bool>> expression)
			where T : class, IObjectBase
		{
			return context.Set<T>().Where(expression);
		}

		public static T Find<T>(this DbContext context, params object[] keyValues)
			where T : class, IObjectBase
		{
			return context.Set<T>().Find(keyValues);
		}

	}
}
