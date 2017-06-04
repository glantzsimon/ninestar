using System.Collections.Generic;

namespace K9.SharedLibrary.Models
{
	public interface IDataSetsHelper
	{
		List<ListItem> GetDataSet<T>(bool refresh = false) where T : class, IObjectBase;
	}
}