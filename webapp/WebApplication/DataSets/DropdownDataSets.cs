
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http.Validation;
using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.DataSets
{

	public class DropdownDataSets : IDropdownDataSets
	{
		private readonly DbContext _db;
		private readonly IDictionary<Type, List<IListItem>> _datasets;

		public static IDropdownDataSets Instance { set; get; }

		public DropdownDataSets(DbContext db)
		{
			_db = db;
			_datasets = new Dictionary<Type, List<IListItem>>
			{
				{
					typeof (Country),
					GetDataSet<Country>()
				}
			};
		}

		public List<IListItem> GetDataSet<T>(bool refresh = false) where T : class, IObjectBase
		{
			var dataset = _datasets[typeof(T)];
			if (refresh || dataset == null)
			{
				IRepository<T> repo = new BaseRepository<T>(_db);
				dataset = repo.ItemList();
				_datasets[typeof(T)] = dataset;
			}
			return dataset;
		}
		
	}

}