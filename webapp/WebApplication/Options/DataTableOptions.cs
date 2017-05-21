using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using K9.DataAccess.Models;
using K9.WebApplication.Extensions;

namespace K9.WebApplication.Options
{
	public class DataTableOptions : IDataTableOptions
	{
		private IIgnoreColumns _ignoreColumns;

		public string DataUrl { get; set; }
		public List<PropertyInfo> Columns { get; set; }

		public void SetColumnsToIgnore(IIgnoreColumns ignoreColumns)
		{
			_ignoreColumns = ignoreColumns;
		}

		private List<PropertyInfo> GetColumnsNotIgnored()
		{
			return Columns.Where(c => !_ignoreColumns.ColumnsToIgnore.Contains(c.Name)).ToList();
		}

		public MvcHtmlString GetColumnDefs()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(GetColumnsNotIgnored().Select(c => new { data = c.Name, type = c.GetDataTableType() }).ToArray()));
		}
	}
}