using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace K9.SharedLibrary.Models
{
	public interface IDataSetsHelper
	{
		List<ListItem> GetDataSet<T>(bool refresh = false) where T : class, IObjectBase;
		List<ListItem> GetDataSetFromEnum<T>(bool refresh = false);
		SelectList GetSelectList<T>(int? selectedId, bool refresh = false) where T : class, IObjectBase;
		SelectList GetSelectListFromEnum<T>(Enum selectedId, bool refresh = false);
		string GetName<T>(int? selectedId, bool refresh = false) where T : class, IObjectBase;
	}
}