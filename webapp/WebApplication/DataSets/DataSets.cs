
using System;
using System.Collections.Generic;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.DataSets
{

	public class DataSets : IDataSets
	{
		private readonly IDictionary<Type, List<IListItem>> _collection;

		public IDictionary<Type, List<IListItem>> Collection
		{
			get { return _collection; }
		}

		public DataSets()
		{
			_collection = new Dictionary<Type, List<IListItem>>();
		}
	}

}