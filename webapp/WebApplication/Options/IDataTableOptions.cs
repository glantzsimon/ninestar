using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace K9.WebApplication.Options
{
	public interface IDataTableOptions
	{
		string TableId { get; set; }
		string DataUrl { get; set; }
		List<PropertyInfo> Columns { get; set; }
		MvcHtmlString GetColumnDefs();
	}
}