using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Options
{
	public interface IDataTableOptions
	{
		string TableId { get; }
		string DataUrl { get; set; }
		bool AllowCreate { get; set; }
		bool AllowEdit { get; set; }
		bool AllowDelete { get; set; }
		bool AllowDetail { get; set; }
		bool AllowCrud();
		List<PropertyInfo> GetColumns();
		List<string> GetColumnNames();
		MvcHtmlString GetColumnsJson();
		MvcHtmlString GetColumnDefsJson();
		string GetButtonRenderFunction();
		List<DataTableColumnInfo> GetColumnInfos();
	}
}