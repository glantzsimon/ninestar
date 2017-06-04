using System.Collections.Generic;
using System.Web.Mvc;

namespace K9.SharedLibrary.Models
{
	public interface IDataSetsHelper
	{
		List<ListItem> GetDataSet<T>(bool refresh = false) where T : class, IObjectBase;
		SelectList GetSelectList<T>(int selectedId, bool refresh = false) where T : class, IObjectBase;
	}
}