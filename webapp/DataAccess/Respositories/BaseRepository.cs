
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using K9.DataAccess.Extensions;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Respositories
{
	public class BaseRepository<T> : IRepository<T> where T : class, IObjectBase
	{
		private readonly DbContext _db;

		public BaseRepository(DbContext db)
		{
			_db = db;
		}

		public int GetCount(string whereClause = "")
		{
			return _db.GetCount<T>(whereClause);
		}

		public string GetName(string tableName, int id)
		{
			return _db.GetName(tableName, id);
		}

		public List<T> GetQuery(string sql)
		{
			return _db.GetQuery<T>(sql);
		}

		public List<T> List()
		{
			return _db.List<T>();
		}

		public List<ListItem> ItemList()
		{
			return _db.GetQuery<ListItem>(string.Format("SELECT [Id], [Name] FROM [{0}] ORDER BY [Name]", typeof(T).Name));
		}

		public void Create(T item)
		{
			item.UpdateAuditFields();
			_db.Create(item);
		}

		public void CreateBatch(List<T> items)
		{
			items.ForEach(x => x.UpdateAuditFields());
			_db.CreateBatch(items);
		}

		public void Update(T item)
		{
			item.UpdateAuditFields();
			_db.Update(item);
		}

		public void UpdateBatch(List<T> items)
		{
			items.ForEach(x => x.UpdateAuditFields());
			_db.UpdateBatch(items);
		}

		public void Delete(int id)
		{
			_db.Delete<T>(id);
		}

		public void DeleteBatch(List<int> ids)
		{
			_db.DeleteBatch<T>(ids);
		}

		public void Delete(T item)
		{
			_db.Delete(item);
		}

		public void DeleteBatch(List<T> items)
		{
			_db.DeleteBatch(items);
		}

		public bool Exists(int id)
		{
			return _db.Exists<T>(id);
		}

		public bool Exists(string query)
		{
			return _db.Exists<T>(query);
		}

		public bool Exists(Expression<Func<T, bool>> expression)
		{
			return _db.Exists(expression);
		}

		public List<T> Find(string name)
		{
			return _db.Find<T>(name);
		}

		public IQueryable<T> Find(Expression<Func<T, bool>> expression)
		{
			return _db.Find(expression);
		}

		public T Find(params object[] keyValues)
		{
			return _db.Find<T>(keyValues);
		}

		/// <summary>
		/// Get items by a foreign key
		/// </summary>
		/// <typeparam name="T2">The type of entity that foreign key belongs to</typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public List<T> GetBy<T2>(int id)
			where T2 : class, IObjectBase
		{
			return _db.GetQuery<T>(string.Format("SELECT * FROM [{0}] WHERE [{1}] = {2}",
				typeof(T2).Name,
				typeof(T).GetForeignKeyName(),
				id));
		}

		/// <summary>
		/// Get all items by foreign key, right-joined by the right-most entity
		/// </summary>
		/// <typeparam name="T2">The type of entity that belongs to right join</typeparam>
		/// <typeparam name="T3">The type of entity that the foreign key belongs to</typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public List<T> GetAllBy<T2, T3>(int id)
			where T2 : class, IObjectBase
			where T3 : class, IObjectBase
		{
			var allItems = _db.List<T3>();
			var items = _db.GetQuery<T>(string.Format("SELECT [{1}].[Id] AS [{2}], {4} AS {3}, [{0}].* FROM [{0}] RIGHT JOIN [{1}] ON [{0}].[{2}] = [{1}].[Id] AND [{0}].[{3}] = {4}",
				typeof(T).Name,
				typeof(T3).Name,
				typeof(T3).GetForeignKeyName(),
				typeof(T2).GetForeignKeyName(),
				id));

			foreach (var item in items)
			{
				var foreignKeyId = (int)item.GetProperty(typeof(T3).GetForeignKeyName());
				item.SetProperty(typeof(T3).Name, allItems.First(x => x.Id == foreignKeyId));
			}

			return items;
		}

		#region EventHandlers

		public void Dispose()
		{
			if (_db != null)
			{
				_db.Dispose();
			}
		}

		#endregion


	}
}
