using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace K9.SharedLibrary.Models
{
	public interface IRepository<T> where T : IObjectBase
	{

		int GetCount(string whereClause = "");

		List<T> GetQuery(string sql);

		List<T> List();

		List<IListItem> ItemList();

		void Create(T item);

		void Update(T item);

		void Delete(int id);

		void Delete(T item);

		bool Exists(int id);

		bool Exists(string name);

		bool Exists(Expression<Func<T, bool>> expression);

		IQueryable<T> Find(string name);

		IQueryable<T> Find(Expression<Func<T, bool>> expression);

		T Find(params object[] keyValues);

		void Dispose();

	}
}
