using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace K9.SharedLibrary.Models
{
	public interface IRepository<T> where T : IObjectBase
	{

		int GetCount(string whereClause = "");

		string GetName(string tableName, int id);

		List<T> GetQuery(string sql);

		List<T> List();

		List<ListItem> ItemList();

		void Create(T item);

		void Update(T item);

		void Delete(int id);

		void Delete(T item);

		bool Exists(int id);

		bool Exists(string query);

		bool Exists(Expression<Func<T, bool>> expression);

		List<T> Find(string name);

		IQueryable<T> Find(Expression<Func<T, bool>> expression);

		T Find(params object[] keyValues);

		void Dispose();

	}
}
