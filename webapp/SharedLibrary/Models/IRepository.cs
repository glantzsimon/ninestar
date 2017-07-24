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

		List<TModel> CustomQuery<TModel>(string sql) where TModel : class;

		void Create(T item);

		void CreateBatch(List<T> items);

		void Update(T item);

		void UpdateBatch(List<T> items);

		void Delete(int id);

		void DeleteBatch(List<int> items);

		void Delete(T item);

		void DeleteBatch(List<T> items);

		bool Exists(int id);

		bool Exists(string query);

		bool Exists(Expression<Func<T, bool>> expression);

		List<T> Find(string name);

		IQueryable<T> Find(Expression<Func<T, bool>> expression);

		T Find(int id);

		List<T> GetBy<T2>(int id) where T2 : class, IObjectBase;

		List<T> GetAllBy<T2, T3>(int id)
			where T2 : class, IObjectBase
			where T3 : class, IObjectBase;

		void Dispose();

	}
}
