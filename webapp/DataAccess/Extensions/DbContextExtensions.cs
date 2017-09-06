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
			return context.Database.SqlQuery<int>($"SELECT COUNT(*) FROM [{typeof(T).Name}] {whereClause}").First();
		}

		public static string GetName(this DbContext context, string tableName, int id)
		{
			return Dapper.SqlMapper.Query<string>(context.Database.Connection,
			    $"SELECT Name FROM [{tableName}] WHERE [Id] = {id}").First();
		}

		public static void Create<T>(this DbContext context, T item) where T : class, IObjectBase
		{
			context.Set<T>().Add(item);
			context.SaveChanges();
		}

		public static void CreateBatch<T>(this DbContext context, List<T> items) where T : class, IObjectBase
		{
			foreach (var item in items)
			{
				context.Set<T>().Add(item);
			}
			context.SaveChanges();
		}

		public static void Update<T>(this DbContext context, T item) where T : class, IObjectBase
		{
			context.Set<T>().Attach(item);
			context.Entry(item).State = EntityState.Modified;
			context.SaveChanges();
		}

		public static void UpdateBatch<T>(this DbContext context, List<T> items) where T : class, IObjectBase
		{
			foreach (T item in items)
			{
				context.Set<T>().Attach(item);
				context.Entry(item).State = EntityState.Modified;
			}

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

		public static void DeleteBatch<T>(this DbContext context, List<int> ids) where T : class, IObjectBase
		{
			var itemsToDelete = new List<T>();
			foreach (var id in ids)
			{
				T item = context.Set<T>().Find(id);
				if (item == null)
				{
					throw new IndexOutOfRangeException();
				}
				itemsToDelete.Add(item);
			}
			DeleteBatch(context, itemsToDelete);
		}

		public static void Delete<T>(this DbContext context, T item) where T : class, IObjectBase
		{
			context.Set<T>().Attach(item);
			context.Set<T>().Remove(item);
			context.SaveChanges();
		}

		public static void DeleteBatch<T>(this DbContext context, List<T> items) where T : class, IObjectBase
		{
			foreach (var item in items)
			{
				context.Set<T>().Attach(item);
				context.Set<T>().Remove(item);
			}
			context.SaveChanges();
		}

		public static bool Exists<T>(this DbContext context, int id) where T : class, IObjectBase
		{
			return
				Dapper.SqlMapper.Query<int>(context.Database.Connection,
				    $"SELECT COUNT(*) FROM [{typeof(T).Name}] WHERE [Id] = {id}").First() > 0;
		}

		public static bool Exists<T>(this DbContext context, string query) where T : class, IObjectBase
		{
			return Dapper.SqlMapper.Query<T>(context.Database.Connection, query).Any();
		}

		public static bool Exists<T>(this DbContext context, Expression<Func<T, bool>> expression)
			where T : class, IObjectBase
		{
			return context.Set<T>().Where(expression).Any();
		}

		public static List<T> Find<T>(this DbContext context, string name) where T : class, IObjectBase
		{
			return
				Dapper.SqlMapper.Query<T>(context.Database.Connection,
				    $"SELECT * FROM [{typeof(T).Name}] WHERE [Name] = '{name}'").ToList();
		}

		public static IQueryable<T> Find<T>(this DbContext context, Expression<Func<T, bool>> expression)
			where T : class, IObjectBase
		{
			return context.Set<T>().Where(expression);
		}

		public static T Find<T>(this DbContext context, int id)
			where T : class, IObjectBase
		{
			return Dapper.SqlMapper.Query<T>(context.Database.Connection,
			    $"SELECT TOP 1 * FROM [{typeof(T).Name}] WHERE Id = {id}").FirstOrDefault();
		}

	}
}
