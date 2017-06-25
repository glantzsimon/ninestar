
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using K9.DataAccess.Extensions;
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
			return _db.GetQuery<ListItem>(string.Format("SELECT ID, Name FROM {0} ORDER BY Name", typeof(T).Name));
		}

		public void Create(T item)
		{
			item.UpdateAuditFields();
			_db.Create(item);
		}

		public void Update(T item)
		{
			item.UpdateAuditFields();
			_db.Update(item);
		}

		public void Delete(int id)
		{
			_db.Delete<T>(id);
		}

		public void Delete(T item)
		{
			_db.Delete(item);
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
