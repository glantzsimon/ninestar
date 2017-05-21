
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using K9.DataAccess.Extensions;
using K9.DataAccess.Models;

namespace K9.DataAccess.Respositories
{
	public class BaseRepository<T> : IRepository<T> where T : ObjectBase
	{
		private readonly DbContext _db;

		public BaseRepository(DbContext db)
		{
			_db = db;
		}

		public List<T> GetQuery(string sql)
		{
			return _db.GetQuery<T>(sql);
		}

		public List<T> List()
		{
			return _db.List<T>();
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

		public bool Exists(string name)
		{
			return _db.Exists<T>(name);
		}

		public bool Exists(Expression<Func<T, bool>> expression)
		{
			return _db.Exists(expression);
		}

		public IQueryable<T> Find(string name)
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
