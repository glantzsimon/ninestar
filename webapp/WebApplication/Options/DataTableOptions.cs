using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace K9.WebApplication.Options
{
	public class DataTableOptions
	{
		public string DataUrl { get; set; }
		public List<PropertyInfo> Columns { get; set; }

		public MvcHtmlString GetColumnDefs()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(Columns.Select(c => new { data = c.Name, type = c.GetDataTableType() }).ToArray()));
		}
	}
}