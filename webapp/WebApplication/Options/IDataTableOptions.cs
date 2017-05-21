using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.Options
{
	public interface IDataTableOptions
	{
		string DataUrl { get; set; }
		List<PropertyInfo> Columns { get; set; }
		void SetColumnsToIgnore(IIgnoreColumns ignoreColumns);
		MvcHtmlString GetColumnDefs();
	}
}