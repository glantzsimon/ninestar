
using System.Collections.Generic;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.DataSets
{
	public interface IDropdownDataSets
	{
		List<IListItem> GetDataSet<T>(bool refresh = false) where T : class, IObjectBase;
	}
}