
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using K9.DataAccess.Models;

namespace K9.DataAccess.Respositories
{
	public interface IRepository<T> where T : ObjectBase
	{

		List<T> List();

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
