
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;
using K9.DataAccess.Respositories;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.DataSets
{

	public class DataSetsHelper : IDataSetsHelper
	{
		private readonly DbContext _db;
		private readonly IDataSets _datasets;

		public DataSetsHelper(DbContext db, IDataSets datasets)
		{
			_db = db;
			_datasets = datasets;
		}

		public List<ListItem> GetDataSet<T>(bool refresh = false) where T : class, IObjectBase
		{
			List<ListItem> dataset = null;
			if (refresh || !_datasets.Collection.ContainsKey(typeof(T)))
			{
				IRepository<T> repo = new BaseRepository<T>(_db);
				dataset = repo.ItemList();
				if (refresh)
				{
					_datasets.Collection[typeof(T)] = dataset;
				}
				else
				{
					_datasets.Collection.Add(typeof(T), dataset);
				}

			}
			return dataset;
		}

		public SelectList GetSelectList<T>(int selectedId, bool refresh = false) where T : class, IObjectBase
		{
			return new SelectList(GetDataSet<T>(refresh), "Id", "Name", selectedId);
		}
	}

}